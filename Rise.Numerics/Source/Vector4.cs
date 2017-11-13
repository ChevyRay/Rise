using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector4 : IEquatable<Vector4>, ICopyable<Vector4>
    {
        public static readonly Vector4 Zero = new Vector4(0f, 0f, 0f, 0f);

        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public float LengthSquared
        {
            get { return X * X + Y * Y + Z * Z + W * W; }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(LengthSquared); }
        }

        public Vector4 Normalized
        {
            get { return Normalize(this); }
        }

        public void CopyTo(out Vector4 other)
        {
            other.X = X;
            other.Y = Y;
            other.Z = Z;
            other.W = W;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector4 && Equals((Vector4)obj);
        }
        public bool Equals(Vector4 other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref Vector4 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                hash = hash * 23 + W.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", X, Y, Z, W);
        }

        public static Vector4 Parse(string str)
        {
            var split = str.Split(',');
            if (split.Length != 4)
                throw new Exception("Failed to parse Vector4 from string: '" + str + "'");
            return new Vector4(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2]),
                float.Parse(split[3])
            );
        }

        public static void Normalize(ref Vector4 v, out Vector4 result)
        {
            float len = v.Length;
            if (len > 0f)
            {
                result.X = v.X / len;
                result.Y = v.Y / len;
                result.Z = v.Z / len;
                result.W = v.W / len;
            }
            else
                result = v;
        }
        public static Vector4 Normalize(Vector4 v)
        {
            Normalize(ref v, out v);
            return v;
        }
        public static void Normalize(ref Vector4 v, float length, out Vector4 result)
        {
            float len = v.Length;
            if (len > 0f)
            {
                length /= len;
                result.X = v.X * length;
                result.Y = v.Y * length;
                result.Z = v.Z * length;
                result.W = v.W * length;
            }
            else
                result = v;
        }
        public static Vector4 Normalize(Vector4 v, float length)
        {
            Normalize(ref v, length, out v);
            return v;
        }

        public static float DistanceSquared(ref Vector4 a, ref Vector4 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z) + (a.W - b.W) * (a.W - b.W);
        }
        public static float DistanceSquared(Vector4 a, Vector4 b)
        {
            return DistanceSquared(ref a, ref b);
        }

        public static float Distance(ref Vector4 a, ref Vector4 b)
        {
            return (float)Math.Sqrt(DistanceSquared(ref a, ref b));
        }
        public static float Distance(Vector4 a, Vector4 b)
        {
            return (float)Math.Sqrt(DistanceSquared(ref a, ref b));
        }

        public static bool operator ==(Vector4 a, Vector4 b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Vector4 a, Vector4 b)
        {
            return !a.Equals(ref b);
        }

        public static Vector4 operator *(Vector4 a, float b)
        {
            a.X *= b;
            a.Y *= b;
            a.Z *= b;
            a.W *= b;
            return a;
        }
        public static Vector4 operator *(float b, Vector4 a)
        {
            a.X *= b;
            a.Y *= b;
            a.Z *= b;
            a.W *= b;
            return a;
        }
        public static Vector4 operator *(Vector4 a, Vector4 b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
            a.Z *= b.Z;
            a.W *= b.W;
            return a;
        }

        public static Vector4 operator /(Vector4 a, float b)
        {
            a.X /= b;
            a.Y /= b;
            a.Z /= b;
            a.W /= b;
            return a;
        }
        public static Vector4 operator /(Vector4 a, Vector4 b)
        {
            a.X /= b.X;
            a.Y /= b.Y;
            a.Z /= b.Z;
            a.W /= b.W;
            return a;
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            a.Z += b.Z;
            a.W += b.W;
            return a;
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            a.Z -= b.Z;
            a.W -= b.W;
            return a;
        }
        public static Vector4 operator -(Vector4 a)
        {
            a.X = -a.X;
            a.Y = -a.Y;
            a.Z = -a.Z;
            a.W = -a.W;
            return a;
        }
    }
}

