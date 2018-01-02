using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Matrix4x4 : IEquatable<Matrix4x4>, ICopyable<Matrix4x4>
    {
        public static readonly Matrix4x4 Identity = new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public Matrix4x4(float m11, float m12, float m13, float m14,
                        float m21, float m22, float m23, float m24,
                        float m31, float m32, float m33, float m34,
                        float m41, float m42, float m43, float m44)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
            M41 = m41; M42 = m42; M43 = m43; M44 = m44;
        }
        public Matrix4x4(Matrix3x2 matrix)
            : this(matrix.M0, matrix.M1, matrix.M2, 0f,
                   matrix.M4, matrix.M4, matrix.M5, 0f,
                   0f, 0f, 1f, 0f,
                   0f, 0f, 0f, 1f)
        {
            
        }

        public Vector3 Position
        {
            get { return new Vector3(M41, M42, M43); }
        }

        public Vector3 Right
        {
            get { return new Vector3(M11, M12, M13); }
        }

        public Vector3 Left
        {
            get { return new Vector3(-M11, -M12, -M13); }
        }

        public Vector3 Up
        {
            get { return new Vector3(M21, M22, M23); }
        }

        public Vector3 Down
        {
            get { return new Vector3(-M21, -M22, -M23); }
        }

        public Vector3 Forward
        {
            get { return new Vector3(-M31, -M32, -M33); }
        }

        public Vector3 Backward
        {
            get { return new Vector3(M31, M32, M33); }
        }

        public Matrix4x4 Inverse
        {
            get
            {
                Matrix4x4 m;
                Invert(out m);
                return m;
            }
        }

        public void CopyTo(out Matrix4x4 m)
        {
            m.M11 = M11;
            m.M12 = M12;
            m.M13 = M13;
            m.M14 = M14;
            m.M21 = M21;
            m.M22 = M22;
            m.M23 = M23;
            m.M24 = M24;
            m.M31 = M31;
            m.M32 = M32;
            m.M33 = M33;
            m.M34 = M34;
            m.M41 = M41;
            m.M42 = M42;
            m.M43 = M43;
            m.M44 = M44;
        }

        public override bool Equals(object obj)
        {
            return obj is Matrix4x4 && Equals((Matrix4x4)obj);
        }
        public bool Equals(Matrix4x4 other)
        {
            return Equals(ref other);
        }
        public bool Equals(ref Matrix4x4 other)
        {
            return M11 == other.M11
                && M12 == other.M12
                && M13 == other.M13
                && M14 == other.M14
                && M21 == other.M21
                && M22 == other.M22
                && M23 == other.M23
                && M24 == other.M24
                && M31 == other.M31
                && M32 == other.M32
                && M33 == other.M33
                && M34 == other.M34
                && M41 == other.M41
                && M42 == other.M42
                && M43 == other.M43
                && M44 == other.M44;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + M11.GetHashCode();
                hash = hash * 23 + M12.GetHashCode();
                hash = hash * 23 + M13.GetHashCode();
                hash = hash * 23 + M14.GetHashCode();
                hash = hash * 23 + M21.GetHashCode();
                hash = hash * 23 + M22.GetHashCode();
                hash = hash * 23 + M23.GetHashCode();
                hash = hash * 23 + M24.GetHashCode();
                hash = hash * 23 + M31.GetHashCode();
                hash = hash * 23 + M32.GetHashCode();
                hash = hash * 23 + M33.GetHashCode();
                hash = hash * 23 + M34.GetHashCode();
                hash = hash * 23 + M41.GetHashCode();
                hash = hash * 23 + M42.GetHashCode();
                hash = hash * 23 + M43.GetHashCode();
                hash = hash * 23 + M44.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}",
                M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44
            );
        }

        public void TransformPoint(ref Vector2 p, out Vector2 result)
        {
            var x = p.X * M11 + p.Y * M21 + M41;
            var y = p.X * M12 + p.Y * M22 + M42;
            result.X = x;
            result.Y = y;
        }
        public Vector2 TransformPoint(Vector2 p)
        {
            TransformPoint(ref p, out p);
            return p;
        }
        public void TransformPoint(ref Vector3 p, out Vector3 result)
        {
            var x = p.X * M11 + p.Y * M21 + p.Z * M31 + M41;
            var y = p.X * M12 + p.Y * M22 + p.Z * M32 + M42;
            var z = p.X * M13 + p.Y * M23 + p.Z * M33 + M43;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }
        public Vector3 TransformPoint(ref Vector3 p)
        {
            Vector3 result;
            TransformPoint(ref p, out result);
            return result;
        }
        public Vector3 TransformPoint(Vector3 p)
        {
            TransformPoint(ref p, out p);
            return p;
        }
        public void TransformPoint(ref Vector3 p, out Vector4 result)
        {
            result.X = p.X * M11 + p.Y * M21 + p.Z * M31 + M41;
            result.Y = p.X * M12 + p.Y * M22 + p.Z * M32 + M42;
            result.Z = p.X * M13 + p.Y * M23 + p.Z * M33 + M43;
            result.W = p.X * M14 + p.Y * M24 + p.Z * M34 + M44;
        }
        public void TransformPoint(ref Vector4 p, out Vector4 result)
        {
            var x = p.X * M11 + p.Y * M21 + p.Z * M31 + p.W * M41;
            var y = p.X * M12 + p.Y * M22 + p.Z * M32 + p.W * M42;
            var z = p.X * M13 + p.Y * M23 + p.Z * M33 + p.W * M43;
            var w = p.X * M14 + p.Y * M24 + p.Z * M34 + p.W * M44;
            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = w;
        }
        public void TransformPoint(ref Vector4 p, out Vector3 result)
        {
            var x = p.X * M11 + p.Y * M21 + p.Z * M31 + p.W * M41;
            var y = p.X * M12 + p.Y * M22 + p.Z * M32 + p.W * M42;
            var z = p.X * M13 + p.Y * M23 + p.Z * M33 + p.W * M43;
            var w = p.X * M14 + p.Y * M24 + p.Z * M34 + p.W * M44;
            result.X = x / w;
            result.Y = y / w;
            result.Z = z / w;
        }
        public Vector4 TransformPoint(Vector4 p)
        {
            TransformPoint(ref p, out p);
            return p;
        }

        public void TransformDirection(ref Vector3 p, out Vector3 result)
        {
            var x = p.X * M11 + p.Y * M21 + p.Z * M31;
            var y = p.X * M12 + p.Y * M22 + p.Z * M32;
            var z = p.X * M13 + p.Y * M23 + p.Z * M33;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }
        public Vector3 TransformDirection(ref Vector3 p)
        {
            Vector3 result;
            TransformDirection(ref p, out result);
            return result;
        }
        public Vector3 TransformDirection(Vector3 p)
        {
            Vector3 result;
            TransformDirection(ref p, out result);
            return result;
        }

        /*public void TransformQuad(ref Quad quad, out Quad result)
        {
            TransformPoint(ref quad.A, out result.A);
            TransformPoint(ref quad.B, out result.B);
            TransformPoint(ref quad.C, out result.C);
            TransformPoint(ref quad.D, out result.D);
        }
        public Quad TransformQuad(Quad quad)
        {
            TransformQuad(ref quad, out quad);
            return quad;
        }
        
        public void TransformRectangle(ref Rectangle r, out Rectangle result)
        {
            var q = new Quad(ref r);
            TransformQuad(ref q, out q);
            q.GetBounds(out result);
        }
        public Rectangle TransformRectangle(Rectangle r)
        {
            var q = new Quad(ref r);
            TransformQuad(ref q, out q);
            q.GetBounds(out r);
            return r;
        }*/

        public void Invert(out Matrix4x4 result)
        {
            float b0 = M31 * M42 - M32 * M41;
            float b1 = M31 * M43 - M33 * M41;
            float b2 = M34 * M41 - M31 * M44;
            float b3 = M32 * M43 - M33 * M42;
            float b4 = M34 * M42 - M32 * M44;
            float b5 = M33 * M44 - M34 * M43;
            float d11 = M22 * b5 + M23 * b4 + M24 * b3;
            float d12 = M21 * b5 + M23 * b2 + M24 * b1;
            float d13 = M21 * -b4 + M22 * b2 + M24 * b0;
            float d14 = M21 * b3 + M22 * -b1 + M23 * b0;
            float det = M11 * d11 - M12 * d12 + M13 * d13 - M14 * d14;
            if (det == 0.0f)
            {
                result = new Matrix4x4(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
                return;
            }
            det = 1f / det;
            float a0 = M11 * M22 - M12 * M21;
            float a1 = M11 * M23 - M13 * M21;
            float a2 = M14 * M21 - M11 * M24;
            float a3 = M12 * M23 - M13 * M22;
            float a4 = M14 * M22 - M12 * M24;
            float a5 = M13 * M24 - M14 * M23;
            float d21 = M12 * b5 + M13 * b4 + M14 * b3;
            float d22 = M11 * b5 + M13 * b2 + M14 * b1;
            float d23 = M11 * -b4 + M12 * b2 + M14 * b0;
            float d24 = M11 * b3 + M12 * -b1 + M13 * b0;
            float d31 = M42 * a5 + M43 * a4 + M44 * a3;
            float d32 = M41 * a5 + M43 * a2 + M44 * a1;
            float d33 = M41 * -a4 + M42 * a2 + M44 * a0;
            float d34 = M41 * a3 + M42 * -a1 + M43 * a0;
            float d41 = M32 * a5 + M33 * a4 + M34 * a3;
            float d42 = M31 * a5 + M33 * a2 + M34 * a1;
            float d43 = M31 * -a4 + M32 * a2 + M34 * a0;
            float d44 = M31 * a3 + M32 * -a1 + M33 * a0;
            result.M11 = +d11 * det;
            result.M12 = -d21 * det;
            result.M13 = +d31 * det;
            result.M14 = -d41 * det;
            result.M21 = -d12 * det;
            result.M22 = +d22 * det;
            result.M23 = -d32 * det;
            result.M24 = +d42 * det;
            result.M31 = +d13 * det;
            result.M32 = -d23 * det;
            result.M33 = +d33 * det;
            result.M34 = -d43 * det;
            result.M41 = -d14 * det;
            result.M42 = +d24 * det;
            result.M43 = -d34 * det;
            result.M44 = +d44 * det;
        }

        public static bool Approx(ref Matrix4x4 a, ref Matrix4x4 b)
        {
            return Calc.Approx(a.M11, b.M11)
                && Calc.Approx(a.M12, b.M12)
                && Calc.Approx(a.M13, b.M13)
                && Calc.Approx(a.M14, b.M14)
                && Calc.Approx(a.M21, b.M21)
                && Calc.Approx(a.M22, b.M22)
                && Calc.Approx(a.M23, b.M23)
                && Calc.Approx(a.M24, b.M24)
                && Calc.Approx(a.M31, b.M31)
                && Calc.Approx(a.M32, b.M32)
                && Calc.Approx(a.M33, b.M33)
                && Calc.Approx(a.M34, b.M34)
                && Calc.Approx(a.M41, b.M41)
                && Calc.Approx(a.M42, b.M42)
                && Calc.Approx(a.M43, b.M43)
                && Calc.Approx(a.M44, b.M44);
        }
        public static bool Approx(Matrix4x4 a, Matrix4x4 b)
        {
            return Approx(ref a, ref b);
        }

        static void Multiply(ref Matrix4x4 m, out Matrix4x4 r,
                             float m11, float m12, float m13, float m14,
                             float m21, float m22, float m23, float m24,
                             float m31, float m32, float m33, float m34,
                             float m41, float m42, float m43, float m44)
        {
            r.M11 = m.M11 * m11 + m.M12 * m21 + m.M13 * m31 + m.M14 * m41;
            r.M12 = m.M11 * m12 + m.M12 * m22 + m.M13 * m32 + m.M14 * m42;
            r.M13 = m.M11 * m13 + m.M12 * m23 + m.M13 * m33 + m.M14 * m43;
            r.M14 = m.M11 * m14 + m.M12 * m24 + m.M13 * m34 + m.M14 * m44;
            r.M21 = m.M21 * m11 + m.M22 * m21 + m.M23 * m31 + m.M24 * m41;
            r.M22 = m.M21 * m12 + m.M22 * m22 + m.M23 * m32 + m.M24 * m42;
            r.M23 = m.M21 * m13 + m.M22 * m23 + m.M23 * m33 + m.M24 * m43;
            r.M24 = m.M21 * m14 + m.M22 * m24 + m.M23 * m34 + m.M24 * m44;
            r.M31 = m.M31 * m11 + m.M32 * m21 + m.M33 * m31 + m.M34 * m41;
            r.M32 = m.M31 * m12 + m.M32 * m22 + m.M33 * m32 + m.M34 * m42;
            r.M33 = m.M31 * m13 + m.M32 * m23 + m.M33 * m33 + m.M34 * m43;
            r.M34 = m.M31 * m14 + m.M32 * m24 + m.M33 * m34 + m.M34 * m44;
            r.M41 = m.M41 * m11 + m.M42 * m21 + m.M43 * m31 + m.M44 * m41;
            r.M42 = m.M41 * m12 + m.M42 * m22 + m.M43 * m32 + m.M44 * m42;
            r.M43 = m.M41 * m13 + m.M42 * m23 + m.M43 * m33 + m.M44 * m43;
            r.M44 = m.M41 * m14 + m.M42 * m24 + m.M43 * m34 + m.M44 * m44;
        }
        public static void Multiply(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 m)
        {
            m.M11 = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31 + a.M14 * b.M41;
            m.M12 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32 + a.M14 * b.M42;
            m.M13 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33 + a.M14 * b.M43;
            m.M14 = a.M11 * b.M14 + a.M12 * b.M24 + a.M13 * b.M34 + a.M14 * b.M44;
            m.M21 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31 + a.M24 * b.M41;
            m.M22 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32 + a.M24 * b.M42;
            m.M23 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33 + a.M24 * b.M43;
            m.M24 = a.M21 * b.M14 + a.M22 * b.M24 + a.M23 * b.M34 + a.M24 * b.M44;
            m.M31 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31 + a.M34 * b.M41;
            m.M32 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32 + a.M34 * b.M42;
            m.M33 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33 + a.M34 * b.M43;
            m.M34 = a.M31 * b.M14 + a.M32 * b.M24 + a.M33 * b.M34 + a.M34 * b.M44;
            m.M41 = a.M41 * b.M11 + a.M42 * b.M21 + a.M43 * b.M31 + a.M44 * b.M41;
            m.M42 = a.M41 * b.M12 + a.M42 * b.M22 + a.M43 * b.M32 + a.M44 * b.M42;
            m.M43 = a.M41 * b.M13 + a.M42 * b.M23 + a.M43 * b.M33 + a.M44 * b.M43;
            m.M44 = a.M41* b.M14 + a.M42 * b.M24 + a.M43 * b.M34 + a.M44 * b.M44;
        }
        public static Matrix4x4 Multiply(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 m;
            Multiply(ref a, ref b, out m);
            return m;
        }
            
        public static void CreateOrthographic(float width, float height, float near, float far, out Matrix4x4 result)
        {
            result.M11 = 2f / width;
            result.M12 = result.M13 = result.M14 = 0f;
            result.M22 = 2f / height;
            result.M21 = result.M23 = result.M24 = 0f;
            result.M33 = 1f / (near - far);
            result.M31 = result.M32 = result.M34 = 0f;
            result.M41 = result.M42 = 0f;
            result.M43 = near / (near - far);
            result.M44 = 1f;
        }
        public static Matrix4x4 CreateOrthographic(float width, float height, float near, float far)
        {
            Matrix4x4 result;
            CreateOrthographic(width, height, near, far, out result);
            return result;
        }
        public static void CreateOrthographic(float left, float right, float bottom, float top, float near, float far, out Matrix4x4 result)
        {
            result.M11 = (float)(2.0 / ((double)right - (double)left));
            result.M12 = result.M13 = result.M14 = result.M21 = 0.0f;
            result.M22 = (float)(2.0 / ((double)top - (double)bottom));
            result.M23 = result.M24 = result.M31 = result.M32 = 0.0f;
            result.M33 = (float)(1.0 / ((double)near - (double)far));
            result.M34 = 0.0f;
            result.M41 = (float)(((double)left + (double)right) / ((double)left - (double)right));
            result.M42 = (float)(((double)top + (double)bottom) / ((double)bottom - (double)top));
            result.M43 = (float)((double)near / ((double)near - (double)far));
            result.M44 = 1.0f;
        }
        public static Matrix4x4 CreateOrthographic(float left, float right, float bottom, float top, float near, float far)
        {
            Matrix4x4 result;
            CreateOrthographic(left, right, bottom, top, near, far, out result);
            return result;
        }

        public static void CreateRotationX(float angle, out Matrix4x4 result)
        {
            result = Identity;
            var val1 = (float)Math.Cos(angle);
            var val2 = (float)Math.Sin(angle);
            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;
        }
        public static Matrix4x4 CreateRotationX(float angle)
        {
            Matrix4x4 result;
            CreateRotationX(angle, out result);
            return result;
        }

        public static void CreateRotationY(float angle, out Matrix4x4 result)
        {
            result = Identity;
            var val1 = (float)Math.Cos(angle);
            var val2 = (float)Math.Sin(angle);
            result.M11 = val1;
            result.M13 = -val2;
            result.M31 = val2;
            result.M33 = val1;
        }
        public static Matrix4x4 CreateRotationY(float angle)
        {
            Matrix4x4 result;
            CreateRotationY(angle, out result);
            return result;
        }

        public static void CreateRotationZ(float radians, out Matrix4x4 result)
        {
            result = Identity;
            var val1 = (float)Math.Cos(radians);
            var val2 = (float)Math.Sin(radians);
            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
        }
        public static Matrix4x4 CreateRotationZ(float radians)
        {
            Matrix4x4 result;
            CreateRotationZ(radians, out result);
            return result;
        }

        public static void CreateRotation(ref Quaternion quaternion, out Matrix4x4 result)
        {
            float num9 = quaternion.X * quaternion.X;
            float num8 = quaternion.Y * quaternion.Y;
            float num7 = quaternion.Z * quaternion.Z;
            float num6 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num4 = quaternion.Z * quaternion.X;
            float num3 = quaternion.Y * quaternion.W;
            float num2 = quaternion.Y * quaternion.Z;
            float num = quaternion.X * quaternion.W;
            result.M11 = 1f - (2f * (num8 + num7));
            result.M12 = 2f * (num6 + num5);
            result.M13 = 2f * (num4 - num3);
            result.M14 = 0f;
            result.M21 = 2f * (num6 - num5);
            result.M22 = 1f - (2f * (num7 + num9));
            result.M23 = 2f * (num2 + num);
            result.M24 = 0f;
            result.M31 = 2f * (num4 + num3);
            result.M32 = 2f * (num2 - num);
            result.M33 = 1f - (2f * (num8 + num9));
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;
        }
        public static Matrix4x4 CreateRotation(Quaternion quaternion)
        {
            Matrix4x4 result;
            CreateRotation(ref quaternion, out result);
            return result;
        }

        public static Matrix4x4 CreateScale(float x, float y, float z)
        {
            return new Matrix4x4(x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
        }
        public static Matrix4x4 CreateScale(ref Vector3 scale)
        {
            return CreateScale(scale.X, scale.Y, scale.Z);
        }
        public static Matrix4x4 CreateScale(Vector3 scale)
        {
            return CreateScale(scale.X, scale.Y, scale.Z);
        }
        public static Matrix4x4 CreateScale(Vector2 scale)
        {
            return CreateScale(scale.X, scale.Y, 1f);
        }
        public static Matrix4x4 CreateScale(float x, float y)
        {
            return CreateScale(x, y, 1f);
        }
        public static Matrix4x4 CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        public static Matrix4x4 CreateTranslation(float x, float y, float z)
        {
            return new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, x, y, z, 1f);
        }
        public static Matrix4x4 CreateTranslation(ref Vector3 translation)
        {
            return CreateTranslation(translation.X, translation.Y, translation.Z);
        }
        public static Matrix4x4 CreateTranslation(Vector3 translation)
        {
            return CreateTranslation(translation.X, translation.Y, translation.Z);
        }
        public static Matrix4x4 CreateTranslation(Vector2 translation)
        {
            return CreateTranslation(translation.X, translation.Y, 0f);
        }
        public static Matrix4x4 CreateTranslation(float x, float y)
        {
            return CreateTranslation(x, y, 0f);
        }

        public static void CreateAxisAngle(ref Vector3 axis, float angle, out Matrix4x4 result)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num2 = (float)Math.Sin((double)angle);
            float num = (float)Math.Cos((double)angle);
            float num11 = x * x;
            float num10 = y * y;
            float num9 = z * z;
            float num8 = x * y;
            float num7 = x * z;
            float num6 = y * z;
            result.M11 = num11 + (num * (1f - num11));
            result.M12 = (num8 - (num * num8)) + (num2 * z);
            result.M13 = (num7 - (num * num7)) - (num2 * y);
            result.M14 = 0;
            result.M21 = (num8 - (num * num8)) - (num2 * z);
            result.M22 = num10 + (num * (1f - num10));
            result.M23 = (num6 - (num * num6)) + (num2 * x);
            result.M24 = 0;
            result.M31 = (num7 - (num * num7)) + (num2 * y);
            result.M32 = (num6 - (num * num6)) - (num2 * x);
            result.M33 = num9 + (num * (1f - num9));
            result.M34 = 0;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            result.M44 = 1;
        }
        public static Matrix4x4 CreateAxisAngle(Vector3 axis, float angle)
        {
            Matrix4x4 result;
            CreateAxisAngle(ref axis, angle, out result);
            return result;
        }

        public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix4x4 result)
        {
            var vector = Vector3.Normalize(cameraPosition - cameraTarget);
            var vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            var vector3 = Vector3.Cross(vector, vector2);
            result.M11 = vector2.X;
            result.M12 = vector3.X;
            result.M13 = vector.X;
            result.M14 = 0f;
            result.M21 = vector2.Y;
            result.M22 = vector3.Y;
            result.M23 = vector.Y;
            result.M24 = 0f;
            result.M31 = vector2.Z;
            result.M32 = vector3.Z;
            result.M33 = vector.Z;
            result.M34 = 0f;
            result.M41 = -Vector3.Dot(vector2, cameraPosition);
            result.M42 = -Vector3.Dot(vector3, cameraPosition);
            result.M43 = -Vector3.Dot(vector, cameraPosition);
            result.M44 = 1f;
        }
        public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Matrix4x4 result;
            CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out result);
            return result;
        }

        public static void CreatePerspective(float width, float height, float near, float far, out Matrix4x4 result)
        {
            result.M11 = (2f * near) / width;
            result.M12 = result.M13 = result.M14 = 0f;
            result.M22 = (2f * near) / height;
            result.M21 = result.M23 = result.M24 = 0f;
            result.M33 = far / (near - far);
            result.M31 = result.M32 = 0f;
            result.M34 = -1f;
            result.M41 = result.M42 = result.M44 = 0f;
            result.M43 = (near * far) / (near - far);
        }
        public static Matrix4x4 CreatePerspective(float width, float height, float near, float far)
        {
            Matrix4x4 result;
            CreatePerspective(width, height, near, far, out result);
            return result;
        }

        public static void CreatePerspectiveFOV(float fov, float aspect, float near, float far, out Matrix4x4 result)
        {
            float num = 1f / (float)Math.Tan(fov * 0.5f);
            float num9 = num / aspect;
            result.M11 = num9;
            result.M12 = result.M13 = result.M14 = 0f;
            result.M22 = num;
            result.M21 = result.M23 = result.M24 = result.M31 = result.M32 = 0f;
            result.M33 = far / (near - far);
            result.M34 = -1;
            result.M41 = result.M42 = result.M44 = 0;
            result.M43 = (near * far) / (near - far);
        }
        public static Matrix4x4 CreatePerspectiveFOV(float fov, float aspect, float near, float far)
        {
            Matrix4x4 result;
            CreatePerspectiveFOV(fov, aspect, near, far, out result);
            return result;
        }

        public static void CreateTransform(ref Vector3 translate, ref Quaternion rotation, ref Vector3 scale, out Matrix4x4 result)
        {
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            result.M11 = 1f - (2f * (yy + zz));
            result.M12 = 2f * (xy + zw);
            result.M13 = 2f * (zx - yw);
            result.M21 = 2f * (xy - zw);
            result.M22 = 1f - (2f * (zz + xx));
            result.M23 = 2f * (yz + xw);
            result.M31 = 2f * (zx + yw);
            result.M32 = 2f * (yz - xw);
            result.M33 = 1f - (2f * (yy + xx));

            // Position
            result.M41 = translate.X;
            result.M42 = translate.Y;
            result.M43 = translate.Z;

            // Scale
            if (scale.X != 1f)
            {
                result.M11 *= scale.X;
                result.M12 *= scale.X;
                result.M13 *= scale.X;
            }
            if (scale.Y != 1f)
            {
                result.M21 *= scale.Y;
                result.M22 *= scale.Y;
                result.M23 *= scale.Y;
            }
            if (scale.Z != 1f)
            {
                result.M31 *= scale.Z;
                result.M32 *= scale.Z;
                result.M33 *= scale.Z;
            }

            result.M14 = 0f;
            result.M24 = 0f;
            result.M34 = 0f;
            result.M44 = 1f;
        }

        public static void CreateShadow(ref Vector3 light, ref Plane plane, out Matrix4x4 result)
        {
            float dot = (plane.Normal.X * light.X) + (plane.Normal.Y * light.Y) + (plane.Normal.Z * light.Z);
            float x = -plane.Normal.X;
            float y = -plane.Normal.Y;
            float z = -plane.Normal.Z;
            float d = -plane.Distance;
            result.M11 = (x * light.X) + dot;
            result.M12 = x * light.Y;
            result.M13 = x * light.Z;
            result.M14 = 0;
            result.M21 = y * light.X;
            result.M22 = (y * light.Y) + dot;
            result.M23 = y * light.Z;
            result.M24 = 0;
            result.M31 = z * light.X;
            result.M32 = z * light.Y;
            result.M33 = (z * light.Z) + dot;
            result.M34 = 0;
            result.M41 = d * light.X;
            result.M42 = d * light.Y;
            result.M43 = d * light.Z;
            result.M44 = dot;
        }
        public static Matrix4x4 CreateShadow(Vector3 light, Plane plane)
        {
            Matrix4x4 result;
            CreateShadow(ref light, ref plane, out result);
            return result;
        }

        public static void CreateReflection(ref Plane value, out Matrix4x4 result)
        {
            Plane plane;
            Plane.Normalize(ref value, out plane);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float num3 = -2f * x;
            float num2 = -2f * y;
            float num = -2f * z;
            result.M11 = (num3 * x) + 1f;
            result.M12 = num2 * x;
            result.M13 = num * x;
            result.M14 = 0;
            result.M21 = num3 * y;
            result.M22 = (num2 * y) + 1;
            result.M23 = num * y;
            result.M24 = 0;
            result.M31 = num3 * z;
            result.M32 = num2 * z;
            result.M33 = (num * z) + 1;
            result.M34 = 0;
            result.M41 = num3 * plane.Distance;
            result.M42 = num2 * plane.Distance;
            result.M43 = num * plane.Distance;
            result.M44 = 1;
        }
        public static Matrix4x4 CreateReflection(Plane plane)
        {
            Matrix4x4 result;
            CreateReflection(ref plane, out result);
            return result;
        }

        public static bool operator ==(Matrix4x4 a, Matrix4x4 b)
        {
            return a.Equals(ref b);
        }
        public static bool operator !=(Matrix4x4 a, Matrix4x4 b)
        {
            return !a.Equals(ref b);
        }
        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 m;
            Multiply(ref a, ref b, out m);
            return m;
        }
    }
}

