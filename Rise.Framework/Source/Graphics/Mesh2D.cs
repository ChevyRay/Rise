using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rise.OpenGL;
namespace Rise
{
    public class Mesh2D : Mesh
    {
        const int vertexSize = 24;

        Vertex2D[] vertices;
        int vertexCount;

        public override int VertexCount { get { return vertexCount; } }

        public Mesh2D(int vertexCapacity, int indexCapacity) : base(indexCapacity)
        {
            vertices = new Vertex2D[vertexCapacity];
        }
        public Mesh2D() : this(4, 6)
        {

        }

        public static Mesh2D CreateRect(Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 mul, Color4 add)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, texMin, texMax, mul, add);
            mesh.Update();
            return mesh;
        }
        public static Mesh2D CreateRect(Rectangle rect, Vector2 texMin, Vector2 texMax)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, texMin, texMax, Color4.White, Color4.Transparent);
            mesh.Update();
            return mesh;
        }
        public static Mesh2D CreateRect(Rectangle rect)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, Vector2.Zero, Vector2.One, Color4.White, Color4.Transparent);
            mesh.Update();
            return mesh;
        }

        internal override void AssignAttributes()
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(0, 2, VertexType.Float, false, vertexSize, new IntPtr(0));
            GL.VertexAttribPointer(1, 2, VertexType.Float, false, vertexSize, new IntPtr(8));
            GL.VertexAttribPointer(2, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(16));
            GL.VertexAttribPointer(3, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(20));
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

        public void GetVertex(int i, out Vertex2D result)
        {
            vertices[i].CopyTo(out result);
        }
        public Vertex2D GetVertex(int i)
        {
            return vertices[i];
        }

        public void SetVertex(int i, ref Vertex2D vert)
        {
            vert.CopyTo(out vertices[i]);
        }

        public void SetVertices(Vertex2D[] verts, bool copy)
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

        public void AddVertex(ref Vertex2D vert)
        {
            if (vertexCount == vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            vert.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertex(Vertex2D vert)
        {
            AddVertex(ref vert);
        }

        public void AddVertices(ref Vertex2D a, ref Vertex2D b)
        {
            while (vertexCount + 2 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c)
        {
            while (vertexCount + 3 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c, ref Vertex2D d)
        {
            while (vertexCount + 4 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
            d.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(Vertex2D[] verts)
        {
            while (vertexCount + verts.Length > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            Array.Copy(verts, 0, vertices, vertexCount, verts.Length);
        }

        public void AddTriangle(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = (int)vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;

            AddVertices(ref a, ref b, ref c);
        }
        public void AddTriangle(Vertex2D a, Vertex2D b, Vertex2D c)
        {
            AddTriangle(ref a, ref b, ref c);
        }

        public void AddQuad(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c, ref Vertex2D d)
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
        public void AddQuad(Vertex2D a, Vertex2D b, Vertex2D c, Vertex2D d)
        {
            AddQuad(ref a, ref b, ref c, ref d);
        }

        public void AddRect(ref Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 mul, Color4 add)
        {
            Vertex2D a, b, c, d;
            a.Pos = rect.TopLeft;
            b.Pos = rect.TopRight;
            c.Pos = rect.BottomRight;
            d.Pos = rect.BottomLeft;
            a.Tex.X = texMin.X;
            a.Tex.Y = texMin.Y;
            b.Tex.X = texMax.X;
            b.Tex.Y = texMin.Y;
            c.Tex.X = texMax.X;
            c.Tex.Y = texMax.Y;
            d.Tex.X = texMin.X;
            d.Tex.Y = texMax.Y;
            a.Mul = b.Mul = c.Mul = d.Mul = mul;
            a.Add = b.Add = c.Add = d.Add = add;
            AddQuad(ref a, ref b, ref c, ref d);
        }
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 mul, Color4 add)
        {
            AddRect(ref rect, texMin, texMax, mul, add);
        }
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax)
        {
            AddRect(ref rect, texMin, texMax, Color4.White, Color4.Transparent);
        }
    }
}
