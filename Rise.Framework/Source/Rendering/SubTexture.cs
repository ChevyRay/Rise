using System;
namespace Rise
{
    public class SubTexture
    {
        public Texture Texture { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public int OffsetX { get; protected set; }
        public int OffsetY { get; protected set; }
        public int TrimWidth { get; protected set; }
        public int TrimHeight { get; protected set; }

        protected Quad uv;

        public SubTexture(Texture texture, RectangleI rect, int offsetX, int offsetY, int trimWidth, int trimHeight)
        {
            Texture = texture;
            Width = rect.W;
            Height = rect.H;
            OffsetX = offsetX;
            OffsetY = offsetY;
            TrimWidth = trimWidth;
            TrimHeight = trimHeight;

            var size = new Vector2(texture.Width, texture.Height);
            uv.A = rect.TopLeft / size;
            uv.B = rect.TopRight / size;
            uv.C = rect.BottomRight / size;
            uv.D = rect.BottomLeft / size;
        }
        public SubTexture(Texture texture, RectangleI rect)
            : this(texture, rect, 0, 0, rect.W, rect.H)
        {

        }
        protected SubTexture()
        {

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
