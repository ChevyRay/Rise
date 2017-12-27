using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex3D : ICopyable<Vertex3D>
    {
        public Vector3 Pos;
        public Vector3 Nor;
        public Vector2 Tex;
        public Color Col;

        public Vertex3D(Vector3 pos, Vector3 nor, Vector2 tex, Color col)
        {
            Pos = pos;
            Nor = nor;
            Tex = tex;
            Col = col;
        }
        public Vertex3D(Vector3 pos, Vector3 nor, Vector2 tex)
            : this(pos, nor, tex, Color.White)
        {

        }
        public Vertex3D(Vector3 pos, Vector3 nor, Color col)
            : this(pos, nor, Vector2.Zero, col)
        {

        }
        public Vertex3D(Vector3 pos, Vector3 nor)
            : this(pos, nor, Vector2.Zero, Color.White)
        {

        }

        public void CopyTo(out Vertex3D other)
        {
            other.Pos = Pos;
            other.Nor = Nor;
            other.Tex = Tex;
            other.Col = Col;
        }
    }
}
