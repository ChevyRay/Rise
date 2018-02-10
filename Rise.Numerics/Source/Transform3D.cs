using Rise;
using System;

namespace Rise
{
    public class Transform3D
    {
        public event Action OnChanged;

        private Transform3D parent;
        private Vector3 position;
        private Vector3 scale = Vector3.One;
        private Quaternion rotation = Quaternion.Identity;
        private Matrix4x4 matrix;
        private bool dirty = true;

        private void MakeDirty()
        {
            dirty = true;
            OnChanged?.Invoke();
        }

        public Transform3D Parent
        {
            get { return parent; }
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                        parent.OnChanged -= MakeDirty;

                    parent = value;

                    if (parent != null)
                        parent.OnChanged += MakeDirty;

                    MakeDirty();
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
                    MakeDirty();
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
                    MakeDirty();
                }
            }
        }

        public float Z
        {
            get { return position.Z; }
            set
            {
                if (position.Z != value)
                {
                    position.Z = value;
                    MakeDirty();
                }
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    MakeDirty();
                }
            }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;
                    MakeDirty();
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
                    MakeDirty();
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
                    MakeDirty();
                }
            }
        }

        public float ScaleZ
        {
            get { return scale.Z; }
            set
            {
                if (scale.Z != value)
                {
                    scale.Z = value;
                    MakeDirty();
                }
            }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    MakeDirty();
                }
            }
        }

        public Matrix4x4 Matrix
        {
            get
            {
                if (dirty)
                {
                    Matrix4x4.CreateTransform(ref position, ref rotation, ref scale, out matrix);
                    if (parent != null)
                        matrix = parent.Matrix * matrix;
                    dirty = false;
                }

                return matrix;
            }
        }

        public Vector3 GlobalPosition
        {
            get
            {
                if (parent != null)
                    return parent.Matrix.TransformPoint(position);
                return position;
            }
        }
    }
}
