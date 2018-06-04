using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RectangleI : IEquatable<RectangleI>, ICopyable<RectangleI>
    {
        public static readonly RectangleI Empty = new RectangleI();

        public int X;
        public int Y;
        public int W;
        public int H;

        public RectangleI(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public RectangleI(int w, int h) : this(0, 0, w, h)
        {
            
        }

        public int Right
        {
            get { return X + W; }
        }

        public int Bottom
        {
            get { return Y + H; }
        }

        public int MinX
        {
            get { return Math.Min(X, X + W); }
        }

        public int MinY
        {
            get { return Math.Min(Y, Y + H); }
        }

        public int MaxX
        {
            get { return Math.Max(X, X + W); }
        }

        public int MaxY
        {
            get { return Math.Max(Y, Y + H); }
        }

        public int AbsW
        {
            get { return Math.Abs(W); }
        }

        public int AbsH
        {
            get { return Math.Abs(H); }
        }

        public int CenterX
        {
            get { return X + W / 2; }
        }

        public int CenterY
        {
            get { return Y + H / 2; }
        }

        public bool IsEmpty
        {
            get { return W == 0f && H == 0f; }
        }

        public bool IsRegular
        {
            get { return W >= 0f && H >= 0f; }
        }

        public RectangleI Regular
        {
            get
            {
                var copy = this;
                if (copy.W < 0)
                {
                    copy.X += copy.W;
                    copy.W = -copy.W;
                }
                if (copy.H < 0)
                {
                    copy.Y += copy.H;
                    copy.H = -copy.H;
                }
                return copy;
            }
        }

        public int Area
        {
            get { return Math.Abs(W * H); }
        }

        public Point2 TopLeft
        {
            get { return new Point2(X, Y); }
        }

        public Point2 BottomLeft
        {
            get { return new Point2(X, Bottom); }
        }

        public Point2 BottomRight
        {
            get { return new Point2(Right, Bottom); }
        }

        public Point2 TopRight
        {
            get { return new Point2(Right, Y); }
        }

        public Point2 Center
        {
            get { return new Point2(CenterX, CenterY); }
        }

        public Point2 TopCenter
        {
            get { return new Point2(CenterX, Y); }
        }

        public Point2 BottomCenter
        {
            get { return new Point2(CenterX, Y + H); }
        }

        public Point2 LeftCenter
        {
            get { return new Point2(X, CenterY); }
        }

        public Point2 RightCenter
        {
            get { return new Point2(X + W, CenterY); }
        }

        public override bool Equals(object obj)
        {
            return obj is RectangleI && Equals((RectangleI)obj);
        }
        public bool Equals(RectangleI other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref RectangleI other)
        {
            return X == other.X && Y == other.Y && W == other.W && H == other.H;
        }

        public bool Contains(Point2 p)
        {
            return p.X >= X && p.Y >= Y && p.X < Right && p.Y < Bottom;
        }
        public bool Contains(ref RectangleI rect)
        {
            return rect.X >= X && rect.Y >= Y && rect.Right <= Right && rect.Bottom <= Bottom;
        }
        public bool Contains(RectangleI rect)
        {
            return Contains(ref rect);
        }

        public void CopyTo(out RectangleI other)
        {
            other.X = X;
            other.Y = Y;
            other.W = W;
            other.H = H;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X;
                hash = hash * 23 + Y;
                hash = hash * 23 + W;
                hash = hash * 23 + H;
                return hash;
            }
        }

        public RectangleI Inflated(int x, int y)
        {
            var rect = this;
            rect.X -= x;
            rect.W += x * 2;
            rect.Y -= y;
            rect.H += y * 2;
            return rect;
        }
        public RectangleI Inflated(int amount)
        {
            return Inflated(amount, amount);
        }

        public bool Intersects(ref RectangleI other)
        {
            return X < other.X + other.W && Y < other.Y + other.H && X + W > other.X && Y + H > other.Y;
        }
        public bool Intersects(RectangleI other)
        {
            return Intersects(ref other);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", X, Y, W, H);
        }

        public static void ScaleToFit(ref RectangleI outer, ref RectangleI inner, out RectangleI result)
        {
            float s = Math.Min(outer.W / (float)inner.W, outer.H / (float)inner.H);
            result.W = (int)(inner.W * s);
            result.H = (int)(inner.H * s);
            result.X = (outer.W - result.W) / 2;
            result.Y = (outer.H - result.H) / 2;
        }
        public static RectangleI ScaleToFit(RectangleI outer, RectangleI inner)
        {
            RectangleI r;
            ScaleToFit(ref outer, ref inner, out r);
            return r;
        }

        public static void Conflate(ref RectangleI a, ref RectangleI b, out RectangleI result)
        {
            int x = Math.Min(a.MinX, b.MinX);
            int y = Math.Min(a.MinY, b.MinY);
            int w = Math.Max(a.MaxX, b.MaxX) - x;
            int h = Math.Max(a.MaxY, b.MaxY) - y;
            result.X = x;
            result.Y = y;
            result.W = w;
            result.H = h;
        }
        public static RectangleI Conflate(RectangleI a, RectangleI b)
        {
            Conflate(ref a, ref b, out a);
            return a;
        }

        public static implicit operator Rectangle(RectangleI r)
        {
            return new Rectangle(r.X, r.Y, r.W, r.H);
        }

        public static bool operator ==(RectangleI a, RectangleI b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(RectangleI a, RectangleI b)
        {
            return !a.Equals(ref b);
        }

        public static RectangleI operator *(RectangleI a, int b)
        {
            return new RectangleI(a.X * b, a.Y * b, a.W * b, a.H * b);
        }

        public static RectangleI operator /(RectangleI a, int b)
        {
            return new RectangleI(a.X / b, a.Y / b, a.W / b, a.H / b);
        }
    }
}
