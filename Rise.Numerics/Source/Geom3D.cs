using System;
namespace Rise
{
	public static class Geom3D
	{
		#region Intersects

		public static bool Intersects(ref BoundingBox a, ref BoundingBox b)
		{
			if (a.Min.X > b.Max.X || b.Min.X > a.Max.X)
				return false;
			if (a.Min.Y > b.Max.Y || b.Min.Y > a.Max.Y)
				return false;
			if (a.Min.Z > b.Max.Z || b.Min.Z > a.Max.Z)
				return false;
			return true;
		}
		public static bool Intersects(ref BoundingBox box, ref BoundingSphere sphere)
		{
			Vector3 vector;
			Vector3.Clamp(ref sphere.Center, ref box.Min, ref box.Max, out vector);
            return Vector3.DistanceSquared(ref sphere.Center, ref vector) <= sphere.Radius * sphere.Radius;
		}
		public static bool Intersects(ref BoundingSphere sphere, ref BoundingBox box)
		{
			return Intersects(ref box, ref sphere);
		}
		public static bool Intersects(ref BoundingBox box, ref Plane plane)
		{
			var min = new Vector3(
				plane.Normal.X >= 0f ? box.Min.X : box.Max.X,
				plane.Normal.Y >= 0f ? box.Min.Y : box.Max.Y,
				plane.Normal.Z >= 0f ? box.Min.Z : box.Max.Z
			);
			var max = new Vector3(
				plane.Normal.X >= 0f ? box.Max.X : box.Min.X,
				plane.Normal.Y >= 0f ? box.Max.Y : box.Min.Y,
				plane.Normal.Z >= 0f ? box.Max.Z : box.Min.Z
			);
			float distance = plane.Distance + Vector3.Dot(ref plane.Normal, ref max);
			if (distance > 0f)
				return false;
			distance = plane.Distance + Vector3.Dot(ref plane.Normal, ref min);
			if (distance < 0f)
				return false;
			return true;
		}
		public static bool Intersects(ref Plane plane, ref BoundingBox box)
		{
			return Intersects(ref box, ref plane);
		}
		public static bool Intersects(ref BoundingSphere a, ref BoundingSphere b)
		{
            return Vector3.DistanceSquared(a.Center, b.Center) < (a.Radius + b.Radius) * (a.Radius + b.Radius);
		}
		public static bool Intersects(ref BoundingSphere sphere, ref Plane plane)
		{
			float dist = Vector3.Dot(ref plane.Normal, ref sphere.Center) + plane.Distance;
			return dist < sphere.Radius && dist > -sphere.Radius;
		}
		public static bool Intersects(ref Plane plane, ref BoundingSphere sphere)
		{
			return Intersects(ref sphere, ref plane);
		}

		#endregion

		#region Contains

