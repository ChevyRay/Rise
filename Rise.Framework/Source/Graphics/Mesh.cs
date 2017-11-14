using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rise.OpenGL;
namespace Rise
{
    public class Mesh : ResourceHandle
    {
        const int vertexSize = 24;

        internal uint vaoID;
        internal uint arrayID;
        internal uint elementID;
        Vertex[] vertices;
        int[] indices;
        int vertexCount;
        int indexCount;
        internal int uploadedVertexCount;
        internal int uploadedIndexCount;
        //internal bool dirty = true;

        public int VertexCount { get { return vertexCount; } }
        public int IndexCount { get { return indexCount; } }

        public Mesh(int vertexCapacity, int indexCapacity)
        {
            vertices = new Vertex[vertexCapacity];
            indices = new int[indexCapacity];

            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            arrayID = GL.GenBuffer();
            elementID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.Array, arrayID);
            GL.BindBuffer(BufferTarget.ElementArray, elementID);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(0, 2, VertexType.Float, false, vertexSize, new IntPtr(0));
            GL.VertexAttribPointer(1, 2, VertexType.Float, false, vertexSize, new IntPtr(8));
            GL.VertexAttribPointer(2, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(16));
            GL.VertexAttribPointer(3, 4, VertexType.UnsignedByte, true, vertexSize, new IntPtr(20));

            GL.BindBuffer(BufferTarget.ElementArray, 0);
            GL.BindBuffer(BufferTarget.Array, 0);
            GL.BindVertexArray(0);
        }
        public Mesh() : this(4, 8)
        {

        }

        protected override void Dispose()
        {
            GL.DeleteVertexArray(vaoID);
            GL.DeleteBuffer(arrayID);
            GL.DeleteBuffer(elementID);
        }

        public void UpdateVertices()
        {
            GL.BindBuffer(BufferTarget.Array, arrayID);
            GL.BufferData(BufferTarget.Array, vertexSize * vertexCount, vertices, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.Array, 0);
            uploadedVertexCount = vertexCount;
            //dirty = true;
        }

        public void UpdateIndices()
        {
            GL.BindBuffer(BufferTarget.ElementArray, elementID);
            GL.BufferData(BufferTarget.ElementArray, sizeof(int) * indexCount, indices, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArray, 0);
            uploadedIndexCount = indexCount;
            //dirty = true;
        }

        public void Update()
        {
            UpdateVertices();
            UpdateIndices();
        }

        public void Clear()
        {
            vertexCount = 0;
            indexCount = 0;
        }

        public void GetVertex(int i, out Vertex result)
        {
            vertices[i].CopyTo(out result);
        }
        public Vertex GetVertex(int i)
        {
            return vertices[i];
        }

        public void SetVertex(int i, ref Vertex vert)
        {
            vert.CopyTo(out vertices[i]);
        }

        public void SetVertices(Vertex[] verts, bool copy)
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

        public void AddVertex(ref Vertex vert)
        {
            if (vertexCount == vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            vert.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertex(Vertex vert)
        {
            AddVertex(ref vert);
        }

        public void AddVertices(ref Vertex a, ref Vertex b)
        {
            while (vertexCount + 2 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex a, ref Vertex b, ref Vertex c)
        {
            while (vertexCount + 3 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(ref Vertex a, ref Vertex b, ref Vertex c, ref Vertex d)
        {
            while (vertexCount + 4 > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            a.CopyTo(out vertices[vertexCount++]);
            b.CopyTo(out vertices[vertexCount++]);
            c.CopyTo(out vertices[vertexCount++]);
            d.CopyTo(out vertices[vertexCount++]);
        }
        public void AddVertices(Vertex[] verts)
        {
            while (vertexCount + verts.Length > vertices.Length)
                Array.Resize(ref vertices, vertexCount * 2);
            Array.Copy(verts, 0, vertices, vertexCount, verts.Length);
        }

        public int GetIndex(int i)
        {
            return indices[i];
        }

        public void SetIndex(int i, int ind)
        {
            indices[i] = ind;
        }

        public void SetIndices(int[] inds, bool copy)
        {
            indexCount = inds.Length;
            if (copy)
            {
                if (indices.Length < indexCount)
                    Array.Resize(ref indices, indexCount);
                Array.Copy(inds, indices, indexCount);
            }
            else
                indices = inds;
        }

        public void AddIndex(int ind)
        {
            if (indexCount == indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = ind;
        }

        public void AddIndices(int a, int b)
        {
            while (indexCount + 2 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
        }
        public void AddIndices(int a, int b, int c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
        }
        public void AddIndices(int a, int b, int c, int d)
        {
            while (indexCount + 4 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
            indices[indexCount++] = d;
        }
        public void AddIndices(int a, int b, int c, int d, int e)
        {
            while (indexCount + 5 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
            indices[indexCount++] = d;
            indices[indexCount++] = e;
        }
        public void AddIndices(int a, int b, int c, int d, int e, int f)
        {
            while (indexCount + 6 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
            indices[indexCount++] = d;
            indices[indexCount++] = e;
            indices[indexCount++] = f;
        }
        public void AddIndices(int[] inds)
        {
            while (indexCount + inds.Length > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            Array.Copy(inds, 0, indices, indexCount, inds.Length);
        }

        public void AddTriangle(ref Vertex a, ref Vertex b, ref Vertex c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            int i = (int)vertexCount;
            indices[indexCount++] = i;
            indices[indexCount++] = i + 1;
            indices[indexCount++] = i + 2;

            AddVertices(ref a, ref b, ref c);
        }
        public void AddTriangle(Vertex a, Vertex b, Vertex c)
        {
            AddTriangle(ref a, ref b, ref c);
        }

        public void AddQuad(ref Vertex a, ref Vertex b, ref Vertex c, ref Vertex d)
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
        public void AddQuad(Vertex a, Vertex b, Vertex c, Vertex d)
        {
            AddQuad(ref a, ref b, ref c, ref d);
        }

        public void AddRect(ref Rectangle rect, Vector2 texMin, Vector2 texMax, Color mul, Color add)
        {
            Vertex a, b, c, d;
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
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax, Color mul, Color add)
        {
            AddRect(ref rect, texMin, texMax, mul, add);
        }
        public void AddRect(Rectangle rect, Vector2 texMin, Vector2 texMax)
        {
            AddRect(ref rect, texMin, texMax, Color.White, Color.Transparent);
        }
    }
}
