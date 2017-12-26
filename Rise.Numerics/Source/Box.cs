using System;
namespace Rise
{
	public struct Box : IEquatable<Box>
	{
		public Vector3 Min;
		public Vector3 Max;

		public Box(Vector3 min, Vector3 max)
		{
			Min = min;
			Max = max;
		}

		public Vector3 Center
		{
			get { return Min + (Max - Min) * 0.5f; }
		}

		public Vector3 Size
		{
			get { return Max - Min; }
		}

		public Vector3 Extent
		{
			get { return (Max - Min) * 0.5f; }
		}

		public override bool Equals(object obj)
		{
			return obj is Box && Equals((Box)obj);
		}
		public bool Equals(ref Box other)
		{
			return Min.Equals(ref other.Min) && Max.Equals(ref other.Max);
		}
		public bool Equals(Box other)
		{
			return Equals(ref other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + Min.X.GetHashCode();
				hash = hash * 23 + Min.Y.GetHashCode();
				hash = hash * 23 + Min.Z.GetHashCode();
				hash = hash * 23 + Max.X.GetHashCode();
				hash = hash * 23 + Max.Y.GetHashCode();
				hash = hash * 23 + Max.Z.GetHashCode();
				return hash;
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

		public float Distance(ref Vector3 point)
		{
			return Geom3D.Distance(ref this, ref point);
		}
		public float Distance(Vector3 point)
		{
			return Geom3D.Distance(ref this, ref point);
		}
		public float Distance(ref Box box)
		{
			return Geom3D.Distance(ref this, ref box);
		}
		public float Distance(Box box)
		{
			return Geom3D.Distance(ref this, ref box);
		}

		public bool Intersects(ref Box box)
		{
			return Geom3D.Intersects(ref this, ref box);
		}
		public bool Intersects(Box box)
		{
			return Geom3D.Intersects(ref this, ref box);
		}
		public bool Intersects(ref Sphere sphere)
		{
			return Geom3D.Intersects(ref this, ref sphere);
		}
		public bool Intersects(Sphere sphere)
		{
			return Geom3D.Intersects(ref this, ref sphere);
		}
		public bool Intersects(ref Plane plane)
		{
			return Geom3D.Intersects(ref this, ref plane);
		}
		public bool Intersects(Plane plane)
		{
			return Geom3D.Intersects(ref this, ref plane);
		}

		public bool Contains(ref Vector3 p)
		{
			return Geom3D.Contains(ref this, ref p);
		}
		public bool Contains(Vector3 p)
		{
			return Geom3D.Contains(ref this, ref p);
		}
		public bool Contains(ref Box box)
		{
			return Geom3D.Contains(ref this, ref box);
		}
		public bool Contains(Box box)
		{
			return Geom3D.Contains(ref this, ref box);
		}
		public bool Contains(ref Sphere sphere)
		{
			return Geom3D.Contains(ref this, ref sphere);
		}
		public bool Contains(Sphere sphere)
		{
			return Geom3D.Contains(ref this, ref sphere);
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

		public static Box Conflate(ref Box a, ref Box b)
		{
			return new Box(new Vector3(Math.Min(a.Min.X, b.Min.X), Math.Min(a.Min.Y, b.Min.Y), Math.Min(a.Min.Z, b.Min.Z)),
			                  new Vector3(Math.Max(a.Max.X, b.Max.X), Math.Max(a.Max.Y, b.Max.Y), Math.Max(a.Max.Z, b.Max.Z)));
		}
		public static Box Conflate(Box a, Box b)
		{
			return Conflate(ref a, ref b);
		}

		public static Box CreateSize(Vector3 center, Vector3 size)
		{
			size *= 0.5f;
			return new Box(center - size, center + size);
		}
		public static Box CreateSize(Vector3 center, float size)
		{
			return CreateSize(center, new Vector3(size, size, size));
		}
		public static Box CreateSize(Vector3 size)
		{
			return CreateSize(Vector3.Zero, size);
		}
		public static Box CreateSize(float size)
		{
			return CreateSize(Vector3.Zero, new Vector3(size, size, size));
		}

		public static Box Lerp(ref Box a, ref Box b, float t)
		{
			return new Box(Vector3.Lerp(a.Min, b.Min, t), Vector3.Lerp(a.Max, b.Max, t));
		}
		public static Box Lerp(Box a, Box b, float t)
		{
			return Lerp(ref a, ref b, t);
		}

		public static Box Bezier(ref Box a, ref Box b, ref Box c, float t)
		{
			return new Box(Vector3.Bezier(a.Min, b.Min, c.Min, t), Vector3.Bezier(a.Max, b.Max, c.Max, t));
		}
		public static Box Bezier(Box a, Box b, Box c, float t)
		{
			return Bezier(ref a, ref b, ref c, t);
		}
		public static Box Bezier(ref Box a, ref Box b, ref Box c, ref Box d, float t)
		{
			return new Box(Vector3.Bezier(a.Min, b.Min, c.Min, d.Min, t), Vector3.Bezier(a.Max, b.Max, c.Max, d.Max, t));
		}
		public static Box Bezier(Box a, Box b, Box c, Box d, float t)
		{
			return Bezier(ref a, ref b, ref c, ref d, t);
		}

		public void GetP1(out Vector3 p)
		{
			p = Min;
		}
		public void GetP2(out Vector3 p)
		{
			p = Max;
		}
		public void GetP3(out Vector3 p)
		{
			p = new Vector3(Min.X, Max.Y, Min.Z);
		}
		public void GetP4(out Vector3 p)
		{
			p = new Vector3(Min.X, Min.Y, Max.Z);
		}
		public void GetP5(out Vector3 p)
		{
			p = new Vector3(Min.X, Max.Y, Max.Z);
		}
		public void GetP6(out Vector3 p)
		{
			p = new Vector3(Max.X, Min.Y, Max.Z);
		}
		public void GetP7(out Vector3 p)
		{
			p = new Vector3(Max.X, Max.Y, Min.Z);
		}
		public void GetP8(out Vector3 p)
		{
			p = new Vector3(Max.X, Min.Y, Min.Z);
		}

		public static bool operator ==(Box a, Box b)
		{
			return a.Equals(ref b);
		}
		public static bool operator !=(Box a, Box b)
		{
			return !a.Equals(ref b);
		}
	}
}
