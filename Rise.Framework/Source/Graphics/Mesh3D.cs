using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rise.OpenGL;
namespace Rise
{
    public class Mesh3D : Mesh
    {
        const int vertexSize = 36;

        Vertex3D[] vertices;
        int vertexCount;

        public override int VertexCount { get { return vertexCount; } }

        public Mesh3D(int vertexCapacity, int indexCapacity) : base(indexCapacity)
        {
            vertices = new Vertex3D[vertexCapacity];
        }
        public Mesh3D() : this(4, 6)
        {

        }

        internal override void AssignAttributes()
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(0, 3, VertexType.Float, false, vertexSize, new IntPtr(0));
            GL.VertexAttribPointer(1, 3, VertexType.Float, false, vertexSize, new IntPtr(12));
            GL.VertexAttribPointer(2, 2, VertexType.Float, false, vertexSize, new IntPtr(24));
            GL.VertexAttribPointer(3, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(32));
        }

        public override void UpdateVertices()
        {
            GL.BindBuffer(BufferTarget.Array, arrayID);
            GL.BufferData(BufferTarget.Array, vertexSize * vertexCount, vertices, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.Array, 0);
            uploadedVertexCount = vertexCount;
        }

        internal override void ClearVertices()
        {
            vertexCount = 0;
        }

        public void GetVertex(int i, out Vertex3D result)
        {
            vertices[i].CopyTo(out result);
        }
        public Vertex3D GetVertex(int i)
        {
            return vertices[i];
        }

        public void SetVertex(int i, ref Vertex3D vert)
        {
            vert.CopyTo(out vertices[i]);
        }
        public void SetVertex(int i, Vertex3D vert)
        {
            vert.CopyTo(out vertices[i]);
        }

        public void SetVertices(Vertex3D[] verts, bool copy)
        {
            vertexCount = verts.Length;
            if (copy)
            {
                if (vertices.Length < vertexCount)
                    Array.Resize(ref vertices, vertexCount);
                Array.Copy(verts, vertices, vertexCount);
            }
            else
                vertices = verts;
        }

        public void SetVertexCount(int count)
        {
            if (vertices.Length < count)
            {
                int cap = vertices.Length;
                while (cap < count)
                    cap *= 2;
                Array.Resize(ref vertices, cap);
            }
            vertexCount = count;
        }

        public void SetPosition(int i, Vector3 val)
        {
            vertices[i].Pos = val;
        }

        public void SetNormal(int i, Vector3 val)
        {
            vertices[i].Nor = val;
        }

        public void SetTexCoord(int i, Vector2 val)
        {
            vertices[i].Tex = val;
        }

        public void SetColor(int i, Color4 val)
        {
            vertices[i].Col = val;
        }

