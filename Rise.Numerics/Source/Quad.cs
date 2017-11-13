using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quad : IEquatable<Quad>, IShape
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        public Vector2 D;

        public Quad(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
        public Quad(ref Rectangle rect)
            : this(rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft)
        {

        }
        public Quad(Rectangle rect)
            : this(ref rect)
        {

        }
        public Quad(float x, float y, float w, float h)
            : this(new Vector2(x, y), new Vector2(x + w, y), new Vector2(x + w, y + h), new Vector2(x, y + h))
        {

        }
        public Quad(float w, float h)
        : this(-w * 0.5f, -h * 0.5f, w, h)
        {
            
        }

        public Vector2 NormalAB
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(B - A)); }
        }

        public Vector2 NormalBC
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(C - B)); }
        }

        public Vector2 NormalCD
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(D - C)); }
        }

        public Vector2 NormalDA
        {
            get { return Vector2.TurnLeft(Vector2.Normalize(A - D)); }
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle result;
                GetBounds(out result);
                return result;
            }
        }

        public Vector2 Centroid
        {
            get
            {
                var a = Vector2.Lerp(A, C, 0.5f);
                var b = Vector2.Lerp(B, D, 0.5f);
                return Vector2.Lerp(a, b, 0.5f);
            }
        }

        public bool IsConvex
        {
            get
            {
                return Vector2.Cross(B - A, C - B) > 0f
                    && Vector2.Cross(C - B, D - C) > 0f
                    && Vector2.Cross(D - C, A - D) > 0f
                    && Vector2.Cross(A - D, B - A) > 0f;
            }
        }

        public bool Contains(Vector2 point)
        {
            return Vector2.Cross(B - A, point - A) > 0f
                && Vector2.Cross(C - B, point - B) > 0f
                && Vector2.Cross(D - C, point - C) > 0f
                && Vector2.Cross(A - D, point - D) > 0f;
        }
        public bool Contains(ref Circle circle)
        {
            return Geom.Contains(ref this, ref circle);
        }
        public bool Contains(ref Rectangle rect)
        {
            return Geom.Contains(ref this, ref rect);
        }
        public bool Contains(ref Triangle tri)
        {
            return Geom.Contains(ref this, ref tri);
        }
        public bool Contains(ref Quad quad)
        {
            return Geom.Contains(ref this, ref quad);
        }
        public bool Contains(Polygon poly)
        {
            return Geom.Contains(ref this, poly);
        }

        public float DistanceTo(Vector2 point)
        {
            return Geom.Distance(point, ref this);
        }
        public float DistanceTo(ref Circle circle)
        {
            return Geom.Distance(ref this, ref circle);
        }
        public float DistanceTo(ref Rectangle rect)
        {
            return Geom.Distance(ref this, ref rect);
        }
        public float DistanceTo(ref Triangle tri)
        {
            return Geom.Distance(ref this, ref tri);
        }
        public float DistanceTo(ref Quad quad)
        {
            return Geom.Distance(ref this, ref quad);
        }
        public float DistanceTo(Polygon poly)
        {
            return Geom.Distance(ref this, poly);
        }

        public override bool Equals(object obj)
        {
            return obj is Quad && Equals((Quad)obj);
        }
        public bool Equals(ref Quad other)
        {
            return A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C) && D.Equals(other.D);
        }
        public bool Equals(Quad other)
        {
            return Equals(ref other);
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
                hash = hash * 23 + D.X.GetHashCode();
                hash = hash * 23 + D.Y.GetHashCode();
                return hash;
            }
        }

        public void GetBounds(out Rectangle result)
        {
            result.X = Calc.Min(A.X, B.X, C.X, D.X);
            result.Y = Calc.Min(A.Y, B.Y, C.Y, D.Y);
            result.W = Calc.Max(A.X, B.X, C.X, D.X) - result.X;
            result.H = Calc.Max(A.Y, B.Y, C.Y, D.Y) - result.Y;
        }

        public bool Intersects(ref Circle circle)
        {
            return Geom.Intersects(ref this, ref circle);
        }
        public bool Intersects(ref Rectangle rect)
        {
            return Geom.Intersects(ref this, ref rect);
        }
        public bool Intersects(ref Triangle tri)
        {
            return Geom.Intersects(ref this, ref tri);
        }
        public bool Intersects(ref Quad quad)
        {
            return Geom.Intersects(ref this, ref quad);
        }
        public bool Intersects(Polygon poly)
        {
            return Geom.Intersects(ref this, poly);
        }

        public bool Intersects(Vector2 point, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, point, out pushOut);
        }
        public bool Intersects(ref Circle circle, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, ref circle, out pushOut);
        }
        public bool Intersects(ref Rectangle rect, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, ref rect, out pushOut);
        }
        public bool Intersects(ref Triangle tri, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, ref tri, out pushOut);
        }
        public bool Intersects(ref Quad quad, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, ref quad, out pushOut);
        }
        public bool Intersects(Polygon poly, out Vector2 pushOut)
        {
            return Geom.Intersects(ref this, poly, out pushOut);
        }

        public static void Line(Vector2 start, Vector2 end, float width, out Quad q)
        {
            var off = Vector2.TurnLeft(Vector2.Normalize(end - start)) * (width * 0.5f);
            q.A = start + off;
            q.B = end + off;
            q.C = end - off;
            q.D = start - off;
        }
        public static Quad Line(Vector2 start, Vector2 end, float width)
        {
            Quad q;
            Line(start, end, width, out q);
            return q;
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            Geom.Project(ref this, axis, out min, out max);
        }
        public Vector2 Project(Vector2 point)
        {
            return Geom.Project(ref this, point);
        }

        public bool Raycast(ref Ray ray)
        {
            return Geom.Raycast(ref this, ref ray);
        }
        public bool Raycast(ref Ray ray, out RayHit hit)
        {
            return Geom.Raycast(ref this, ref ray, out hit);
        }
        public bool Raycast(ref Ray ray, out float dist)
        {
            return Geom.Raycast(ref this, ref ray, out dist);
        }

        public override string ToString()
        {
            return string.Format("({0}),({1}),({2}),({3})", A, B, C, D);
        }

        public static bool operator ==(Quad a, Quad b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Quad a, Quad b)
        {
            return !a.Equals(ref b);
        }
    }
}

