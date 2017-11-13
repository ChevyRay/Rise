using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Vector2 : IEquatable<Vector2>
    {
        public static readonly Vector2 Zero = new Vector2(0f, 0f);
        public static readonly Vector2 One = new Vector2(1f);
        public static readonly Vector2 Right = new Vector2(1f, 0f);
        public static readonly Vector2 Left = new Vector2(-1f, 0f);
        public static readonly Vector2 Up = new Vector2(0f, -1f);
        public static readonly Vector2 Down = new Vector2(0f, 1f);

        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Vector2(float value)
        {
            X = value;
            Y = value;
        }

        public bool IsZero
        {
            get { return X == 0f && Y == 0f; }
        }

        public bool IsApproxZero
        {
            get { return Calc.Approx(X, 0f) && Calc.Approx(Y, 0f); }
        }

        public float LengthSquared
        {
            get { return X * X + Y * Y; }
        }

        public float Length
        {
            get { return (float)Math.Sqrt(LengthSquared); }
        }

        public float Angle
        {
            get { return (float)Math.Atan2(Y, X); }
        }

        public Vector2 Normalized
        {
            get { return Normalize(this); }
        }

        public Vector2 LeftPerp
        {
            get { return TurnLeft(this); }
        }

        public Vector2 RightPerp
        {
            get { return TurnRight(this); }
        }

        public Vector2 Centroid
        {
            get { return this; }
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2 && Equals((Vector2)obj);
        }
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", X, Y);
        }

        public static Vector2 Abs(Vector2 v)
        {
            if (v.X < 0f)
                v.X = -v.X;
            if (v.Y < 0f)
                v.Y = -v.Y;
            return v;
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return (float)Math.Atan2(Math.Abs(Cross(a, b)), Dot(a, b));
        }

        public static Vector2 Approach(Vector2 a, Vector2 b, float amount)
        {
            if (a != b)
            {
                var move = b - a;
                if (move.Length > amount)
                    return a + Normalize(move) * amount;
            }
            return b;
        }

        public static bool Approx(Vector2 a, Vector2 b)
        {
            return Calc.Approx(a.X, b.X) && Calc.Approx(a.Y, b.Y);
        }

        public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            a.X = Calc.Bezier(a.X, b.X, c.X, t);
            a.Y = Calc.Bezier(a.Y, b.Y, c.Y, t);
            return a;
        }
        public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            a.X = Calc.Bezier(a.X, b.X, c.X, d.X, t);
            a.Y = Calc.Bezier(a.Y, b.Y, c.Y, d.Y, t);
            return a;
        }

        public static Vector2 CatmullRom(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            a.X = Calc.CatmullRom(a.X, b.X, c.X, d.X, t);
            a.Y = Calc.CatmullRom(a.Y, b.Y, c.Y, d.Y, t);
            return a;
        }

        public static Vector2 Ceil(Vector2 a)
        {
            a.X = Calc.Ceil(a.X);
            a.Y = Calc.Ceil(a.Y);
            return a;
        }

        public static Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
        {
            a.X = Calc.Clamp(a.X, min.X, max.X);
            a.Y = Calc.Clamp(a.Y, min.Y, max.Y);
            return a;
        }

        public static Vector2 ClampLength(Vector2 a, float max)
        {
            float len = a.LengthSquared;
            if (len > max * max)
                return Normalize(a, max);
            return a;
        }
        public static Vector2 ClampLength(Vector2 a, float min, float max)
        {
            if (min > max)
                Calc.Swap(ref min, ref max);
            float len = a.LengthSquared;
            if (len > max * max)
                return Normalize(a, max);
            else if (len < min * min)
                return Normalize(a, min);
            return a;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            return Calc.Distance(a.X, a.Y, b.X, b.Y);
        }

        public static float DistanceSquared(Vector2 a, Vector2 b)
        {
            return Calc.DistanceSquared(a.X, a.Y, b.X, b.Y);
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static Vector2 Floor(Vector2 a)
        {
            a.X = Calc.Floor(a.X);
            a.Y = Calc.Floor(a.Y);
            return a;
        }

        public static Vector2 Hermite(Vector2 a, float ta, Vector2 b, float tb, float t)
        {
            a.X = Calc.Hermite(a.X, ta, b.X, tb, t);
            a.Y = Calc.Hermite(a.Y, ta, b.Y, tb, t);
            return a;
        }
        public static Vector2 Hermite(Vector2 a, Vector2 ta, Vector2 b, Vector2 tb, float t)
        {
            a.X = Calc.Hermite(a.X, ta.X, b.X, tb.X, t);
            a.Y = Calc.Hermite(a.Y, ta.Y, b.Y, tb.Y, t);
            return a;
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            a.X += (b.X - a.X) * t;
            a.Y += (b.Y - a.Y) * t;
            return a;
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            a.X = Math.Max(a.X, b.X);
            a.Y = Math.Max(a.Y, b.Y);
            return a;
        }

        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            a.X = Math.Min(a.X, b.X);
            a.Y = Math.Min(a.Y, b.Y);
            return a;
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float len = v.Length;
            if (len > 0f)
                return new Vector2(v.X / len, v.Y / len);
            return v;
        }
        public static Vector2 Normalize(Vector2 v, float length)
        {
            float len = v.Length;
            if (len > 0f)
            {
                length /= len;
                return new Vector2(v.X * length, v.Y * length);
            }
            return v;
        }

        public static Vector2 Parse(string str)
        {
            int i = str.IndexOf(',');
            float x, y;
            if (i <= 0 || i >= str.Length - 1 || !float.TryParse(str.Substring(0, i), out x) || !float.TryParse(str.Substring(i + 1), out y))
                throw new Exception("Failed to parse vector from string: '" + str + "'");
            return new Vector2(x, y);
        }

        public static Vector2 Polar(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public static Vector2 Polar(float angle, float length)
        {
            return new Vector2((float)(Math.Cos(angle) * length), (float)(Math.Sin(angle) * length));
        }

        public static Vector2 Project(Vector2 p, Vector2 lineStart, Vector2 lineEnd)
        {
            var axis = Normalize(lineEnd - lineStart);
            return lineStart + axis * Dot(p, axis);
        }

        public static Vector2 Reflect(Vector2 v, Vector2 axis)
        {
            Vector2 result;
            float val = Dot(v, axis) * 2.0f;
            result.X = v.X - (axis.X * val);
            result.Y = v.Y - (axis.Y * val);
            return result;
        }

        public static Vector2 Rotate(Vector2 v, float amount)
        {
            float len = v.Length;
            float ang = v.Angle + amount;
            v.X = (float)(Math.Cos(ang) * len);
            v.Y = (float)(Math.Sin(ang) * len);
            return v;
        }
        public static Vector2 Rotate(Vector2 a, float cos, float sin)
        {
            return new Vector2(
                a.X * cos - a.Y * sin,
                a.X * sin + a.Y * cos
            );
        }

        public static Vector2 Round(Vector2 a)
        {
            a.X = Calc.Round(a.X);
            a.Y = Calc.Round(a.Y);
            return a;
        }

        public static Vector2 Sign(Vector2 v)
        {
            return new Vector2(Calc.Sign(v.X), Calc.Sign(v.Y));
        }

        public static bool TryParse(string str, out Vector2 result)
        {
            int i = str.IndexOf(',');
            float x, y;
            if (i <= 0 || i >= str.Length - 1 || !float.TryParse(str.Substring(0, i), out x) || !float.TryParse(str.Substring(i + 1), out y))
            {
                result = default(Vector2);
                return false;
            }
            result = new Vector2(x, y);
            return true;
        }

        public static Vector2 TurnLeft(Vector2 dir)
        {
            float x = dir.X;
            dir.X = dir.Y;
            dir.Y = -x;
            return dir;
        }

        public static Vector2 TurnRight(Vector2 dir)
        {
            float x = dir.X;
            dir.X = -dir.Y;
            dir.Y = x;
            return dir;
        }

        public static implicit operator Vector3(Vector2 a)
        {
            return new Vector3(a.X, a.Y, 0f);
        }

        public static implicit operator Vector4(Vector2 a)
        {
            return new Vector4(a.X, a.Y, 0f, 0f);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !a.Equals(b);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            a.X *= b;
            a.Y *= b;
            return a;
        }
        public static Vector2 operator *(float b, Vector2 a)
        {
            a.X *= b;
            a.Y *= b;
            return a;
        }
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
            return a;
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            a.X /= b;
            a.Y /= b;
            return a;
        }
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            a.X /= b.X;
            a.Y /= b.Y;
            return a;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            a.X += b.X;
            a.Y += b.Y;
            return a;
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            a.X -= b.X;
            a.Y -= b.Y;
            return a;
        }

        public static Vector2 operator -(Vector2 a)
        {
            a.X = -a.X;
            a.Y = -a.Y;
            return a;
        }
    }
}

