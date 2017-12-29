using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2D : ICopyable<Vertex2D>
    {
        public Vector2 Pos;
        public Vector2 Tex;
        public Color4 Mul;
        public Color4 Add;

        public Vertex2D(Vector2 pos, Vector2 tex, Color4 mul, Color4 add)
        {
            Pos = pos;
            Tex = tex;
            Mul = mul;
            Add = add;
        }
        public Vertex2D(Vector2 pos, Vector2 tex, Color4 mul)
            : this(pos, tex, mul, Color4.Transparent)
        {
            
        }
        public Vertex2D(Vector2 pos, Vector2 tex)
            : this(pos, tex, Color4.White, Color4.Transparent)
        {

        }
        public Vertex2D(Vector2 pos, Color4 add)
            : this(pos, Vector2.Zero, Color4.Transparent, add)
        {

        }

        public void CopyTo(out Vertex2D other)
        {
            other.Pos = Pos;
            other.Tex = Tex;
            other.Mul = Mul;
            other.Add = Add;
        }
    }
}
