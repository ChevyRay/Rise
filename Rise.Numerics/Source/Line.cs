using System;
namespace Rise
{
    public struct Line : IEquatable<Line>
    {
        public Vector2 A;
        public Vector2 B;

        public Line(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
        public Line(float x1, float y1, float x2, float y2)
        {
            A = new Vector2(x1, y1);
            B = new Vector2(x2, y2);
        }

        public Vector2 Axis
        {
            get { return Vector2.Normalize(B - A); }
        }

        public Vector2 LeftAxis
        {
            get { return Vector2.TurnLeft(Axis); }
        }

        public Vector2 RightAxis
        {
            get { return Vector2.TurnRight(Axis); }
        }

        public float Length
        {
            get { return Vector2.Distance(A, B); }
        }

        public float SquareLength
        {
            get { return Vector2.DistanceSquared(A, B); }
        }

        public float Angle
        {
            get { return Calc.Atan2(B.Y - A.Y, B.X - A.X); }
        }

        public Rectangle Bounds
        {
            get { return Rectangle.FromBounds(A, B); }
        }

        public float DistanceToPoint(Vector2 point)
        {
            var p = point - A;
            var n = Vector2.Normalize(B - A);
            float dot = Vector2.Dot(p, n);
            return Vector2.Distance(A + n * dot, point);
        }

        public override bool Equals(object obj)
        {
            return obj is Line && Equals((Line)obj);
        }
        public bool Equals(Line other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref Line other)
        {
            return A.Equals(other.A) && B.Equals(other.B);
        }

        public Vector2 GetPoint(float t)
        {
            return Vector2.Lerp(A, B, t);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + A.X.GetHashCode();
                hash = hash * 23 + A.Y.GetHashCode();
                hash = hash * 23 + B.X.GetHashCode();
                hash = hash * 23 + B.Y.GetHashCode();
                return hash;
            }
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            float dot = Vector2.Dot(A, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(B, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }
        public Vector2 Project(Vector2 p)
        {
            var axis = Axis;
            return A + axis * Vector2.Dot(p, axis);
        }

        public float SquareDistanceToPoint(Vector2 point)
        {
            var p = point - A;
            var n = Vector2.Normalize(B - A);
            float dot = Vector2.Dot(p, n);
            return Vector2.DistanceSquared(A + n * dot, point);
        }

        public override string ToString()
        {
            return string.Format("({0}),({1})", A, B);
        }

        public static bool operator ==(Line a, Line b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Line a, Line b)
        {
            return !a.Equals(ref b);
        }
    }
}

