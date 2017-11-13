using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix3x2 : IEquatable<Matrix3x2>, ICopyable<Matrix3x2>
    {
        public static readonly Matrix3x2 Identity = new Matrix3x2(1f, 0f, 0f, 0f, 1f, 0f);

        public float M0;
        public float M1;
        public float M2;
        public float M3;
        public float M4;
        public float M5;

        public Matrix3x2(float m0, float m1, float m2, float m3, float m4, float m5)
        {
            M0 = m0;
            M1 = m1;
            M2 = m2;
            M3 = m3;
            M4 = m4;
            M5 = m5;
        }

        public Vector2 Position
        {
            get { return TransformPoint(Vector2.Zero); }
        }

        public Vector2 Left
        {
            get { return TransformDirection(Vector2.Left).Normalized; }
        }

        public Vector2 Right
        {
            get { return TransformDirection(Vector2.Right).Normalized; }
        }

        public Vector2 Up
        {
            get { return TransformDirection(Vector2.Up).Normalized; }
        }

        public Vector2 Down
        {
            get { return TransformDirection(Vector2.Down).Normalized; }
        }

        public void CopyTo(out Matrix3x2 m)
        {
            m.M0 = M0;
            m.M1 = M1;
            m.M2 = M2;
            m.M3 = M3;
            m.M4 = M4;
            m.M5 = M5;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix3x2 && Equals((Matrix3x2)obj);
        }
        public bool Equals(ref Matrix3x2 other)
        {
            return M0 == other.M0
                && M1 == other.M1
                && M2 == other.M2
                && M3 == other.M3
                && M4 == other.M4
                && M5 == other.M5;
        }
        public bool Equals(Matrix3x2 other)
        {
            return Equals(ref other);
        }
            
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + M0.GetHashCode();
                hash = hash * 23 + M1.GetHashCode();
                hash = hash * 23 + M2.GetHashCode();
                hash = hash * 23 + M3.GetHashCode();
                hash = hash * 23 + M4.GetHashCode();
                hash = hash * 23 + M5.GetHashCode();
                return hash;
            }
        }
            
        public override string ToString()
        {
            return string.Format("{0},{1},{2}\n{3},{4},{5}", M0, M1, M2, M3, M4, M5);
        }

        public void Invert(out Matrix3x2 r)
        {
            float invdet = 1f / (M0 * M4 - M3 * M1);
            r.M0 = M4 * invdet;
            r.M1 = -M1 * invdet;
            r.M2 = (M1 * M5 - M2 * M4) * invdet;
            r.M3 = -M3 * invdet;
            r.M4 = M0 * invdet;
            r.M5 = -(M0 * M5 - M2 * M3) * invdet;
        }

        public Vector2 TransformPoint(Vector2 p)
        {
            return new Vector2(
                p.X * M0 + p.Y * M1 + M2,
                p.X * M3 + p.Y * M4 + M5
            );
        }

        public Vector2 TransformPoint(float x, float y)
        {
            return new Vector2(
                x * M0 + y * M1 + M2,
                x * M3 + y * M4 + M5
            );
        }

        public Vector2 TransformDirection(Vector2 p)
        {
            return new Vector2(
                p.X * M0 + p.Y * M1,
                p.X * M3 + p.Y * M4
            );
        }

        /*public void TransformQuad(ref Quad q, out Quad result)
        {
            result.A = TransformPoint(q.A);
            result.B = TransformPoint(q.B);
            result.C = TransformPoint(q.C);
            result.D = TransformPoint(q.D);
        }
        public Quad TransformQuad(Quad q)
        {
            Quad result;
            TransformQuad(ref q, out result);
            return result;
        }

        public void TransformRectangle(ref Rectangle r, out Rectangle t)
        {
            t = TransformQuad(r).Bounds;
        }
        public void TransformRectangle(Rectangle r, out Rectangle t)
        {
            t = TransformQuad(r).Bounds;
        }
        public Rectangle TransformRectangle(ref Rectangle r)
        {
            return TransformQuad(r).Bounds;
        }
        public Rectangle TransformRectangle(Rectangle r)
        {
            return TransformQuad(r).Bounds;
        }*/

        public static bool Approx(ref Matrix3x2 a, ref Matrix3x2 b)
        {
            return Calc.Approx(a.M0, b.M0)
                && Calc.Approx(a.M1, b.M1)
                && Calc.Approx(a.M2, b.M2)
                && Calc.Approx(a.M3, b.M3)
                && Calc.Approx(a.M4, b.M4)
                && Calc.Approx(a.M5, b.M5);
        }
        public static bool Approx(Matrix3x2 a, Matrix3x2 b)
        {
            return Approx(ref a, ref b);
        }

        public static void MultiplyCopy(ref Matrix3x2 a, ref Matrix3x2 b, out Matrix3x2 m)
        {
            float m0 = a.M0 * b.M0 + a.M3 * b.M1;
            float m1 = a.M1 * b.M0 + a.M4 * b.M1;
            float m2 = a.M2 * b.M0 + a.M5 * b.M1 + b.M2;
            float m3 = a.M0 * b.M3 + a.M3 * b.M4;
            float m4 = a.M1 * b.M3 + a.M4 * b.M4;
            float m5 = a.M2 * b.M3 + a.M5 * b.M4 + b.M5;
            m.M0 = m0;
            m.M1 = m1;
            m.M2 = m2;
            m.M3 = m3;
            m.M4 = m4;
            m.M5 = m5;
        }

        public static void Multiply(ref Matrix3x2 a, ref Matrix3x2 b, out Matrix3x2 m)
        {
            m.M0 = a.M0 * b.M0 + a.M3 * b.M1;
            m.M1 = a.M1 * b.M0 + a.M4 * b.M1;
            m.M2 = a.M2 * b.M0 + a.M5 * b.M1 + b.M2;
            m.M3 = a.M0 * b.M3 + a.M3 * b.M4;
            m.M4 = a.M1 * b.M3 + a.M4 * b.M4;
            m.M5 = a.M2 * b.M3 + a.M5 * b.M4 + b.M5;
        }
        public static Matrix3x2 Multiply(ref Matrix3x2 a, ref Matrix3x2 b)
        {
            Matrix3x2 m;
            Multiply(ref a, ref b, out m);
            return m;
        }
        public static void Multiply(Matrix3x2 a, Matrix3x2 b, out Matrix3x2 m)
        {
            Multiply(ref a, ref b, out m);
        }
        public static Matrix3x2 Multiply(Matrix3x2 a, Matrix3x2 b)
        {
            Matrix3x2 m;
            Multiply(ref a, ref b, out m);
            return m;
        }

        public static void Rotation(float angle, out Matrix3x2 m)
        {
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);
            m.M0 = c;
            m.M1 = -s;
            m.M2 = 0f;
            m.M3 = s;
            m.M4 = c;
            m.M5 = 0f;
        }
        public static Matrix3x2 Rotation(float angle)
        {
            Matrix3x2 m;
            Rotation(angle, out m);
            return m;
        }

        public static void Scale(float x, float y, out Matrix3x2 m)
        {
            m.M0 = x;
            m.M1 = 0f;
            m.M2 = 0f;
            m.M3 = 0f;
            m.M4 = y;
            m.M5 = 0f;
        }
        public static Matrix3x2 Scale(float x, float y)
        {
            Matrix3x2 m;
            Scale(x, y, out m);
            return m;
        }
        public static void Scale(Vector2 scale, out Matrix3x2 m)
        {
            Scale(scale.X, scale.Y, out m);
        }
        public static Matrix3x2 Scale(Vector2 scale)
        {
            Matrix3x2 m;
            Scale(scale.X, scale.Y, out m);
            return m;
        }
        public static void Scale(float scale, out Matrix3x2 m)
        {
            Scale(scale, scale, out m);
        }
        public static Matrix3x2 Scale(float scale)
        {
            Matrix3x2 m;
            Scale(scale, scale, out m);
            return m;
        }

        public static void Skew(float x, float y, out Matrix3x2 m)
        {
            m.M0 = 0f;
            m.M1 = (float)Math.Tan(x);
            m.M2 = 0f;
            m.M3 = (float)Math.Tan(y);
            m.M4 = 1f;
            m.M5 = 0f;
        }
        public static Matrix3x2 Skew(float x, float y)
        {
            Matrix3x2 m;
            Skew(x, y, out m);
            return m;
        }
        public static void Skew(Vector2 skew, out Matrix3x2 m)
        {
            Skew(skew.X, skew.Y, out m);
        }
        public static Matrix3x2 Skew(Vector2 skew)
        {
            Matrix3x2 m;
            Skew(skew.X, skew.Y, out m);
            return m;
        }

        public static void Translation(float x, float y, out Matrix3x2 m)
        {
            m.M0 = 1f;
            m.M1 = 0f;
            m.M2 = x;
            m.M3 = 0f;
            m.M4 = 1f;
            m.M5 = y;
        }
        public static Matrix3x2 Translation(float x, float y)
        {
            Matrix3x2 m;
            Translation(x, y, out m);
            return m;
        }
        public static void Translation(Vector2 translation, out Matrix3x2 m)
        {
            Translation(translation.X, translation.Y, out m);
        }
        public static Matrix3x2 Translation(Vector2 translation)
        {
            Matrix3x2 m;
            Translation(translation.X, translation.Y, out m);
            return m;
        }

        public static implicit operator Matrix4x4(Matrix3x2 m)
        {
            return new Matrix4x4(m);
        }

        public static bool operator ==(Matrix3x2 a, Matrix3x2 b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Matrix3x2 a, Matrix3x2 b)
        {
            return !a.Equals(ref b);
        }
        public static Matrix3x2 operator *(Matrix3x2 a, Matrix3x2 b)
        {
            Matrix3x2 m;
            Multiply(ref a, ref b, out m);
            return m;
        }
    }
}

