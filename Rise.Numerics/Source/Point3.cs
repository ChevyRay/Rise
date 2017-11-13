using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Point3 : IEquatable<Point3>
    {
        public static readonly Point3 Zero = new Point3(0, 0, 0);

        public int X;
        public int Y;
        public int Z;

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Point3(int x, int y) : this(x, y, 0)
        {
            
        }
        public Point3(int val) : this(val, val, val)
        {

        }

        public override bool Equals(object obj)
        {
            return obj is Point3 && Equals((Point3)obj);
        }
        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X;
                hash = hash * 23 + Y;
                hash = hash * 23 + Z;
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X, Y, Z);
        }

        public static implicit operator Vector3(Point3 p)
        {
            return new Vector3(p.X, p.Y, p.Z);
        }

        public static bool operator ==(Point3 a, Point3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        public static bool operator !=(Point3 a, Point3 b)
        {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }

        public static Point3 operator +(Point3 a, Point3 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
            return a;
        }

        public static Point3 operator -(Point3 a, Point3 b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            a.Z -= b.Z;
            return a;
        }

        public static Point3 operator *(Point3 a, Point3 b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
            a.Z *= b.Z;
            return a;
        }
        public static Point3 operator *(Point3 p, int n)
        {
            p.X *= n;
            p.Y *= n;
            p.Z *= n;
            return p;
        }
        public static Point3 operator *(int n, Point3 p)
        {
            p.X *= n;
            p.Y *= n;
            p.Z *= n;
            return p;
        }

        public static Point3 operator /(Point3 a, Point3 b)
        {
            a.X /= b.X;
            a.Y /= b.Y;
            a.Z /= b.Z;
            return a;
        }
        public static Point3 operator /(Point3 p, int n)
        {
            p.X /= n;
            p.Y /= n;
            p.Z /= n;
            return p;
        }
        public static Point3 operator /(int n, Point3 p)
        {
            p.X /= n;
            p.Y /= n;
            p.Z /= n;
            return p;
        }
    }
}
