using System;
namespace Rise
{
    public struct Ray : IEquatable<Ray>
    {
        public Vector2 Point;
        public Vector2 Normal;

        public Ray(Vector2 point, Vector2 normal)
        {
            Point = point;
            Normal = normal;
        }

        public override bool Equals(object obj)
        {
            return obj is Ray && Equals((Ray)obj);
        }
        public bool Equals(ref Ray other)
        {
            return Point.Equals(other.Point) && Normal.Equals(other.Normal);
        }
        public bool Equals(Ray other)
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

        public static bool operator ==(Ray a, Ray b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Ray a, Ray b)
        {
            return !a.Equals(ref b);
        }
    }
}
