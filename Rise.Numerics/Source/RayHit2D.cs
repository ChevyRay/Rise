using System;
namespace Rise
{
    public struct RayHit2D : IEquatable<RayHit2D>
    {
        public float Distance;
        public Vector2 Normal;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public bool Equals(RayHit2D other)
        {
            return Distance == other.Distance && Normal.Equals(other.Normal);
        }
        public bool Equals(ref RayHit2D other)
        {
            return Distance == other.Distance && Normal.Equals(other.Normal);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Distance.GetHashCode();
                hash = hash * 23 + Normal.X.GetHashCode();
                hash = hash * 23 + Normal.Y.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(RayHit2D a, RayHit2D b)
        {
            return a.Equals(ref b);
        }

        public static bool operator !=(RayHit2D a, RayHit2D b)
        {
            return !a.Equals(ref b);
        }
    }
}
