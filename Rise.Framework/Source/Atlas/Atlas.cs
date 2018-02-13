using System;
using System.Collections.Generic;
namespace Rise
{
    public class Atlas
    {
        public Texture2D Texture { get; private set; }
        public Vector2 WhitePixel { get; internal set; }
        Dictionary<string, AtlasImage> images = new Dictionary<string, AtlasImage>(StringComparer.Ordinal);
        Dictionary<string, AtlasFont> fonts = new Dictionary<string, AtlasFont>(StringComparer.Ordinal);

        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }

        public Atlas(Texture2D texture, Vector2 whitePixel)
        {
            Texture = texture;
            WhitePixel = whitePixel;
        }

        public AtlasImage AddImage(ref string name, int width, int height, int offsetX, int offsetY, ref Rectangle uvRect, bool rotate90)
        {
            if (images.ContainsKey(name))
                throw new Exception($"Atlas already has image with name: \"{name}\"");

            var image = new AtlasImage(this, ref name, width, height, offsetX, offsetY, ref uvRect, rotate90);
            images.Add(name, image);
            return image;
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, Rectangle uvRect, bool rotate90)
        {
            return AddImage(ref name, width, height, offsetX, offsetY, ref uvRect, rotate90);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, Rectangle uvRect)
        {
            return AddImage(ref name, width, height, offsetX, offsetY, ref uvRect, false);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, RectangleI subRect, bool rotate90)
        {
            Rectangle uvRect = subRect;
            uvRect.X /= Texture.Width;
            uvRect.Y /= Texture.Height;
            uvRect.W /= Texture.Width;
            uvRect.H /= Texture.Height;
            return AddImage(ref name, width, height, offsetX, offsetY, ref uvRect, rotate90);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, RectangleI subRect)
        {
            Rectangle uvRect = subRect;
            uvRect.X /= Texture.Width;
            uvRect.Y /= Texture.Height;
            uvRect.W /= Texture.Width;
            uvRect.H /= Texture.Height;
            return AddImage(ref name, width, height, offsetX, offsetY, ref uvRect, false);
        }

        public AtlasFont AddFont(string name, int ascent, int descent, int lineGap)
        {
            if (fonts.ContainsKey(name))
                throw new Exception($"Atlas already has font with name: \"{name}\"");

            var font = new AtlasFont(this, ref name, ascent, descent, lineGap);
            fonts.Add(name, font);
            return font;
        }
    }
}
