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

        public static Mesh3D CreateCube(Vector3 size, Color4 color)
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
        public static Mesh3D CreateCube(Vector3 size)
        {
            return CreateCube(size, Color4.White);
        }
        public static Mesh3D CreateCube(float size, Color4 color)
        {
            return CreateCube(new Vector3(size), Color4.White);
        }
        public static Mesh3D CreateCube(float size)
        {
            return CreateCube(new Vector3(size), Color4.White);
        }
    }
}
