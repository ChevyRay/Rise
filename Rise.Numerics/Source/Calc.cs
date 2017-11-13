using System;
namespace Rise
{
    public static class Calc
    {
        public const float ZeroTolerance = 1e-6f;
        public const float HalfPI = (float)(Math.PI * 0.5);
        public const float PI = (float)Math.PI;
        public const float Tau = (float)(Math.PI * 2.0);
        public const float Deg = (float)(180.0 / Math.PI);
        public const float Rad = (float)(Math.PI / 180.0);

        public static float Abs(this float x)
        {
            return x >= 0f ? x : -x;
        }
        public static int Abs(this int x)
        {
            return x >= 0 ? x : -x;
        }

        public static float Acos(float x)
        {
            return (float)Math.Acos(x);
        }

        public static float Approach(this float a, float b, float amount)
        {
            if (a < b)
            {
                a += amount;
                return a > b ? b : a;
            }
            if (a > b)
            {
                a -= amount;
                return a < b ? b : a;
            }
            return b;
        }

        public static bool Approx(this float x, float y)
        {
            return x >= y - ZeroTolerance && x <= y + ZeroTolerance;
        }

        public static float Asin(this float x)
        {
            return (float)Math.Asin(x);
        }

        public static float Atan(this float x)
        {
            return (float)Math.Atan(x);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float Barycentric(float a, float b, float c, float n1, float n2)
        {
            return a + (b - a) * n1 + (c - a) * n2;
        }

        public static float Bezier(float a, float b, float c, float t)
        {
            return a * (1f - t) * (1f - t) + b * 2f * (1f - t) * t + c * t * t;
        }

        public static float Bezier(float a, float b, float c, float d, float t)
        {
            return t * t * t * (d + 3f * (b - c) - a) + 3f * t * t * (a - 2f * b + c) + 3f * t * (b - a) + a;
        }

        public static float CatmullRom(float a, float b, float c, float d, float t)
        {
            return 0.5f * (2f * b + (c - a) * t + (2f * a - 5f * b + 4f * c - d) * t * t + (3f * b - a - 3f * c + d) * t * t * t);
        }

        public static float Clamp(this float x, float min, float max)
        {
            Order(ref min, ref max);
            return x < min ? min : (x > max ? max : x);
        }

        public static int Clamp(this int x, int min, int max)
        {
            Order(ref min, ref max);
            return x < min ? min : (x > max ? max : x);
        }

        public static float Cos(this float x)
        {
            return (float)Math.Cos(x);
        }

        public static float Cosh(this float x)
        {
            return (float)Math.Cosh(x);
        }

        public static float Distance(float x0, float y0, float x1, float y1)
        {
            return (float)Math.Sqrt(DistanceSquared(x0, y0, x1, y1));
        }

        public static float DistanceSquared(float x0, float y0, float x1, float y1)
        {
            return (x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1);
        }

        public static bool InRange(this float x, float min, float max)
        {
            Order(ref min, ref max);
            return x >= min && x <= max;
        }

        public static bool InRange(this int x, int min, int max)
        {
            Order(ref min, ref max);
            return x >= min && x <= max;
        }

        public static bool IsMultipleOf(int largeNum, int smallNum)
        {
            return ((largeNum / smallNum) * smallNum) == largeNum;
        }

        public static float Sign(this float x)
        {
            return x > 0f ? 1f : (x < 0f ? -1f : 0f);
        }

        public static int Sign(this int x)
        {
            return x > 0 ? 1 : (x < 0 ? -1 : 0);
        }

        public static int SignInt(this float x)
        {
            return x > 0f ? 1 : (x < 0f ? -1 : 0);
        }

        public static bool SameSign(this float x, float y)
        {
            return x > 0f ? y > 0f : (x < 0f ? y < 0f : y == 0f);
        }

        public static bool SameSign(this int x, int y)
        {
            return x > 0 ? y > 0 : (x < 0 ? y < 0 : y == 0);
        }
            
        public static float Round(this float x)
        {
            return (float)Math.Round(x);
        }
            
        public static int RoundToInt(this float x)
        {
            return (int)Math.Round(x);
        }
            
        public static float Ceil(this float x)
        {
            return (float)Math.Ceiling(x);
        }
            
        public static int CeilToInt(this float x)
        {
            return (int)Math.Ceiling(x);
        }
            
        public static float Floor(this float x)
        {
            return (float)Math.Floor(x);
        }
            
        public static int FloorToInt(this float x)
        {
            return (int)Math.Floor(x);
        }

        public static float Hermite(float p0, float m0, float p1, float m1, float t)
        {
            return (2f * p0 - 2f * p1 + m1 + m0) * t * t * t + (3f * p1 - 3f * p0 - 2f * m0 - m1) * t * t + m0 * t + p0;
        }

        public static float SmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public static float InvLerp(this float a, float b, float t)
        {
            return (t - a) / (b - a);
        }

        public static float Lerp(this float a, float b, float t)
        {
            //return a + (b - a) * t;
            return a * (1f - t) + b * t;
        }

        public static double Lerp(this double a, double b, double t)
        {
            //return a + (b - a) * t;
            return a * (1.0 - t) + b * t;
        }

        public static float Log(this float x)
        {
            return (float)Math.Log(x);
        }

        public static float Log(this float x, float b)
        {
            return (float)Math.Log(x, b);
        }

        public static float Max(float a, float b)
        {
            return Math.Max(a, b);
        }

        public static float Max(float a, float b, float c)
        {
            return Math.Max(Math.Max(a, b), c);
        }

        public static float Max(float a, float b, float c, float d)
        {
            return Math.Max(Math.Max(Math.Max(a, b), c), d);
        }

        public static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }

