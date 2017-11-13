using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex : ICopyable<Vertex>
    {
        [VertexLayout(0, 2, VertexType.Float, false, 0)]
        public Vector2 Pos;

        [VertexLayout(1, 2, VertexType.Float, false, 8)]
        public Vector2 Tex;

        [VertexLayout(2, 4, VertexType.UnsignedByte, true, 16)]
        public Color Mul;

        [VertexLayout(3, 4, VertexType.UnsignedByte, true, 16)]
        public Color Add;

        public Vertex(Vector2 pos, Vector2 tex, Color mul, Color add)
        {
            Pos = pos;
            Tex = tex;
            Mul = mul;
            Add = add;
        }
        public Vertex(Vector2 pos, Vector2 tex, Color mul)
            : this(pos, tex, mul, Color.Transparent)
        {
            
        }
        public Vertex(Vector2 pos, Vector2 tex)
            : this(pos, tex, Color.White, Color.Transparent)
        {

        }
        public Vertex(Vector2 pos, Color add)
            : this(pos, Vector2.Zero, Color.Transparent, add)
        {

        }

        public void CopyTo(out Vertex other)
        {
            other.Pos = Pos;
            other.Tex = Tex;
            other.Mul = Mul;
            other.Add = Add;
        }
    }
}
