using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Rectangle : IEquatable<Rectangle>, IShape
    {
        public static readonly Rectangle Empty = new Rectangle();

        public float X;
        public float Y;
        public float W;
        public float H;

        public Rectangle(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public Rectangle(float w, float h)
        {
            X = 0f;
            Y = 0f;
            W = w;
            H = h;
        }

        public float Right
        {
            get { return X + W; }
        }

        public float Bottom
        {
            get { return Y + H; }
        }

        public float MinX
        {
            get { return Math.Min(X, X + W); }
        }

        public float MinY
        {
            get { return Math.Min(Y, Y + H); }
        }

        public float MaxX
        {
            get { return Math.Max(X, X + W); }
        }

        public float MaxY
        {
            get { return Math.Max(Y, Y + H); }
        }

        public float AbsW
        {
            get { return Math.Abs(W); }
        }

        public float AbsH
        {
            get { return Math.Abs(H); }
        }

        public float CenterX
        {
            get { return X + W * 0.5f; }
        }

        public float CenterY
        {
            get { return Y + H * 0.5f; }
        }

        public bool IsEmpty
        {
            get { return W == 0f && H == 0f; }
        }

        public bool IsRegular
        {
            get { return W >= 0f && H >= 0f; }
        }

        public Rectangle Regular
        {
            get
            {
                var copy = this;
                if (copy.W < 0f)
                {
                    copy.X += copy.W;
                    copy.W = -copy.W;
                }
                if (copy.H < 0f)
                {
                    copy.Y += copy.H;
                    copy.H = -copy.H;
                }
                return copy;
            }
        }

        public float Area
        {
            get { return Math.Abs(W * H); }
        }

        public Vector2 TopLeft
        {
            get { return new Vector2(X, Y); }
        }

        public Vector2 BottomLeft
        {
            get { return new Vector2(X, Bottom); }
        }

        public Vector2 BottomRight
        {
            get { return new Vector2(Right, Bottom); }
        }

        public Vector2 TopRight
        {
            get { return new Vector2(Right, Y); }
        }

        public Vector2 Center
        {
            get { return new Vector2(CenterX, CenterY); }
        }

        public Vector2 TopCenter
        {
            get { return new Vector2(CenterX, Y); }
        }

        public Vector2 BottomCenter
        {
            get { return new Vector2(CenterX, Y + H); }
        }

        public Vector2 LeftCenter
        {
            get { return new Vector2(X, CenterY); }
        }

        public Vector2 RightCenter
        {
            get { return new Vector2(X + W, CenterY); }
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle && Equals((Rectangle)obj);
        }
        public bool Equals(Rectangle other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref Rectangle other)
        {
            return X == other.X && Y == other.Y && W == other.W && H == other.H;
        }

        public bool Contains(Vector2 point)
        {
            return point.X > X && point.Y > Y && point.X < Right && point.Y < Bottom;
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

        public void GetBounds(out Rectangle result)
        {
            result.X = X;
            result.Y = Y;
            result.W = W;
            result.H = H;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + W.GetHashCode();
                hash = hash * 23 + H.GetHashCode();
                return hash;
            }
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
            return string.Format("{0},{1},{2},{3}", X, Y, W, H);
        }

        public int CompareArea(ref Rectangle a, ref Rectangle b)
        {
            return Math.Sign(a.Area - b.Area);
        }
        public int CompareArea(Rectangle a, Rectangle b)
        {
            return CompareArea(ref a, ref b);
        }

        public static Rectangle Box(float centerX, float centerY, float size)
        {
            return new Rectangle(centerX - size * 0.5f, centerY - size * 0.5f, size, size);
        }
        public static Rectangle Box(Vector2 center, float size)
        {
            return new Rectangle(center.X - size * 0.5f, center.Y - size * 0.5f, size, size);
        }
        public static Rectangle Box(float size)
        {
            return new Rectangle(size * -0.5f, size * -0.5f, size, size);
        }
        public static Rectangle Box(float centerX, float centerY, float w, float h)
        {
            return new Rectangle(centerX - w * 0.5f, centerY - h * 0.5f, w, h);
        }
        public static Rectangle Box(Vector2 center, float w, float h)
        {
            return new Rectangle(center.X - w * 0.5f, center.Y - h * 0.5f, w, h);
        }
        public static Rectangle Box(float w, float h)
        {
            return new Rectangle(w * -0.5f, h * -0.5f, w, h);
        }

        public static int ComparePerimeter(ref Rectangle a, ref Rectangle b)
        {
            return Math.Sign(Math.Abs(a.W + a.H) - Math.Abs(b.W + b.H));
        }
        public static int ComparePerimeter(Rectangle a, Rectangle b)
        {
            return ComparePerimeter(ref a, ref b);
        }

        public static void Conflate(ref Rectangle a, ref Rectangle b, out Rectangle result)
        {
            result.X = Math.Min(a.MinX, b.MinX);
            result.Y = Math.Min(a.MinY, b.MinY);
            result.W = Math.Max(a.MaxX, b.MaxX) - result.X;
            result.H = Math.Max(a.MaxY, b.MaxY) - result.Y;
        }
        public static Rectangle Conflate(Rectangle a, Rectangle b)
        {
            Rectangle r;
            Conflate(ref a, ref b, out r);
            return r;
        }

        public static Rectangle FromBounds(float minX, float minY, float maxX, float maxY)
        {
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
        public static Rectangle FromBounds(Vector2 min, Vector2 max)
        {
            if (min.X > max.X)
                Calc.Swap(ref min.X, ref max.X);
            if (min.Y > max.Y)
                Calc.Swap(ref min.Y, ref max.Y);
            return new Rectangle(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        public Rectangle Inflated(float x, float y)
        {
            var rect = this;
            rect.X -= x;
            rect.W += x * 2f;
            rect.Y -= y;
            rect.H += y * 2f;
            return rect;
        }
        public Rectangle Inflated(float amount)
        {
            return Inflated(amount, amount);
        }

        public static implicit operator Quad(Rectangle r)
        {
            return new Quad(r.TopLeft, r.TopRight, r.BottomRight, r.BottomLeft);
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(ref b);
        }
    }
}

