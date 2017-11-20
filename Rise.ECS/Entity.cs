using System;
using System.Collections.Generic;
namespace Rise
{
    public class Entity : IComparable<Entity>
    {
        Scene scene;
        Entity parent;
        List<Entity> children;
        internal bool cleanup;
        List<Component> components;

        //One thing I dislike is "deferred removal" of entities. I want the entity
        //to be removed from the scene the moment I call RemoveChild()... but the problem
        //with this is that it modifies the entity list, so if anything is removed during
        //an iterater, the iterator will be corrupted.
        //To solve this, instead of slicing the list, I "null" the index where the Entity
        //exists, so the array keeps its shape and size. Then, at the end of the frame,
        //we loop through and clean up all the null slots when it is safe to do so in one go.
        //This means that iterators have to null-check as they iterate, but can always count
        //on visiting each entity that exists, and also only visiting them once during iteration.
        //To optimize slot-nulling, there is a global counter, and each time an entity is added
        //to anything, it increments this value and uses it as its "searchIndex".
        //This means that the entities in the list are *always*, by default, sorted by their
        //search index, as later additions will always have a higher number.
        //Since it now technically a sorted list, we can search it using BinarySearch(), whcih
        //is considerably faster than calling IndexOf(), as it has to visit far less slots
        //to locate the desired index.
        static ulong nextSearchIndex;
        ulong searchIndex;

        Vector2 position;
        Vector2 scale;
        float angle;
        Matrix3x2 matrix;
        bool matrixChanged;

        public Entity()
        {
            
        }

        public Scene Scene
        {
            get { return scene; }
            set
            {
                if (scene != value)
                {
                    if (parent != null)
                        parent.RemoveChild(this);
                    else if (scene != null)
                        scene.RemoveEntity(this);
                    scene?.AddEntity(this);
                }
            }
        }

        public Entity Parent
        {
            get { return parent; }
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.RemoveChild(this);
                    else if (scene != null)
                        scene.RemoveEntity(this);
                    value?.AddChild(this);
                }
            }
        }

        public bool IsRoot
        {
            get { return scene != null && parent == null; }
        }

        internal void AssignSearchIndex()
        {
            //ulong.MaxValue is *very* large so this realistically will never happen
            //if (nextSearchIndex == ulong.MaxValue)
            //    throw new Exception("Houston, we have a problem.");

            //If we wanted to be clever and solve this, we would just have to reset the
            //search indexer to zero, then scan all entities and re-update their indices
            
            searchIndex = nextSearchIndex++;
        }

        //Used by BinarySorty() to speed up entity removal
        public int CompareTo(Entity e)
        {
            return searchIndex.CompareTo(e.searchIndex);
        }

        //An entity can either be added as a child to an existing one, or to a scene
        public T AddChild<T>(T child) where T : Entity
        {
            if (child.parent != null)
                throw new Exception("Entity already has a parent.");
            if (child.scene != null)
                throw new Exception("Entity is already in a scene.");
            if (child.IsAncestorOf(this))
                throw new Exception("Cannot add an ancestor as a child.");

            child.parent = this;
            child.AssignSearchIndex();
            children.Add(child);

            if (child.cleanup)
                TriggerCleanup();

            if (scene != null)
                child.AddedToScene(scene);

            return child;
        }

        //A child can only be removed from its parent
        public T RemoveChild<T>(T child) where T : Entity
        {
            if (child.parent != this)
                throw new Exception("Entity is not a child.");

            TriggerCleanup();

            children[children.BinarySearch(child)] = null;
            child.parent = null;

            if (scene != null)
                child.RemovedFromScene();

            return child;
        }

        public void RemoveSelf()
        {
            if (parent != null)
                parent.RemoveChild(this);
            else
                scene?.RemoveEntity(this);
        }

        public T AddComponent<T>(T comp) where T : Component
        {
            if (comp.Entity != null)
                throw new Exception("An entity already owns this component.");

            components.Add(comp);
            comp.AddedToEntity(this);

            return comp;
        }

        public T RemoveComponent<T>(T comp) where T : Component
        {
            if (comp.Entity != this)
                throw new Exception("Component not owned by this Entity.");

            //Could use the indexing trick here, but as entities are expected to
            //have & keep a set of components, it's not really worth it.
            //Entities are removed at a greater frequency, so they use it

            components[components.IndexOf(comp)] = null;
            comp.RemovedFromEntity();

            return comp;
        }

        internal void AddedToScene(Scene s)
        {
            scene = s;
            s.EntityAdded(this);

            if (children != null)
                for (int i = 0, n = children.Count; i < n; ++i)
                    children[i]?.AddedToScene(s);
        }

        internal void RemovedFromScene()
        {
            if (children != null)
                for (int i = children.Count - 1; i >= 0; --i)
                    children[i]?.RemovedFromScene();
            
            scene.EntityRemoved(this);
            scene = null;
        }

        internal void TriggerCleanup()
        {
            if (!cleanup)
            {
                cleanup = true;
                if (parent != null)
                    parent.TriggerCleanup();
                else if (scene != null)
                    scene.TriggerCleanup();
            }
        }

        internal void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;
                for (int i = children.Count - 1; i >= 0; --i)
                {
                    if (children[i] != null)
                        children[i].Cleanup();
                    else
                        children.RemoveAt(i);
                }
            }
        }

        void MatrixChanged()
        {
            if (!matrixChanged)
            {
                matrixChanged = true;
                if (children != null)
                    for (int i = 0, n = children.Count; i < n; ++i)
                        children[i]?.MatrixChanged();
            }
        }

        public bool IsAncestorOf(Entity node)
        {
            if (scene == node.scene)
            {
                var p = node.parent;
                while (p != null)
                {
                    if (p == this)
                        return true;
                    p = p.parent;
                }
            }
            return false;
        }

        public bool IsDescendentOf(Entity node)
        {
            return node.IsAncestorOf(this);
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    MatrixChanged();
                }
            }
        }

        public float X
        {
            get { return position.X; }
            set
            {
                if (position.X != value)
                {
                    position.X = value;
                    MatrixChanged();
                }
            }
        }

        public float Y
        {
            get { return position.Y; }
            set
            {
                if (position.Y != value)
                {
                    position.Y = value;
                    MatrixChanged();
                }
            }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    MatrixChanged();
                }
            }
        }

        public float ScaleX
        {
            get { return scale.X; }
            set
            {
                if (scale.X != value)
                {
                    scale.X = value;
                    MatrixChanged();
                }
            }
        }

        public float ScaleY
        {
            get { return scale.Y; }
            set
            {
                if (scale.Y != value)
                {
                    scale.Y = value;
                    MatrixChanged();
                }
            }
        }

        public float Angle
        {
            get { return angle; }
            set
            {
                if (angle != value)
                {
                    angle = value;
                    MatrixChanged();
                }
            }
        }

        public Matrix3x2 Matrix
        {
            get
            {
                if (matrixChanged)
                {
                    //TODO: can probably optimize this w/ mutable matrix transformations
                    matrixChanged = false;
                    matrix = Matrix3x2.Scale(scale) * Matrix3x2.Rotation(angle) * Matrix3x2.Translation(position);
                    if (parent != null)
                        matrix *= parent.Matrix;
                }
                return matrix;
            }
        }
    }
}
