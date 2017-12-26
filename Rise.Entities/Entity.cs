using System;
using System.Collections.Generic;
namespace Rise.Entities
{
    public class Entity : IComparable<Entity>
    {
        public event Action OnAdded;
        public event Action<Scene> OnRemoved;

        static Predicate<Component> whereNull = e => e == null;

        public Scene Scene { get; private set; }
        internal ulong sceneAddIndex;

        Entity parent;

        Vector2 position;
        Vector2 scale = Vector2.One;
        float angle;
        Matrix3x2 matrix;
        bool matrixChanged = true;

        List<Component> components = new List<Component>();
        bool cleanup;

        bool changeLock;

        public Entity()
        {
            
        }

        internal void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;
                components.RemoveAll(whereNull);
            }
        }

        public T Add<T>(T component) where T : Component
        {
            if (component.Entity != null)
                throw new Exception("Component is already on an entity.");

            components.Add(component);
            component.Added(this);

            return component;
        }

        public void Remove(Component component)
        {
            if (component.Entity != this)
                throw new Exception("Component is not on this entity.");
            
            components[components.IndexOf(component)] = null;
            component.Removed();

            TriggerCleanup();
        }

        public int CompareTo(Entity other)
        {
            return sceneAddIndex.CompareTo(other.sceneAddIndex);
        }

        internal void Added(Scene scene, uint addIndex)
        {
            if (changeLock)
                throw new Exception("Cannot add entities during add/remove callbacks.");
            changeLock = true;

            Scene = scene;
            sceneAddIndex = addIndex;
            OnAdded?.Invoke();

            for (int i = 0, n = components.Count; i < n; ++i)
                components[i]?.AddedToScene(true);

            if (cleanup)
                Scene.TriggerCleanupEntities();

            changeLock = false;
        }

        internal void Removed()
        {
            if (changeLock)
                throw new Exception("Cannot remove entities during add/remove callbacks.");

            changeLock = true;

            var s = Scene;
            Scene = null;
            OnRemoved?.Invoke(s);

            for (int i = 0, n = components.Count; i < n; ++i)
                components[i]?.RemovedFromScene(s, true);

            changeLock = false;
        }

        protected void TriggerCleanup()
        {
            if (!cleanup)
            {
                cleanup = true;
                Scene?.TriggerCleanupEntities();
            }
        }

        public bool IsAncestorOf(Entity e)
        {
            if (e.parent != null)
            {
                var p = e.parent;
                while (p != null)
                {
                    if (p == this)
                        return true;
                    p = p.parent;
                }
            }
            return false;
        }

        public bool IsDescendantOf(Entity e)
        {
            return e.IsAncestorOf(this);
        }

        public void LookAt(Vector2 position)
        {
            Angle = (position - GlobalPosition).Angle;
        }

        public void SetParent(Entity p, bool keepPosition)
        {
            if (keepPosition)
            {
                var pos = GlobalPosition;
                Parent = p;
                Position = GlobalToLocal(pos);
            }
            else
                Parent = p;
        }

        public Vector2 LocalToGlobal(Vector2 p)
        {
            if (MatrixChanged)
                UpdateMatrix();
            return matrix.TransformPoint(p);
        }

        public Vector2 GlobalToLocal(Vector2 p)
        {
            if (MatrixChanged)
                UpdateMatrix();
            Matrix3x2 inv;
            matrix.Invert(out inv);
            return inv.TransformPoint(p);
        }

        //TODO: entity & parent should both be in the same scene
        //should handle the case where either is removed/etc.
        public Entity Parent
        {
            get { return parent; }
            set
            {
                if (parent != value)
                {
                    if (value != null && IsAncestorOf(value))
                        throw new Exception("Infinite parent loop.");

                    parent = value;
                    matrixChanged = true;
                }
            }
        }

        public Vector2 GlobalPosition
        {
            get { return Matrix.TransformPoint(position); }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    matrixChanged = true;
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
                    matrixChanged = true;
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
                    matrixChanged = true;
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
                    matrixChanged = true;
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
                    matrixChanged = true;
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
                    matrixChanged = true;
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
                    matrixChanged = true;
                }
            }
        }

        public bool MatrixChanged
        {
            get { return matrixChanged || (parent != null && parent.MatrixChanged); }
        }

        void UpdateMatrix()
        {
            matrixChanged = false;
            matrix = Matrix3x2.Scale(scale) * Matrix3x2.Rotation(angle) * Matrix3x2.Translation(position);
            if (parent != null)
                matrix *= parent.Matrix;
        }

        public Matrix3x2 Matrix
        {
            get
            {
                if (MatrixChanged)
                    UpdateMatrix();
                return matrix;
            }
        }
    }
}