        public static int Max(int a, int b, int c)
        {
            return Math.Max(Math.Max(a, b), c);
        }

        public static int Max(int a, int b, int c, int d)
        {
            return Math.Max(Math.Max(Math.Max(a, b), c), d);
        }

        public static float Min(float a, float b)
        {
            return a <= b ? a : b;
        }

        public static float Min(float a, float b, float c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        public static float Min(float a, float b, float c, float d)
        {
            return Math.Min(Math.Min(Math.Min(a, b), c), d);
        }

        public static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static int Min(int a, int b, int c)
        {
            return Math.Min(Math.Min(a, b), c);
        }

        public static int Min(int a, int b, int c, int d)
        {
            return Math.Min(Math.Min(Math.Min(a, b), c), d);
        }

        public static float Map(this float x, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + ((x - inMin) / (inMax - inMin)) * (outMax - outMin);
        }

        public static float MapClamp(this float x, float inMin, float inMax, float outMin, float outMax)
        {
            return Clamp(Map(x, inMin, inMax, outMin, outMax), outMin, outMax);
        }

        public static bool IsPowerOf2(this int x)
        {
            return x != 0 && (x & (x - 1)) == 0;
        }

        public static int NextPowerOf2(this int x)
        {
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        public static void Order<T>(ref T a, ref T b) where T : IComparable
        {
            if (a.CompareTo(b) > 0)
            {
                T c = a;
                a = b;
                b = c;
            }
        }

        public static float Pow(float x, float y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Sin(this float x)
        {
            return (float)Math.Sin(x);
        }

        public static float Sinh(this float x)
        {
            return (float)Math.Sinh(x);
        }

        public static float Sqrt(this float x)
        {
            return (float)Math.Sqrt(x);
        }

        public static int Snap(this int x, int mult)
        {
            return (int)Math.Round((double)x / mult) * mult;
        }

        public static float Snap(this float x, float mult)
        {
            return (float)Math.Round(x / mult) * mult;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        public static float Tan(this float x)
        {
            return (float)Math.Tan(x);
        }

        public static float Tanh(this float x)
        {
            return (float)Math.Tanh(x);
        }
    }
}

