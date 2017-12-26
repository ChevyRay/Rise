using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Triangle : IEquatable<Triangle>, IShape
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Vector3 Center
        {
            get { return (A + B + C) / 3f; }
        }

        public Vector2 NormalAB
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(B - A)); }
        }

        public Vector2 NormalBC
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(C - B)); }
        }

        public Vector2 NormalCA
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(A - C)); }
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.Cross(B - A, point - A) > 0f
                && Vector2.Cross(C - B, point - B) > 0f
                && Vector2.Cross(A - C, point - C) > 0f;
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
            return Geom2D.Distance(point, ref this);
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
            return obj is Triangle && Equals((Triangle)obj);
        }
        public bool Equals(ref Triangle other)
        {
            return A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C);
        }
        public bool Equals(Triangle other)
        {
            return Equals(ref other);
        }

        public void GetBounds(out Rectangle result)
        {
            result.X = Calc.Min(A.X, B.X, C.X);
            result.Y = Calc.Min(A.Y, B.Y, C.Y);
            result.W = Calc.Max(A.X, B.X, C.X) - result.X;
            result.H = Calc.Max(A.Y, B.Y, C.Y) - result.Y;
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
                hash = hash * 23 + C.X.GetHashCode();
                hash = hash * 23 + C.Y.GetHashCode();
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

        public override string ToString()
        {
            return string.Format("({0}),({1}),({2})", A, B, C);
        }

        public static bool operator ==(Triangle a, Triangle b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Triangle a, Triangle b)
        {
            return !a.Equals(ref b);
        }
    }
}
