using System;
namespace Rise
{
	public struct Ray3D : IEquatable<Ray3D>
	{
		public Vector3 Point;
		public Vector3 Normal;

		public Ray3D(Vector3 point, Vector3 normal)
		{
			Point = point;
			Normal = normal;
		}

		public override bool Equals(object obj)
		{
			return obj is Ray3D && Equals((Ray3D)obj);
		}
		public bool Equals(ref Ray3D other)
		{
			return Point.Equals(other.Point) && Normal.Equals(other.Normal);
		}
		public bool Equals(Ray3D other)
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
				hash = hash * 23 + Point.Z.GetHashCode();
				hash = hash * 23 + Normal.X.GetHashCode();
				hash = hash * 23 + Normal.Y.GetHashCode();
				hash = hash * 23 + Normal.Z.GetHashCode();
				return hash;
			}
		}

		public Vector3 GetPoint(float dist)
		{
			return Point + Normal * dist;
		}

		public static bool operator ==(Ray3D a, Ray3D b)
		{
			return a.Equals(ref b);
		}
		public static bool operator !=(Ray3D a, Ray3D b)
		{
			return !a.Equals(ref b);
		}
	}
}
