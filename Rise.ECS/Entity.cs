using System;
using System.Collections.Generic;
namespace Rise
{
    public class Entity : IComparable<Entity>
    {
        static ulong nextSearchIndex;

        internal Scene scene;
        internal Entity parent;
        List<Entity> children;
        ulong searchIndex;
        internal bool cleanup;
        List<Component> components;

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

        internal void AssignSearchIndex()
        {
            searchIndex = nextSearchIndex++;
        }

        public int CompareTo(Entity e)
        {
            return searchIndex.CompareTo(e.searchIndex);
        }

        public T AddChild<T>(T child) where T : Entity
        {
            if (child.scene != null)
                throw new Exception("Entity is already in a scene.");
            if (child.parent != null)
                throw new Exception("Entity already has a parent.");
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
