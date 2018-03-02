using System;
using System.Runtime.CompilerServices;
using Rise.OpenGL;
namespace Rise
{
    public class Mesh2D : Mesh
    {
        const int vertexSize = 23;

        Vertex2D[] vertices;
        int vertexCount;

        public override int VertexCount { get { return vertexCount; } }

        public Mesh2D(int vertexCapacity, int indexCapacity) : base(indexCapacity)
        {
            vertices = new Vertex2D[vertexCapacity];

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(0, 2, VertexType.Float, false, vertexSize, new IntPtr(0));
            GL.VertexAttribPointer(1, 2, VertexType.Float, false, vertexSize, new IntPtr(8));
            GL.VertexAttribPointer(2, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(16));
            GL.VertexAttribPointer(3, 1, VertexType.UnsignedByte, true, vertexSize, new IntPtr(20));
            GL.VertexAttribPointer(4, 1, VertexType.UnsignedByte, true, vertexSize, new IntPtr(21));
            GL.VertexAttribPointer(5, 1, VertexType.UnsignedByte, true, vertexSize, new IntPtr(22));

            GL.BindBuffer(BufferTarget.ElementArray, 0);
            GL.BindBuffer(BufferTarget.Array, 0);
            GL.BindVertexArray(0);
        }
        public Mesh2D() : this(4, 6)
        {

        }

        public static Mesh2D CreateRect(Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 col)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, texMin, texMax, col);
            mesh.Update();
            return mesh;
        }
        public static Mesh2D CreateRect(Rectangle rect, Vector2 texMin, Vector2 texMax)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, texMin, texMax, Color4.White);
            mesh.Update();
            return mesh;
        }
        public static Mesh2D CreateRect(Rectangle rect)
        {
            var mesh = new Mesh2D(4, 6);
            mesh.AddRect(ref rect, Vector2.Zero, Vector2.One, Color4.White);
            mesh.Update();
            return mesh;
        }

        public override void UpdateVertices()
        {
            GL.BindBuffer(BufferTarget.Array, arrayID);
            GL.BufferData(BufferTarget.Array, vertexSize * vertexCount, vertices, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.Array, 0);
            uploadedVertexCount = vertexCount;
            dirty = true;
        }

        internal override void ClearVertices()
        {
            vertexCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetVertex(int i, out Vertex2D result)
        {
            vertices[i].CopyTo(out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vertex2D GetVertex(int i)
        {
            return vertices[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertex(ref Vertex2D vert)
        {
            if (vertexCount == vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            vert.CopyTo(out vertices[vertexCount++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertex(Vertex2D vert)
        {
            AddVertex(ref vert);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertices(ref Vertex2D a, ref Vertex2D b)
        {
            while (vertexCount + 2 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertices(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c)
        {
            while (vertexCount + 3 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertices(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c, ref Vertex2D d)
        {
            while (vertexCount + 4 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
            d.CopyTo(out vertices[vertexCount++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddVertices(Vertex2D[] verts)
        {
            while (vertexCount + verts.Length > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            Array.Copy(verts, 0, vertices, vertexCount, verts.Length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTriangle(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;

            AddVertices(ref a, ref b, ref c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTriangle(Vertex2D a, Vertex2D b, Vertex2D c)
        {
            AddTriangle(ref a, ref b, ref c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(ref Vertex2D a, ref Vertex2D b, ref Vertex2D c, ref Vertex2D d)
        {
            while (indexCount + 6 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 2;
            indices[indexCount++] = i + 3;

            //AddVertices(ref a, ref b, ref c, ref d);
            while (vertexCount + 4 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
            d.CopyTo(out vertices[vertexCount++]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(Vertex2D a, Vertex2D b, Vertex2D c, Vertex2D d)
        {
            AddQuad(ref a, ref b, ref c, ref d);
        }

        static Vertex2D v0 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 1f, 0f, 0f);
        static Vertex2D v1 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 1f, 0f, 0f);
        static Vertex2D v2 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 1f, 0f, 0f);
        static Vertex2D v3 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 1f, 0f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRect(ref Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 col)
        {
            v0.Pos = rect.TopLeft;
            v1.Pos = rect.TopRight;
            v2.Pos = rect.BottomRight;
            v3.Pos = rect.BottomLeft;
            v0.Tex.X = texMin.X;
            v0.Tex.Y = texMin.Y;
            v1.Tex.X = texMax.X;
            v1.Tex.Y = texMin.Y;
            v2.Tex.X = texMax.X;
            v2.Tex.Y = texMax.Y;
            v3.Tex.X = texMin.X;
            v3.Tex.Y = texMax.Y;
            v0.Col = v1.Col = v2.Col = v3.Col = col;
            AddQuad(ref v0, ref v1, ref v2, ref v3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax, Color4 col)
        {
            AddRect(ref rect, texMin, texMax, col);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax)
        {
            AddRect(ref rect, texMin, texMax, Color4.White);
        }
    }
}
