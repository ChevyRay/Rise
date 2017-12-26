using System;
namespace Rise
{
    public struct Ray2D : IEquatable<Ray2D>
    {
        public Vector2 Point;
        public Vector2 Normal;

        public Ray2D(Vector2 point, Vector2 normal)
        {
            Point = point;
            Normal = normal;
        }

        public override bool Equals(object obj)
        {
            return obj is Ray2D && Equals((Ray2D)obj);
        }
        public bool Equals(ref Ray2D other)
        {
            return Point.Equals(other.Point) && Normal.Equals(other.Normal);
        }
        public bool Equals(Ray2D other)
        {
            return Equals(ref other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Point.X.GetHashCode();
                hash = hash * 23 + Point.Y.GetHashCode();
                hash = hash * 23 + Normal.X.GetHashCode();
                hash = hash * 23 + Normal.Y.GetHashCode();
                return hash;
            }
        }

        public Vector2 GetPoint(float dist)
        {
            return Point + Normal * dist;
        }

        public static bool operator ==(Ray2D a, Ray2D b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Ray2D a, Ray2D b)
        {
            return !a.Equals(ref b);
        }
    }
}
