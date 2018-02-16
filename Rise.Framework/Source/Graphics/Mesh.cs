using System;
using System.Runtime.CompilerServices;
using Rise.OpenGL;
namespace Rise
{
    public abstract class Mesh : ResourceHandle
    {
        internal uint vaoID;
        internal uint arrayID;
        internal uint elementID;
        protected int[] indices;
        protected int indexCount;
        internal int uploadedVertexCount;
        internal int uploadedIndexCount;
        internal bool dirty;

        public abstract int VertexCount { get; }
        public int IndexCount { get { return indexCount; } }

        internal Mesh(int indexCapacity)
        {
            indices = new int[indexCapacity];

            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            arrayID = GL.GenBuffer();
            elementID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.Array, arrayID);
            GL.BindBuffer(BufferTarget.ElementArray, elementID);
        }

        protected override void Dispose()
        {
            GL.DeleteVertexArray(vaoID);
            GL.DeleteBuffer(arrayID);
            GL.DeleteBuffer(elementID);
        }

        public abstract void UpdateVertices();

        public void UpdateIndices()
        {
            GL.BindBuffer(BufferTarget.ElementArray, elementID);
            GL.BufferData(BufferTarget.ElementArray, sizeof(int) * indexCount, indices, BufferUsage.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArray, 0);
            uploadedIndexCount = indexCount;
            dirty = true;
        }

        public void Update()
        {
            UpdateVertices();
            UpdateIndices();
        }

        internal abstract void ClearVertices();
        public void Clear()
        {
            ClearVertices();
            indexCount = 0;
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

        public void SetIndexCount(int count)
        {
            if (indices.Length < count)
            {
                int cap = indices.Length;
                while (cap < count)
                    cap *= 2;
                Array.Resize(ref indices, cap);
            }
            indexCount = count;
        }

        public void SetTriangle(int i, int a, int b, int c)
        {
            i *= 3;
            indices[i++] = a;
            indices[i++] = b;
            indices[i] = c;
        }

        public void AddIndex(int ind)
        {
            if (indexCount == indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = ind;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddIndices(int a, int b)
        {
            while (indexCount + 2 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddIndices(int a, int b, int c)
        {
            while (indexCount + 3 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddIndices(int a, int b, int c, int d)
        {
            while (indexCount + 4 > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            indices[indexCount++] = a;
            indices[indexCount++] = b;
            indices[indexCount++] = c;
            indices[indexCount++] = d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddIndices(int[] inds)
        {
            while (indexCount + inds.Length > indices.Length)
                Array.Resize(ref indices, indexCount * 2);
            Array.Copy(inds, 0, indices, indexCount, inds.Length);
        }
    }
}
