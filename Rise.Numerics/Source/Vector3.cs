using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector3 : IEquatable<Vector3>
    {
		public static readonly Vector3 Zero = new Vector3(0f, 0f, 0f);
        public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);
        public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);
        public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);
        public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);
        public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);
        public static readonly Vector3 Back = new Vector3(0f, 0f, -1f);
		
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3(float x, float y) : this(x, y, 1f)
        {
            
        }
        public Vector3(Vector2 pos, float z) : this(pos.X, pos.Y, 0f)
        {
            
        }
        public Vector3(Vector2 pos) : this(pos.X, pos.Y, 0f)
        {
            
        }

        public Vector2 XY
        {
            get { return new Vector2(X, Y); }
        }

        public Vector2 XZ
        {
            get { return new Vector2(X, Z); }
        }

        public Vector2 YZ
        {
            get { return new Vector2(Y, Z); }
        }

        public float LengthSquared
        {
            get { return X * X + Y * Y + Z * Z; }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public Vector3 Normalized
        {
            get { return Normalize(this); }
        }

        public void CopyTo(out Vector3 other)
        {
            other.X = X;
            other.Y = Y;
            other.Z = Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3 && Equals((Vector3)obj);
        }
        public bool Equals(Vector3 other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref Vector3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X, Y, Z);
        }

        public static void Add(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X + b.X;
            result.Y = a.Y + b.Y;
            result.Z = a.Z + b.Z;
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            throw new NotImplementedException();
        }

        public static void Approach(ref Vector3 a, ref Vector3 b, float amount, out Vector3 result)
        {
            if (a != b)
            {
                var move = b - a;
                if (move.Length > amount)
                {
                    Normalize(ref move, amount, out move);
                    result = a + move;
                }
            }
            result = b;
        }
        public static Vector3 Approach(Vector3 a, Vector3 b, float amount)
        {
            Approach(ref a, ref b, amount, out a);
            return a;
        }

        public static bool Approx(ref Vector3 a, ref Vector3 b)
        {
            return Calc.Approx(a.X, b.X) && Calc.Approx(a.Y, b.Y) && Calc.Approx(a.Z, b.Z);
        }
        public static bool Approx(Vector3 a, Vector3 b)
        {
            return Approx(ref a, ref b);
        }

        public static void Bezier(ref Vector3 a, ref Vector3 b, ref Vector3 c, float t, out Vector3 result)
        {
            result.X = Calc.Bezier(a.X, b.X, c.X, t);
            result.Y = Calc.Bezier(a.Y, b.Y, c.Y, t);
            result.Z = Calc.Bezier(a.Z, b.Z, c.Z, t);
        }
        public static void Bezier(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, float t, out Vector3 result)
        {
            result.X = Calc.Bezier(a.X, b.X, c.X, d.X, t);
            result.Y = Calc.Bezier(a.Y, b.Y, c.Y, d.Y, t);
            result.Z = Calc.Bezier(a.Z, b.Z, c.Z, d.Z, t);
        }
        public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            Bezier(ref a, ref b, ref c, t, out a);
            return a;
        }
        public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            Bezier(ref a, ref b, ref c, ref d, t, out a);
            return a;
        }

        public static void CatmullRom(ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 d, float t, out Vector3 result)
        {
            result.X = Calc.CatmullRom(a.X, b.X, c.X, d.X, t);
            result.Y = Calc.CatmullRom(a.Y, b.Y, c.Y, d.Y, t);
            result.Z = Calc.CatmullRom(a.Z, b.Z, c.Z, d.Z, t);
        }
        public static Vector3 CatmullRom(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            CatmullRom(ref a, ref b, ref c, ref d, t, out a);
            return a;
        }

        public static void Ceil(ref Vector3 v, out Vector3 result)
        {
            result.X = Calc.Ceil(v.X);
            result.Y = Calc.Ceil(v.Y);
            result.Z = Calc.Ceil(v.Z);
        }
        public static Vector3 Ceil(Vector3 v)
        {
            Ceil(ref v, out v);
            return v;
        }

        public static void Clamp(ref Vector3 a, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            result.X = Calc.Clamp(a.X, min.X, max.X);
            result.Y = Calc.Clamp(a.Y, min.Y, max.Y);
            result.Z = Calc.Clamp(a.Z, min.Z, max.Z);
        }
        public static Vector3 Clamp(Vector3 a, Vector3 min, Vector3 max)
        {
            Clamp(ref a, ref min, ref max, out a);
            return a;
        }

        public static void ClampLength(ref Vector3 v, float max, out Vector3 result)
        {
            float len = v.LengthSquared;
            if (len > max * max)
                Normalize(ref v, max, out result);
            else
                result = v;
        }
        public static void ClampLength(ref Vector3 v, float min, float max, out Vector3 result)
        {
            if (min > max)
                Calc.Swap(ref min, ref max);
            float len = v.LengthSquared;
            if (len > max * max)
                Normalize(ref v, max, out result);
            else if (len < min * min)
                Normalize(ref v, min, out result);
            else
                result = v;
        }
        public static Vector3 ClampLength(Vector3 v, float max)
        {
            ClampLength(ref v, max, out v);
            return v;
        }
        public static Vector3 ClampLength(Vector3 v, float min, float max)
        {
            ClampLength(ref v, min, max, out v);
            return v;
        }

        public static void Cross(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.Y * b.Z - a.Z * b.Y;
            result.Y = a.Z * b.X - a.X * b.Z;
            result.Z = a.X * b.Y - a.Y * b.X;
        }
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            Vector3 result;
            Cross(ref a, ref b, out result);
            return result;
        }

        public static float Distance(ref Vector3 a, ref Vector3 b)
        {
            return (float)Math.Sqrt(DistanceSquared(ref a, ref b));
        }
        public static float Distance(Vector3 a, Vector3 b)
        {
            return (float)Math.Sqrt(DistanceSquared(ref a, ref b));
        }

        public static float DistanceSquared(ref Vector3 a, ref Vector3 b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y) + (a.Z - b.Z) * (a.Z - b.Z);
        }
        public static float DistanceSquared(Vector3 a, Vector3 b)
        {
            return DistanceSquared(ref a, ref b);
        }

        public static void Divide(ref Vector3 a, float b, out Vector3 result)
        {
            result.X = a.X / b;
            result.Y = a.Y / b;
            result.Z = a.Z / b;
        }
        public static void Divide(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X / b.X;
            result.Y = a.Y / b.Y;
            result.Z = a.Z / b.Z;
        }

        public static float Dot(ref Vector3 a, ref Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static float Dot(Vector3 a, Vector3 b)
        {
            return Dot(ref a, ref b);
        }

        public static void Floor(ref Vector3 v, out Vector3 result)
        {
            result.X = Calc.Floor(v.X);
            result.Y = Calc.Floor(v.Y);
            result.Z = Calc.Floor(v.Z);
        }
        public static Vector3 Floor(Vector3 v)
        {
            Floor(ref v, out v);
            return v;
        }

        public static void Hermite(ref Vector3 a, float ta, ref Vector3 b, float tb, float t, out Vector3 result)
        {
            result.X = Calc.Hermite(a.X, ta, b.X, tb, t);
            result.Y = Calc.Hermite(a.Y, ta, b.Y, tb, t);
            result.Z = Calc.Hermite(a.Z, ta, b.Z, tb, t);
        }
        public static void Hermite(ref Vector3 a, ref Vector3 ta, ref Vector3 b, ref Vector3 tb, float t, out Vector3 result)
        {
            result.X = Calc.Hermite(a.X, ta.X, b.X, tb.X, t);
            result.Y = Calc.Hermite(a.Y, ta.Y, b.Y, tb.Y, t);
            result.Z = Calc.Hermite(a.Z, ta.Z, b.Z, tb.Z, t);
        }
        public static Vector3 Hermite(Vector3 a, float ta, Vector3 b, float tb, float t)
        {
            Hermite(ref a, ta, ref b, tb, t, out a);
            return a;
        }
        public static Vector3 Hermite(Vector3 a, Vector3 ta, Vector3 b, Vector3 tb, float t)
        {
            Hermite(ref a, ref ta, ref b, ref tb, t, out a);
            return a;
        }

        public static void Lerp(ref Vector3 a, ref Vector3 b, float t, out Vector3 result)
        {
            result.X = a.X + (b.X - a.X) * t;
            result.Y = a.Y + (b.Y - a.Y) * t;
            result.Z = a.Z + (b.Z - a.Z) * t;
        }
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            Lerp(ref a, ref b, t, out a);
            return a;
        }

        public static void Max(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = Math.Max(a.X, b.X);
            result.Y = Math.Max(a.Y, b.Y);
            result.Z = Math.Max(a.Z, b.Z);
        }
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            Max(ref a, ref b, out a);
            return a;
        }

        public static void Min(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = Math.Min(a.X, b.X);
            result.Y = Math.Min(a.Y, b.Y);
            result.Z = Math.Min(a.Z, b.Z);
        }
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            Min(ref a, ref b, out a);
            return a;
        }

        public static void Multiply(ref Vector3 a, float b, out Vector3 result)
        {
            result.X = a.X * b;
            result.Y = a.Y * b;
            result.Z = a.Z * b;
        }
        public static void Multiply(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X * b.X;
            result.Y = a.Y * b.Y;
            result.Z = a.Z * b.Z;
        }

        public static void Negate(ref Vector3 v, out Vector3 result)
        {
            result.X = -v.X;
            result.Y = -v.Y;
            result.Z = -v.Z;
        }

        public static void Normalize(ref Vector3 v, out Vector3 result)
        {
            float len = v.Length;
            if (len > 0f)
            {
                result.X = v.X / len;
                result.Y = v.Y / len;
                result.Z = v.Z / len;
            }
            else
                result = v;
        }
        public static Vector3 Normalize(Vector3 v)
        {
            Normalize(ref v, out v);
            return v;
        }
        public static void Normalize(ref Vector3 v, float length, out Vector3 result)
        {
            float len = v.Length;
            if (len > 0f)
            {
                length /= len;
                result.X = v.X * length;
                result.Y = v.Y * length;
                result.Z = v.Z * length;
            }
            else
                result = v;
        }
        public static Vector3 Normalize(Vector3 v, float length)
        {
            Normalize(ref v, length, out v);
            return v;
        }

        public static Vector3 Parse(string str)
        {
            var split = str.Split(',');
            if (split.Length != 3)
                throw new Exception("Failed to parse Vector3 from string: '" + str + "'");
            return new Vector3(
                float.Parse(split[0]),
                float.Parse(split[1]),
                float.Parse(split[2])
            );
        }

        public static void Project(ref Vector3 p, ref Vector3 lineStart, ref Vector3 lineEnd, out Vector3 result)
        {
            throw new NotImplementedException();
        }

        public static void Reflect(ref Vector3 v, ref Vector3 axis, out Vector3 result)
        {
            throw new NotImplementedException();
        }
        public static Vector3 Reflect(Vector3 v, Vector3 axis)
        {
            Vector3 result;
            Reflect(ref v, ref axis, out result);
            return result;
        }

        public static void Round(ref Vector3 v, out Vector3 result)
        {
            result.X = Calc.Round(v.X);
            result.Y = Calc.Round(v.Y);
            result.Z = Calc.Round(v.Z);
        }
        public static Vector3 Round(Vector3 v)
        {
            Round(ref v, out v);
            return v;
        }

        public static void Sign(ref Vector3 v, out Vector3 result)
        {
            result.X = Calc.Sign(v.X);
            result.Y = Calc.Sign(v.Y);
            result.Z = Calc.Sign(v.Z);
        }
        public static Vector3 Sign(Vector3 v)
        {
            Sign(ref v, out v);
            return v;
        }

        public static void Subtract(ref Vector3 a, ref Vector3 b, out Vector3 result)
        {
            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;
            result.Z = a.Z - b.Z;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(ref b);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            Multiply(ref a, b, out a);
            return a;
        }
        public static Vector3 operator *(float b, Vector3 a)
        {
            Multiply(ref a, b, out a);
            return a;
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            Multiply(ref a, ref b, out a);
            return a;
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            Divide(ref a, b, out a);
            return a;
        }
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            Divide(ref a, ref b, out a);
            return a;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            Add(ref a, ref b, out a);
            return a;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            Subtract(ref a, ref b, out a);
            return a;
        }
        public static Vector3 operator -(Vector3 a)
        {
            Negate(ref a, out a);
            return a;
        }
    }
}

