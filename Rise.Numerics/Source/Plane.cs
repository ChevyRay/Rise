using System;
namespace Rise
{
	public struct Plane : IEquatable<Plane>
	{
		public Vector3 Normal;
		public float Distance;

		public Plane(Vector3 normal, float distance)
		{
			Normal = normal;
			Distance = distance;
		}
		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			Normal = Vector3.Cross(b - a, c - a).Normalized;
			Distance = -Vector3.Dot(ref Normal, ref a);
		}
        public Plane(Vector3 position, Vector3 normal)
            : this(normal, Vector3.Dot(ref position, ref normal))
        {
            
        }

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		public bool Equals(ref Plane plane)
		{
			return Distance == plane.Distance && Normal.Equals(ref plane.Normal);
		}
		public bool Equals(Plane plane)
		{
			return Equals(ref plane);
		}

		public Plane Normalized
		{
			get
			{
				Plane result;
				Normalize(ref this, out result);
				return result;
			}
		}

		public void Project(ref Vector3 point, out Vector3 result)
		{
			Geom3D.Project(ref this, ref point, out result);
		}
		public Vector3 Project(ref Vector3 point)
		{
			Vector3 result;
			Geom3D.Project(ref this, ref point, out result);
			return result;
		}

		public bool Intersects(ref Box box)
		{
			return Geom3D.Intersects(ref box, ref this);
		}
		public bool Intersects(Box box)
		{
			return Geom3D.Intersects(ref box, ref this);
		}
		public bool Intersects(ref Sphere sphere)
		{
			return Geom3D.Intersects(ref sphere, ref this);
		}
		public bool Intersects(Sphere sphere)
		{
			return Geom3D.Intersects(ref sphere, ref this);
		}

		public bool Raycast(ref Ray3D ray, out float dist)
		{
			return Geom3D.Raycast(ref this, ref ray, out dist);
		}
		public bool Raycast(Ray3D ray, out float dist)
		{
			return Geom3D.Raycast(ref this, ref ray, out dist);
		}
		public bool Raycast(ref Ray3D ray, out Vector3 point)
		{
			float dist;
			if (Geom3D.Raycast(ref this, ref ray, out dist))
			{
				point = ray.GetPoint(dist);
				return true;
			}
			point = ray.Point;
			return false;
		}
		public bool Raycast(Ray3D ray, out Vector3 point)
		{
			return Raycast(ref ray, out point);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + Normal.X.GetHashCode();
				hash = hash * 23 + Normal.Y.GetHashCode();
				hash = hash * 23 + Normal.Z.GetHashCode();
				hash = hash * 23 + Distance.GetHashCode();
				return hash;
			}
		}

		public float DistanceToPoint(ref Vector3 p)
		{
			return (float)Math.Abs((Normal.X * p.X + Normal.Y * p.Y + Normal.Z * p.Z) / Math.Sqrt(Normal.X * Normal.X + Normal.Y * Normal.Y + Normal.Z * Normal.Z));
		}
		public float DistanceToPoint(Vector3 p)
		{
			return DistanceToPoint(ref p);
		}

		public static void Normalize(ref Plane plane, out Plane result)
		{
			float mult = 1.0f / (float)Math.Sqrt(plane.Normal.X * plane.Normal.X + plane.Normal.Y * plane.Normal.Y + plane.Normal.Z * plane.Normal.Z);
			result.Normal.X = plane.Normal.X * mult;
			result.Normal.Y = plane.Normal.Y * mult;
			result.Normal.Z = plane.Normal.Z * mult;
			result.Distance = plane.Distance * mult;
		}

		public static bool operator ==(Plane a, Plane b)
		{
			return a.Equals(ref b);
		}
		public static bool operator !=(Plane a, Plane b)
		{
			return !a.Equals(ref b);
		}
	}
}
