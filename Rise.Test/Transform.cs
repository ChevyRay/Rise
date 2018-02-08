using System;
namespace Rise.Test
{
    public class Transform
    {
        Vector2 position;
        ulong state;
        bool dirty;
        Matrix3x2 matrix;
        Transform parent;

        public Transform()
        {
            state = 1;
        }

        public Transform Parent
        {
            get { return parent; }
            set
            {
                if (parent != value)
                {
                    parent = value;
                    dirty = true;
                    ++state;
                }
            }
        }

        public Vector2 Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    dirty = true;
                    ++state;
                }
            }
        }

        public Matrix3x2 Matrix
        {
            get
            {
                if (Dirty)
                {
                    dirty = false;
                    matrix = Matrix3x2.Translation(position); // etc.
                    if (parent != null)
                    {
                        state = parent.state;
                        matrix = parent.Matrix * matrix;
                    }
                }
                return matrix;
            }
        }

        public bool Dirty
        {
            get { return dirty || (parent != null && state != parent.state); }
        }
    }
}
