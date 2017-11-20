using System;
using Rise;
namespace NodeIdea
{
    public class Transform : Node
    {
        Vector2 position;
        Vector2 scale = Vector2.One;
        float angle;
        Matrix3x2 matrix;
        bool matrixChanged = true;
        Predicate<Node> matrixChangedCallback;

        public Transform()
        {
            matrixChangedCallback = n =>
            {
                if (matrixChanged)
                    return false;
                matrixChanged = true;
                return true;
            };
        }
        public Transform(Vector2 position) : this()
        {
            this.position = position;
        }
        public Transform(Vector2 position, Vector2 scale, float angle) : this()
        {
            this.position = position;
            this.scale = scale;
            this.angle = angle;
        }
        public Transform(Vector2 position, float angle) : this()
        {
            this.position = position;
            this.scale = scale;
            this.angle = angle;
        }

        void MatrixChanged()
        {
            if (!matrixChanged)
            {
                matrixChanged = true;
                Propagate(matrixChangedCallback);
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
