namespace Rise
{
    public class AtlasImage
    {
        public Atlas Atlas { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public int TrimWidth { get; private set; }
        public int TrimHeight { get; private set; }

        Quad uv;

        internal AtlasImage(Atlas atlas, ref string name, int w, int h, int ox, int oy, int tw, int th, ref Rectangle uvRect, bool rotate90)
        {
            Atlas = atlas;
            Name = name;
            Width = w;
            Height = h;
            OffsetX = ox;
            OffsetY = oy;
            TrimWidth = tw;
            TrimHeight = th;

            if (rotate90)
            {
                uv.A = uvRect.TopRight;
                uv.B = uvRect.BottomRight;
                uv.C = uvRect.BottomLeft;
                uv.D = uvRect.TopLeft;
            }
            else
            {
                uv.A = uvRect.TopLeft;
                uv.B = uvRect.TopRight;
                uv.C = uvRect.BottomRight;
                uv.D = uvRect.BottomLeft;
            }
        }

        public void GetUVs(out Vector2 a, out Vector2 b, out Vector2 c, out Vector2 d)
        {
            a = uv.A;
            b = uv.B;
            c = uv.C;
            d = uv.D;
        }
    }
}
