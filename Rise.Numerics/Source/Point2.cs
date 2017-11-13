using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Point2 : IEquatable<Point2>
    {
        public static readonly Point2 Zero = new Point2(0, 0);

        public int X;
        public int Y;

        public Point2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point2(int val) : this(val, val)
        {
            
        }

        public override bool Equals(object obj)
        {
            return obj is Point2 && Equals((Point2)obj);
        }
        public bool Equals(Point2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X;
                hash = hash * 23 + Y;
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }

        public static implicit operator Vector2(Point2 p)
        {
            return new Vector2(p.X, p.Y);
        }

        public static bool operator ==(Point2 a, Point2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Point2 a, Point2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Point2 operator +(Point2 a, Point2 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            return a;
        }

        public static Point2 operator -(Point2 a, Point2 b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            return a;
        }

        public static Point2 operator *(Point2 a, Point2 b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
            return a;
        }
        public static Point2 operator *(Point2 p, int n)
        {
            p.X *= n;
            p.Y *= n;
            return p;
        }
        public static Point2 operator *(int n, Point2 p)
        {
            p.X *= n;
            p.Y *= n;
            return p;
        }

        public static Point2 operator /(Point2 a, Point2 b)
        {
            a.X /= b.X;
            a.Y /= b.Y;
            return a;
        }
        public static Point2 operator /(Point2 p, int n)
        {
            p.X /= n;
            p.Y /= n;
            return p;
        }
        public static Point2 operator /(int n, Point2 p)
        {
            p.X /= n;
            p.Y /= n;
            return p;
        }
    }
}