		public static bool Contains(ref BoundingBox box, ref Vector3 point)
		{
			return point.X > box.Min.X && point.X > box.Min.Y && point.X > box.Min.Z && point.X < box.Max.X && point.X < box.Max.Y && point.X < box.Max.Z;
		}
		public static bool Contains(ref BoundingBox a, ref BoundingBox b)
		{
			return b.Min.X >= a.Min.X && b.Min.Y >= a.Min.Y && b.Min.Z >= a.Min.Z
			    && b.Max.X <= a.Max.X && b.Max.Y <= a.Max.Y && b.Max.Z <= a.Max.Z;
		}
		public static bool Contains(ref BoundingBox box, ref BoundingSphere sphere)
		{
			Vector3 vector;
			Vector3.Clamp(ref sphere.Center, ref box.Min, ref box.Max, out vector);
            float distance = Vector3.DistanceSquared(ref sphere.Center, ref vector);
			return distance <= sphere.Radius * sphere.Radius
				&& box.Min.X + sphere.Radius <= sphere.Center.X
				&& sphere.Center.X <= box.Max.X - sphere.Radius
				&& box.Max.X - box.Min.X > sphere.Radius
				&& box.Min.Y + sphere.Radius <= sphere.Center.Y
				&& sphere.Center.Y <= box.Max.Y - sphere.Radius
				&& box.Max.Y - box.Min.Y > sphere.Radius
				&& box.Min.Z + sphere.Radius <= sphere.Center.Z
				&& sphere.Center.Z <= box.Max.Z - sphere.Radius
				&& box.Max.Z - box.Min.Z > sphere.Radius;
		}
		public static bool Contains(ref BoundingSphere sphere, ref Vector3 point)
		{
            return Vector3.DistanceSquared(ref sphere.Center, ref point) < sphere.Radius * sphere.Radius;
		}
		public static bool Contains(ref BoundingSphere a, ref BoundingSphere b)
		{
			float distance = Vector3.Distance(ref a.Center, ref b.Center);
			return a.Radius + b.Radius >= distance && a.Radius - b.Radius >= distance;
		}
		public static bool Contains(ref BoundingSphere sphere, ref BoundingBox box)
		{
			if (!Intersects(ref box, ref sphere))
				return false;
			float rad = sphere.Radius * sphere.Radius;
			Vector3 vector;
			vector.X = sphere.Center.X - box.Min.X;
			vector.Y = sphere.Center.Y - box.Max.Y;
			vector.Z = sphere.Center.Z - box.Max.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Max.X;
			vector.Y = sphere.Center.Y - box.Max.Y;
			vector.Z = sphere.Center.Z - box.Max.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Max.X;
			vector.Y = sphere.Center.Y - box.Min.Y;
			vector.Z = sphere.Center.Z - box.Max.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Min.X;
			vector.Y = sphere.Center.Y - box.Min.Y;
			vector.Z = sphere.Center.Z - box.Max.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Min.X;
			vector.Y = sphere.Center.Y - box.Max.Y;
			vector.Z = sphere.Center.Z - box.Min.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Max.X;
			vector.Y = sphere.Center.Y - box.Max.Y;
			vector.Z = sphere.Center.Z - box.Min.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Max.X;
			vector.Y = sphere.Center.Y - box.Min.Y;
			vector.Z = sphere.Center.Z - box.Min.Z;
			if (vector.LengthSquared > rad)
				return false;
			vector.X = sphere.Center.X - box.Min.X;
			vector.Y = sphere.Center.Y - box.Min.Y;
			vector.Z = sphere.Center.Z - box.Min.Z;
			if (vector.LengthSquared > rad)
				return false;
			return true;
		}

		#endregion

		#region Raycast

