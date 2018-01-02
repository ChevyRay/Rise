using System;
namespace Rise
{
    public class Frustum
    {
        Matrix4x4 matrix;
        Plane[] planes = new Plane[6];
        Vector3[] corners = new Vector3[8];

        public Frustum(Matrix4x4 matrix)
        {
            this.matrix = matrix;
            Update();
        }

        public Plane Near
        {
            get { return planes[0]; }
        }

        public Plane Far
        {
            get { return planes[1]; }
        }

        public Plane Left
        {
            get { return planes[2]; }
        }

        public Plane Right
        {
            get { return planes[3]; }
        }

        public Plane Top
        {
            get { return planes[4]; }
        }

        public Plane Bottom
        {
            get { return planes[5]; }
        }

        public Matrix4x4 Matrix
        {
            get { return matrix; }
            set
            {
                matrix = value;
                Update();
            }
        }

        public bool Contains(Vector3 point)
        {
            for (int i = 0; i < 6; ++i)
                if (planes[i].InFront(ref point))
                    return false;
            return true;
        }
        public bool Contains(ref BoundingSphere sphere)
        {
            for (int i = 0; i < 6; ++i)
                if (planes[i].InFront(ref sphere))
                    return false;
            return true;
        }
        public bool Contains(ref BoundingBox box)
        {
            for (int i = 0; i < 6; ++i)
                if (planes[i].InFront(ref box))
                    return false;
            return true;
        }

        static void GetPoint(ref Plane a, ref Plane b, ref Plane c, out Vector3 p)
        {
            Vector3 v1, v2, v3, cross;
            Vector3.Cross(ref b.Normal, ref c.Normal, out cross);
            var f = -Vector3.Dot(ref a.Normal, ref cross);
            Vector3.Cross(ref b.Normal, ref c.Normal, out cross);
            Vector3.Multiply(ref cross, a.Distance, out v1);
            Vector3.Cross(ref c.Normal, ref a.Normal, out cross);
            Vector3.Multiply(ref cross, b.Distance, out v2);
            Vector3.Cross(ref a.Normal, ref b.Normal, out cross);
            Vector3.Multiply(ref cross, c.Distance, out v3);
            p.X = (v1.X + v2.X + v3.X) / f;
            p.Y = (v1.Y + v2.Y + v3.Y) / f;
            p.Z = (v1.Z + v2.Z + v3.Z) / f;
        }

        static void NormalizePlane(ref Plane p)
        {
            float factor = 1f / p.Normal.Length;
            p.Normal.X *= factor;
            p.Normal.Y *= factor;
            p.Normal.Z *= factor;
            p.Distance *= factor;
        }

        void Update()
        {            
            planes[0] = new Plane(-matrix.M13, -matrix.M23, -matrix.M33, -matrix.M43);
            planes[1] = new Plane(matrix.M13 - matrix.M14, matrix.M23 - matrix.M24, matrix.M33 - matrix.M34, matrix.M43 - matrix.M44);
            planes[2] = new Plane(-matrix.M14 - matrix.M11, -matrix.M24 - matrix.M21, -matrix.M34 - matrix.M31, -matrix.M44 - matrix.M41);
            planes[3] = new Plane(matrix.M11 - matrix.M14, matrix.M21 - matrix.M24, matrix.M31 - matrix.M34, matrix.M41 - matrix.M44);
            planes[4] = new Plane(matrix.M12 - matrix.M14, matrix.M22 - matrix.M24, matrix.M32 - matrix.M34, matrix.M42 - matrix.M44);
            planes[5] = new Plane(-matrix.M14 - matrix.M12, -matrix.M24 - matrix.M22, -matrix.M34 - matrix.M32, -matrix.M44 - matrix.M42);
            for (int i = 0; i < 6; ++i)
                NormalizePlane(ref planes[i]);
            GetPoint(ref planes[0], ref planes[2], ref planes[4], out corners[0]);
            GetPoint(ref planes[0], ref planes[3], ref planes[4], out corners[1]);
            GetPoint(ref planes[0], ref planes[3], ref planes[5], out corners[2]);
            GetPoint(ref planes[0], ref planes[2], ref planes[5], out corners[3]);
            GetPoint(ref planes[1], ref planes[2], ref planes[4], out corners[4]);
            GetPoint(ref planes[1], ref planes[3], ref planes[4], out corners[5]);
            GetPoint(ref planes[1], ref planes[3], ref planes[5], out corners[6]);
            GetPoint(ref planes[1], ref planes[2], ref planes[5], out corners[7]);
        }
    }
}
