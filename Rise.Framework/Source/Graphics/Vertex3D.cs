using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex3D : ICopyable<Vertex3D>
    {
        public Vector3 Pos;
        public Vector3 Nor;
        public Vector2 Tex;
        public Color Mul;
        public Color Add;

        public Vertex3D(Vector3 pos, Vector3 nor, Vector2 tex, Color mul, Color add)
        {
            Pos = pos;
            Nor = nor;
            Tex = tex;
            Mul = mul;
            Add = add;
        }
        public Vertex3D(Vector3 pos, Vector3 nor, Vector2 tex, Color mul)
            : this(pos, nor, tex, mul, Color.Transparent)
        {

        }
        public Vertex3D(Vector3 pos, Vector3 nor, Vector2 tex)
            : this(pos, nor, tex, Color.White, Color.Transparent)
        {

        }
        public Vertex3D(Vector3 pos, Vector3 nor, Color add)
            : this(pos, nor, Vector2.Zero, Color.Transparent, add)
        {

        }
        public Vertex3D(Vector3 pos, Vector3 nor)
            : this(pos, nor, Vector2.Zero, Color.White, Color.Transparent)
        {

        }

        public void CopyTo(out Vertex3D other)
        {
            other.Pos = Pos;
            other.Nor = Nor;
            other.Tex = Tex;
            other.Mul = Mul;
            other.Add = Add;
        }
    }
}
