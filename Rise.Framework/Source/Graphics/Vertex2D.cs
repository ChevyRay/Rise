using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex2D : ICopyable<Vertex2D>
    {
        public Vector2 Pos;
        public Vector2 Tex;
        public Color4 Col;
        public byte Mult;
        public byte Wash;
        public byte Veto;

        public Vertex2D(Vector2 pos, Vector2 tex, Color4 col, byte mult, byte wash, byte veto)
        {
            Pos = pos;
            Tex = tex;
            Col = col;
            Mult = mult;
            Wash = wash;
            Veto = veto;
        }
        public Vertex2D(Vector2 pos, Vector2 tex, Color4 col)
            : this(pos, tex, col, 255, 0, 0)
        {
            
        }
        public Vertex2D(Vector2 pos, Vector2 tex)
            : this(pos, tex, Color4.White, 255, 0, 0)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(out Vertex2D other)
        {
            other.Pos = Pos;
            other.Tex = Tex;
            other.Col = Col;
            other.Mult = Mult;
            other.Wash = Wash;
            other.Veto = Veto;
        }
    }
}