		public static bool Raycast(ref BoundingBox box, ref Ray3D ray, out float dist)
		{
			const float epsilon = 1e-6f;
			float? tMin = null;
			float? tMax = null;
			dist = 0f;

			if (Math.Abs(ray.Normal.X) < epsilon)
			{
				if (ray.Point.X < box.Min.X || ray.Point.X > box.Max.X)
					return false;
			}
			else
			{
				tMin = (box.Min.X - ray.Point.X) / ray.Normal.X;
				tMax = (box.Max.X - ray.Point.X) / ray.Normal.X;
				if (tMin > tMax)
					Calc.Swap(ref tMin, ref tMax);
			}

			if (Math.Abs(ray.Normal.Y) < epsilon)
			{
				if (ray.Point.Y < box.Min.Y || ray.Point.Y > box.Max.Y)
					return false;
			}
			else
			{
				var tMinY = (box.Min.Y - ray.Point.Y) / ray.Normal.Y;
				var tMaxY = (box.Max.Y - ray.Point.Y) / ray.Normal.Y;
				if (tMinY > tMaxY)
					Calc.Swap(ref tMinY, ref tMaxY);
				if ((tMin.HasValue && tMin > tMaxY) || (tMax.HasValue && tMinY > tMax))
					return false;
				if (!tMin.HasValue || tMinY > tMin)
					tMin = tMinY;
				if (!tMax.HasValue || tMaxY < tMax)
					tMax = tMaxY;
			}

			if (Math.Abs(ray.Normal.Z) < epsilon)
			{
				if (ray.Point.Z < box.Min.Z || ray.Point.Z > box.Max.Z)
					return false;
			}
			else
			{
				var tMinZ = (box.Min.Z - ray.Point.Z) / ray.Normal.Z;
				var tMaxZ = (box.Max.Z - ray.Point.Z) / ray.Normal.Z;
				if (tMinZ > tMaxZ)
					Calc.Swap(ref tMinZ, ref tMaxZ);
				if ((tMin.HasValue && tMin > tMaxZ) || (tMax.HasValue && tMinZ > tMax))
					return false;
				if (!tMin.HasValue || tMinZ > tMin)
					tMin = tMinZ;
				if (!tMax.HasValue || tMaxZ < tMax)
					tMax = tMaxZ;
			}

			if (tMin < 0f || ((tMin.HasValue && tMin < 0f) && tMax > 0f))
				return false;

			dist = tMin.Value;
			return true;
		}
		public static bool Raycast(ref Plane plane, ref Ray3D ray, out float dist)
		{
			float dot = Vector3.Dot(ref ray.Normal, ref plane.Normal);
			if (Math.Abs(dot) < 0.00001f)
			{
				dist = 0f;
				return false;
			}
			dist = (-plane.Distance - Vector3.Dot(plane.Normal, ray.Point)) / dot;
			if (dist <= 0f)
			{
				dist = 0f;
				return false;
			}
			return true;
		}
		public static bool Raycast(ref BoundingSphere sphere, ref Ray3D ray, out float dist)
		{
			dist = 0f;
			var diff = sphere.Center - ray.Point;
			var sqrDist = diff.LengthSquared;
			var radius = sphere.Radius * sphere.Radius;

			if (sqrDist < radius)
				return false;

			float distanceAlongRay = Vector3.Dot(ref ray.Normal, ref diff);
			if (distanceAlongRay < 0f)
				return false;

			dist = radius + distanceAlongRay * distanceAlongRay - sqrDist;

			if (dist < 0f)
			{
				dist = 0f;
				return false;
			}

			dist = distanceAlongRay - (float)Math.Sqrt(dist);
			return true;
		}

		#endregion

		#region Project

		public static void Project(ref BoundingBox box, ref Vector3 point, out Vector3 result)
		{
			Vector3 temp;
			Vector3.Max(ref point, ref box.Min, out temp);
			Vector3.Min(ref temp, ref box.Max, out result);
		}
		public static void Project(ref Plane plane, ref Vector3 point, out Vector3 result)
		{
			float dist = plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.Distance;
			Vector3.Multiply(ref plane.Normal, dist, out result);
			Vector3.Subtract(ref point, ref result, out result);
		}
		public static void Project(ref BoundingSphere sphere, ref Vector3 point, out Vector3 result)
		{
			Vector3.Subtract(ref point, ref sphere.Center, out result);
			Vector3.Normalize(ref result, out result);
			Vector3.Multiply(ref result, sphere.Radius, out result);
			Vector3.Add(ref result, ref sphere.Center, out result);
		}

		#endregion

		#region Distance

