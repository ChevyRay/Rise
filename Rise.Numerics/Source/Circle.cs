using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Circle : IEquatable<Circle>, IShape
    {
        public Vector2 Center;
        public float Radius;

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
        public Circle(float x, float y, float radius)
        {
            Center.X = x;
            Center.Y = y;
            Radius = radius;
        }
        public Circle(float radius) : this(0f, 0f, radius)
        {
            
        }

        public float Left
        {
            get { return Center.X - Radius; }
        }

        public float Right
        {
            get { return Center.X + Radius; }
        }

        public float Top
        {
            get { return Center.Y - Radius; }
        }

        public float Bottom
        {
            get { return Center.Y + Radius; }
        }

        public float Area
        {
            get { return Calc.PI * Radius * Radius; }
        }

        public float Circumference
        {
            get { return Calc.PI * Radius * 2f; }
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.DistanceSquared(Center, point) < Radius * Radius;
        }
        public bool Contains(ref Circle circle)
        {
            return Geom2D.Contains(ref this, ref circle);
        }
        public bool Contains(ref Rectangle rect)
        {
            return Geom2D.Contains(ref this, ref rect);
        }
        public bool Contains(ref Triangle tri)
        {
            return Geom2D.Contains(ref this, ref tri);
        }
        public bool Contains(ref Quad quad)
        {
            return Geom2D.Contains(ref this, ref quad);
        }
        public bool Contains(Polygon poly)
        {
            return Geom2D.Contains(ref this, poly);
        }

        public float DistanceTo(Vector2 point)
        {
            return Vector2.Distance(Center, point) - Radius;
        }
        public float DistanceTo(ref Circle circle)
        {
            return Geom2D.Distance(ref this, ref circle);
        }
        public float DistanceTo(ref Rectangle rect)
        {
            return Geom2D.Distance(ref this, ref rect);
        }
        public float DistanceTo(ref Triangle tri)
        {
            return Geom2D.Distance(ref this, ref tri);
        }
        public float DistanceTo(ref Quad quad)
        {
            return Geom2D.Distance(ref this, ref quad);
        }
        public float DistanceTo(Polygon poly)
        {
            return Geom2D.Distance(ref this, poly);
        }

        public override bool Equals(object obj)
        {
            return obj is Circle && base.Equals((Circle)obj);
        }
        public bool Equals(ref Circle other)
        {
            return Center.Equals(other.Center) && Radius == other.Radius;
        }
        public bool Equals(Circle other)
        {
            return Equals(ref other);
        }

        public void GetBounds(out Rectangle result)
        {
            result.X = Center.X - Radius;
            result.Y = Center.Y - Radius;
            result.W = result.H = Radius * 2f;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Center.X.GetHashCode();
                hash = hash * 23 + Center.Y.GetHashCode();
                hash = hash * 23 + Radius.GetHashCode();
                return hash;
            }
        }

        public bool Intersects(ref Circle circle)
        {
            return Geom2D.Intersects(ref this, ref circle);
        }
        public bool Intersects(ref Rectangle rect)
        {
            return Geom2D.Intersects(ref this, ref rect);
        }
        public bool Intersects(ref Triangle tri)
        {
            return Geom2D.Intersects(ref this, ref tri);
        }
        public bool Intersects(ref Quad quad)
        {
            return Geom2D.Intersects(ref this, ref quad);
        }
        public bool Intersects(Polygon poly)
        {
            return Geom2D.Intersects(ref this, poly);
        }

        public bool Intersects(Vector2 point, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, point, out pushOut);
        }
        public bool Intersects(ref Circle circle, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, ref circle, out pushOut);
        }
        public bool Intersects(ref Rectangle rect, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, ref rect, out pushOut);
        }
        public bool Intersects(ref Triangle tri, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, ref tri, out pushOut);
        }
        public bool Intersects(ref Quad quad, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, ref quad, out pushOut);
        }
        public bool Intersects(Polygon poly, out Vector2 pushOut)
        {
            return Geom2D.Intersects(ref this, poly, out pushOut);
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            Geom2D.Project(ref this, axis, out min, out max);
        }
        public Vector2 Project(Vector2 point)
        {
            return Geom2D.Project(ref this, point);
        }

        public bool Raycast(ref Ray2D ray)
        {
            return Geom2D.Raycast(ref this, ref ray);
        }
        public bool Raycast(ref Ray2D ray, out RayHit2D hit)
        {
            return Geom2D.Raycast(ref this, ref ray, out hit);
        }
        public bool Raycast(ref Ray2D ray, out float dist)
        {
            return Geom2D.Raycast(ref this, ref ray, out dist);
        }
    }
}