        public void AddVertex(ref Vertex3D vert)
        {
            if (vertexCount == vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            vert.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertex(Vertex3D vert)
        {
            AddVertex(ref vert);
        }

        public void AddVertices(ref Vertex3D a, ref Vertex3D b)
        {
            while (vertexCount + 2 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex3D a, ref Vertex3D b, ref Vertex3D c)
        {
            while (vertexCount + 3 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex3D a, ref Vertex3D b, ref Vertex3D c, ref Vertex3D d)
        {
            while (vertexCount + 4 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
            d.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(Vertex3D[] verts)
        {
            while (vertexCount + verts.Length > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            Array.Copy(verts, 0, vertices, vertexCount, verts.Length);
        }

        public void AddTriangle(ref Vertex3D a, ref Vertex3D b, ref Vertex3D c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = (int)vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;

            AddVertices(ref a, ref b, ref c);
        }
        public void AddTriangle(Vertex3D a, Vertex3D b, Vertex3D c)
        {
            AddTriangle(ref a, ref b, ref c);
        }

        public void AddQuad(ref Vertex3D a, ref Vertex3D b, ref Vertex3D c, ref Vertex3D d)
        {
            while (indexCount + 6 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = (int)vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 2;
            indices[indexCount++] = i + 3;

            AddVertices(ref a, ref b, ref c, ref d);
        }
        public void AddQuad(Vertex3D a, Vertex3D b, Vertex3D c, Vertex3D d)
        {
            AddQuad(ref a, ref b, ref c, ref d);
        }

        public void CalculateNormals()
        {
            //Zero all normals
            for (int i = 0; i < vertexCount; ++i)
                vertices[i].Nor = Vector3.Zero;

            //For each triangle, add the normal of that face to each of its vertices
            Vector3 ab, ac, n;
            for (int i = 0; i < indexCount; i += 3)
            {
                Vector3.Subtract(ref vertices[indices[i + 1]].Pos, ref vertices[indices[i]].Pos, out ab);
                Vector3.Subtract(ref vertices[indices[i + 2]].Pos, ref vertices[indices[i]].Pos, out ac);
                Vector3.Cross(ref ab, ref ac, out n);
                Vector3.Normalize(ref n, out n);
                vertices[indices[i]].Nor += n;
                vertices[indices[i + 1]].Nor += n;
                vertices[indices[i + 2]].Nor += n;
            }

            //Normalize the vertices so their normals blend between the vertices they share
            for (int i = 0; i < VertexCount; ++i)
                Vector3.Normalize(ref vertices[i].Nor, out vertices[i].Nor);
        }

        public void GetBoundingBox(out BoundingBox box)
        {
            box.Min = new Vector3(float.MaxValue);
            box.Max = new Vector3(float.MinValue);
            for (int i = 0; i < vertices.Length; ++i)
            {
                Vector3.Min(ref box.Min, ref vertices[i].Pos, out box.Min);
                Vector3.Min(ref box.Min, ref vertices[i].Pos, out box.Min);
            }
        }
        public BoundingBox GetBoundingBox()
        {
            BoundingBox box;
            GetBoundingBox(out box);
            return box;
        }

        public void GetBoundingSphere(out BoundingSphere sphere)
        {
            BoundingBox box;
            GetBoundingBox(out box);
            sphere.Center = box.Center;
            sphere.Radius = Vector3.Distance(ref box.Min, ref sphere.Center);
        }
        public BoundingSphere GetBoundingSphere()
        {
            BoundingSphere sphere;
            GetBoundingSphere(out sphere);
            return sphere;
        }

        public static Mesh3D CreateQuad(float w, float h, Color4 color)
        {
            var mesh = new Mesh3D(4, 6);
            var a = new Vertex3D(new Vector3(w * 0.5f, h * 0.5f), Vector3.Forward, new Vector2(0f, 0f), color);
            var b = new Vertex3D(new Vector3(w * -0.5f, h * 0.5f), Vector3.Forward, new Vector2(1f, 0f), color);
            var c = new Vertex3D(new Vector3(w * -0.5f, h * -0.5f), Vector3.Forward, new Vector2(1f, 1f), color);
            var d = new Vertex3D(new Vector3(w * 0.5f, h * -0.5f), Vector3.Forward, new Vector2(0f, 1f), color);
            mesh.AddQuad(ref a, ref b, ref c, ref d);
            mesh.CalculateNormals();
            mesh.Update();
            return mesh;
        }

        public static Mesh3D CreateBox(Vector3 size, Color4 color)
        {
            var mesh = new Mesh3D(24, 36);
            mesh.SetVertexCount(24);
            mesh.SetIndexCount(36);
            size *= 0.5f;

            mesh.SetVertex(0, new Vertex3D(new Vector3(1, 1, 1) * size, Vector3.Forward, new Vector2(0, 0), color));
            mesh.SetVertex(1, new Vertex3D(new Vector3(-1, 1, 1) * size, Vector3.Forward, new Vector2(1, 0), color));
            mesh.SetVertex(2, new Vertex3D(new Vector3(-1, -1, 1) * size, Vector3.Forward, new Vector2(1, 1), color));
            mesh.SetVertex(3, new Vertex3D(new Vector3(1, -1, 1) * size, Vector3.Forward, new Vector2(0, 1), color));

            mesh.SetVertex(4, new Vertex3D(new Vector3(-1, 1, -1) * size, Vector3.Back, new Vector2(0, 0), color));
            mesh.SetVertex(5, new Vertex3D(new Vector3(1, 1, -1) * size, Vector3.Back, new Vector2(1, 0), color));
            mesh.SetVertex(6, new Vertex3D(new Vector3(1, -1, -1) * size, Vector3.Back, new Vector2(1, 1), color));
            mesh.SetVertex(7, new Vertex3D(new Vector3(-1, -1, -1) * size, Vector3.Back, new Vector2(0, 1), color));

            mesh.SetVertex(8, new Vertex3D(new Vector3(-1, 1, 1) * size, Vector3.Left, new Vector2(0, 0), color));
            mesh.SetVertex(9, new Vertex3D(new Vector3(-1, 1, -1) * size, Vector3.Left, new Vector2(1, 0), color));
            mesh.SetVertex(10, new Vertex3D(new Vector3(-1, -1, -1) * size, Vector3.Left, new Vector2(1, 1), color));
            mesh.SetVertex(11, new Vertex3D(new Vector3(-1, -1, 1) * size, Vector3.Left, new Vector2(0, 1), color));

            mesh.SetVertex(12, new Vertex3D(new Vector3(1, 1, -1) * size, Vector3.Right, new Vector2(0, 0), color));
            mesh.SetVertex(13, new Vertex3D(new Vector3(1, 1, 1) * size, Vector3.Right, new Vector2(1, 0), color));
            mesh.SetVertex(14, new Vertex3D(new Vector3(1, -1, 1) * size, Vector3.Right, new Vector2(1, 1), color));
            mesh.SetVertex(15, new Vertex3D(new Vector3(1, -1, -1) * size, Vector3.Right, new Vector2(0, 1), color));

            mesh.SetVertex(16, new Vertex3D(new Vector3(-1, -1, 1) * size, Vector3.Down, new Vector2(0, 0), color));
            mesh.SetVertex(17, new Vertex3D(new Vector3(-1, -1, -1) * size, Vector3.Down, new Vector2(1, 0), color));
            mesh.SetVertex(18, new Vertex3D(new Vector3(1, -1, -1) * size, Vector3.Down, new Vector2(1, 1), color));
            mesh.SetVertex(19, new Vertex3D(new Vector3(1, -1, 1) * size, Vector3.Down, new Vector2(0, 1), color));

            mesh.SetVertex(20, new Vertex3D(new Vector3(-1, 1, 1) * size, Vector3.Up, new Vector2(0, 0), color));
            mesh.SetVertex(21, new Vertex3D(new Vector3(1, 1, 1) * size, Vector3.Up, new Vector2(1, 0), color));
            mesh.SetVertex(22, new Vertex3D(new Vector3(1, 1, -1) * size, Vector3.Up, new Vector2(1, 1), color));
            mesh.SetVertex(23, new Vertex3D(new Vector3(-1, 1, -1) * size, Vector3.Up, new Vector2(0, 1), color));

            for (int i = 0, f = 0; i < 12; i += 2, f += 4)
            {
                mesh.SetTriangle(i, f, f + 1, f + 2);
                mesh.SetTriangle(i + 1, f, f + 2, f + 3);
            }
            //mesh.CalculateNormals();
            mesh.Update();
            return mesh;
        }
        public static Mesh3D CreateBox(Vector3 size)
        {
            return CreateBox(size, Color4.White);
        }

        public static Mesh3D CreateCube(float size, Color4 color)
        {
            return CreateBox(new Vector3(size), Color4.White);
        }
        public static Mesh3D CreateCube(float size)
        {
            return CreateBox(new Vector3(size), Color4.White);
        }

        public static Mesh3D CreateSphere(float radius, int segmentsW, int segmentsH, Color4 color)
        {
            float tdelta = 360f / segmentsW;
            float pdelta = 180f / segmentsH;
            ++segmentsW;
            ++segmentsH;
            float phi = -90f;
            Vector3 v;

            var mesh = new Mesh3D(24, 36);
            mesh.SetVertexCount(segmentsW * segmentsH);
            mesh.SetIndexCount((segmentsW - 1) * (segmentsH - 1) * 6);

            int n = 0;
            for (int i = 0; i < segmentsH; i++)
            {
                float theta = 0f;
                for (int j = 0; j < segmentsW; j++)
                {
                    v.X = radius * Calc.Cos(phi * Calc.PI / 180f) * Calc.Cos(theta * Calc.PI / 180f);
                    v.Y = radius * Calc.Sin(phi * Calc.PI / 180f);
                    v.Z = radius * Calc.Cos(phi * Calc.PI / 180f) * Calc.Sin(theta * Calc.PI / 180f);
                    mesh.vertices[n].Pos = v;
                    mesh.vertices[n].Nor = v.Normalized;
                    mesh.vertices[n].Tex = new Vector2(theta / 360f, (phi + 90f) / 180f);
                    mesh.vertices[n].Col = color;
                    theta += tdelta;
                    ++n;
                }
                phi += pdelta;
            }

            n = 0;
            for (int i = 0; i < segmentsH - 1; i++)
            {
                for (int j = 0; j < segmentsW - 1; j++)
                {
                    mesh.SetTriangle(n++, (i + 1) * segmentsW + j, (i + 1) * segmentsW + j + 1, i * segmentsW + j + 1);
                    mesh.SetTriangle(n++, i * segmentsW + j + 1, i * segmentsW + j, (i + 1) * segmentsW + j);
                }
            }

            mesh.CalculateNormals();
            mesh.Update();
            return mesh;
        }
        public static Mesh3D CreateSphere(float radius, int segments, Color4 color)
        {
            return CreateSphere(radius, segments, segments, color);
        }
        public static Mesh3D CreateSphere(float radius, int segments)
        {
            return CreateSphere(radius, segments, segments, Color4.White);
        }

        public static Mesh3D CreateCylinder(float radius, float height, int numSegments, bool capped)
        {
            float lastx = 0f;
            float lastz = 0f;
            float lastv = 0f;
            ++numSegments;

            var mesh = new Mesh3D();

            if (capped)
            {
                mesh.AddVertex(new Vertex3D(new Vector3(0, 0 - (height / 2.0f), 0), new Vector3(0f, -1f, 0f), new Vector2(0.5f, 0.5f)));
                mesh.AddVertex(new Vertex3D(new Vector3(0, height - (height / 2.0f), 0), new Vector3(0f, 1f, 0f), new Vector2(0.5f, 0.5f)));
            }

            for (int i = 0; i < numSegments; i++)
            {
                float v = i / (float)(numSegments - 1);
                float pos = (Calc.Tau / (numSegments-1)) * i;
                float x = Calc.Sin(pos);
                float z = Calc.Cos(pos);
                mesh.AddVertex(new Vertex3D(new Vector3(x*radius, 0 - (height / 2.0f), z*radius), new Vector3(x, 0f, z), new Vector2(v, 0f)));
                mesh.AddVertex(new Vertex3D(new Vector3(x*radius, height - (height / 2.0f), z*radius), new Vector3(x, 0f, z), new Vector2(v, 1f)));
                if (capped)
                {
                    mesh.AddVertex(new Vertex3D(new Vector3(x*radius, 0 - (height / 2.0f), z*radius), new Vector3(0f, -1f, 0f), new Vector2((0.5f + z * 0.5f), (0.5f + x * 0.5f))));
                    mesh.AddVertex(new Vertex3D(new Vector3(x*radius, height - (height / 2.0f), z*radius), new Vector3(0f, 1f, 0f), new Vector2((0.5f + z * 0.5f), (0.5f + x * 0.5f))));
                }
                lastx = x;
                lastz = z;          
                lastv = v;
            }


            int vo = 2;
            int vi = 1;
            if (capped)
            {
                vi = 3;
                vo = 6;
            }

            for (int i = 1 ; i <= numSegments - 1; i++)
            {
                mesh.AddIndices(vo, vo - vi, vo - vi - 1);
                mesh.AddIndices(vo, vo + 1, vo - vi);
                vo += 2;

                if (capped)
                {
                    mesh.AddIndices(vo, vo - vi - 1, 0);
                    mesh.AddIndices(1, vo-vi, vo + 1);
                    vo += 2;
                }
            }

            mesh.Update();
            return mesh;
        }
    }
}