		public static float Distance(ref Vector3 a, ref Vector3 b)
		{
			return (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z));
		}
		public static float Distance(ref BoundingBox box, ref Vector3 point)
		{
			float dist = 0f;
			if (point.X < box.Min.X)
				dist += (box.Min.X - point.X) * (box.Min.X - point.X);
			if (point.X > box.Max.X)
				dist += (point.X - box.Max.X) * (point.X - box.Max.X);
			if (point.Y < box.Min.Y)
				dist += (box.Min.Y - point.Y) * (box.Min.Y - point.Y);
			if (point.Y > box.Max.Y)
				dist += (point.Y - box.Max.Y) * (point.Y - box.Max.Y);
			if (point.Z < box.Min.Z)
				dist += (box.Min.Z - point.Z) * (box.Min.Z - point.Z);
			if (point.Z > box.Max.Z)
				dist += (point.Z - box.Max.Z) * (point.Z - box.Max.Z);
			return (float)Math.Sqrt(dist);
		}
		public static float Distance(ref Vector3 point, ref BoundingBox box)
		{
			return Distance(ref box, ref point);
		}
		public static float Distance(ref BoundingBox a, ref BoundingBox b)
		{
			float dist = 0f;
			if (a.Min.X > b.Max.X)
			{
				float delta = b.Max.X - a.Min.X;
				dist += delta * delta;
			}
			else if (b.Min.X > a.Max.X)
			{
				float delta = a.Max.X - b.Min.X;
				dist += delta * delta;
			}
			if (a.Min.Y > b.Max.Y)
			{
				float delta = b.Max.Y - a.Min.Y;
				dist += delta * delta;
			}
			else if (b.Min.Y > a.Max.Y)
			{
				float delta = a.Max.Y - b.Min.Y;
				dist += delta * delta;
			}
			if (a.Min.Z > b.Max.Z)
			{
				float delta = b.Max.Z - a.Min.Z;
				dist += delta * delta;
			}
			else if (b.Min.Z > a.Max.Z)
			{
				float delta = a.Max.Z - b.Min.Z;
				dist += delta * delta;
			}
			return (float)Math.Sqrt(dist);
		}
		public static float Distance(ref BoundingBox box, ref BoundingSphere sphere)
		{
			var dist = Distance(ref box, ref sphere.Center);
			if (dist <= sphere.Radius)
				return 0f;
			return dist - sphere.Radius;
		}
		public static float Distance(ref BoundingSphere sphere, ref BoundingBox box)
		{
			return Distance(ref box, ref sphere);
		}
		public static float Distance(ref BoundingSphere sphere, ref Vector3 point)
		{
			float dist = Vector3.Distance(ref sphere.Center, ref point);
			dist -= sphere.Radius;
			return Math.Max(dist, 0f);
		}
		public static float Distance(ref Vector3 point, ref BoundingSphere sphere)
		{
			return Distance(ref sphere, ref point);
		}
		public static float Distance(ref BoundingSphere a, ref BoundingSphere b)
		{
			float dist = Vector3.Distance(ref a.Center, ref b.Center);
			dist -= a.Radius + b.Radius;
			return Math.Max(dist, 0f);
		}
		public static float Distance(ref Plane plane, ref Vector3 point)
		{
			float dot = Vector3.Dot(ref plane.Normal, ref point);
			return dot - plane.Distance;
		}
		public static float Distance(ref Vector3 point, ref Plane plane)
		{
			return Distance(ref plane, ref point);
		}
		public static float Distance(ref Plane plane, ref BoundingBox box)
		{
			var min = new Vector3(
				plane.Normal.X >= 0f ? box.Min.X : box.Max.X,
				plane.Normal.Y >= 0f ? box.Min.Y : box.Max.Y,
				plane.Normal.Z >= 0f ? box.Min.Z : box.Max.Z
			);
			var max = new Vector3(
				plane.Normal.X >= 0f ? box.Max.X : box.Min.X,
				plane.Normal.Y >= 0f ? box.Max.Y : box.Min.Y,
				plane.Normal.Z >= 0f ? box.Max.Z : box.Min.Z
			);
			float distance = plane.Distance + Vector3.Dot(ref plane.Normal, ref max);
			if (distance > 0f)
				return distance;
			distance = plane.Distance + Vector3.Dot(ref plane.Normal, ref min);
			if (distance < 0f)
				return Math.Abs(distance);
			return 0f;
		}
		public static float Distance(ref Plane plane, ref BoundingSphere sphere)
		{
			float dist = Distance(ref plane, ref sphere.Center);
			if (dist <= sphere.Radius)
				return 0f;
			return dist - sphere.Radius;
		}

		#endregion
	}
}
