using System;
using System.Collections.Generic;
namespace Rise
{
    //TODO: should this be in Rise.Framework? It feels a bit too specific
    public class Atlas
    {
        public Texture2D Texture { get; private set; }
        Dictionary<string, AtlasImage> images = new Dictionary<string, AtlasImage>(StringComparer.Ordinal);
        Dictionary<string, AtlasFont> fonts = new Dictionary<string, AtlasFont>(StringComparer.Ordinal);
        Dictionary<string, AtlasTiles> tiles = new Dictionary<string, AtlasTiles>(StringComparer.Ordinal);

        public int Width { get { return Texture.Width; } }
        public int Height { get { return Texture.Height; } }

        public Atlas(Texture2D texture)
        {
            Texture = texture;
        }

        public bool TryGetImage(ref string name, out AtlasImage result)
        {
            return images.TryGetValue(name, out result);
        }
        public bool TryGetImage(string name, out AtlasImage result)
        {
            return images.TryGetValue(name, out result);
        }
        public AtlasImage GetImage(ref string name)
        {
            AtlasImage result;
            if (!images.TryGetValue(name, out result))
                throw new Exception($"Atlas does not have image with name: \"{name}\"");
            return result;
        }
        public AtlasImage GetImage(string name)
        {
            return GetImage(ref name);
        }

        public bool TryGetFont(ref string name, out AtlasFont result)
        {
            return fonts.TryGetValue(name, out result);
        }
        public bool TryGetFont(string name, out AtlasFont result)
        {
            return fonts.TryGetValue(name, out result);
        }
        public AtlasFont GetFont(ref string name)
        {
            AtlasFont result;
            if (!fonts.TryGetValue(name, out result))
                throw new Exception($"Atlas does not have font with name: \"{name}\"");
            return result;
        }
        public AtlasFont GetFont(string name)
        {
            return GetFont(ref name);
        }

        public bool TryGetTiles(ref string name, out AtlasTiles result)
        {
            return tiles.TryGetValue(name, out result);
        }
        public bool TryGetTiles(string name, out AtlasTiles result)
        {
            return tiles.TryGetValue(name, out result);
        }
        public AtlasTiles GetTiles(ref string name)
        {
            AtlasTiles result;
            if (!tiles.TryGetValue(name, out result))
                throw new Exception($"Atlas does not have tiles with name: \"{name}\"");
            return result;
        }
        public AtlasTiles GetTiles(string name)
        {
            return GetTiles(ref name);
        }

        public AtlasImage AddImage(ref string name, int width, int height, int offsetX, int offsetY, int trimW, int trimH, ref Rectangle uvRect, bool rotate90)
        {
            if (images.ContainsKey(name))
                throw new Exception($"Atlas already has image with name: \"{name}\"");

            var image = new AtlasImage(this, ref name, width, height, offsetX, offsetY, trimW, trimH, ref uvRect, rotate90);
            images.Add(name, image);
            return image;
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, int trimW, int trimH, Rectangle uvRect, bool rotate90)
        {
            return AddImage(ref name, width, height, offsetX, offsetY, trimW, trimH, ref uvRect, rotate90);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, int trimW, int trimH, Rectangle uvRect)
        {
            return AddImage(ref name, width, height, offsetX, offsetY, trimW, trimH, ref uvRect, false);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, int trimW, int trimH, RectangleI subRect, bool rotate90)
        {
            Rectangle uvRect = subRect;
            uvRect.X /= Texture.Width;
            uvRect.Y /= Texture.Height;
            uvRect.W /= Texture.Width;
            uvRect.H /= Texture.Height;
            return AddImage(ref name, width, height, offsetX, offsetY, trimW, trimH, ref uvRect, rotate90);
        }
        public AtlasImage AddImage(string name, int width, int height, int offsetX, int offsetY, int trimW, int trimH, RectangleI subRect)
        {
            Rectangle uvRect = subRect;
            uvRect.X /= Texture.Width;
            uvRect.Y /= Texture.Height;
            uvRect.W /= Texture.Width;
            uvRect.H /= Texture.Height;
            return AddImage(ref name, width, height, offsetX, offsetY, trimW, trimH, ref uvRect, false);
        }

        public AtlasFont AddFont(string name, int ascent, int descent, int lineGap)
        {
            if (fonts.ContainsKey(name))
                throw new Exception($"Atlas already has font with name: \"{name}\"");

            var font = new AtlasFont(this, ref name, ascent, descent, lineGap);
            fonts.Add(name, font);
            return font;
        }

        public AtlasTiles AddTiles(string name, int cols, int rows, int tileWidth, int tileHeight)
        {
            var tileset = new AtlasTiles(this, name, cols, rows, tileWidth, tileHeight);
            tiles.Add(name, tileset);
            return tileset;
        }
    }
}
