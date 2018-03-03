namespace Rise
{
    public class AtlasImage : SubTexture
    {
        public Atlas Atlas { get; private set; }
        public string Name { get; private set; }

        internal AtlasImage(Atlas atlas, ref string name, int w, int h, int ox, int oy, int tw, int th, ref Rectangle uvRect, bool rotate90)
        {
            Atlas = atlas;
            Texture = atlas.Texture;
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
    }
}
