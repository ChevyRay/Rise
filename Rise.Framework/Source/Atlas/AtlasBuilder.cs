using System;
using System.Collections.Generic;
using System.IO;
namespace Rise
{
    //TODO: detect duplicates (eg. if 2 different bitmaps, when trimmed, are equal, we should only pack one)
    public class AtlasBuilder
    {
        struct Packed
        {
            public int ID;
            public RectangleI Rect;
        }

        int maxSize;
        Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>(StringComparer.Ordinal);
        Dictionary<string, FontSize> fonts = new Dictionary<string, FontSize>(StringComparer.Ordinal);
        List<FontSize> fontsToPremultiply = new List<FontSize>();
        Dictionary<Bitmap, RectangleI> trims = new Dictionary<Bitmap, RectangleI>();
        int packCount = 1;

        public AtlasBuilder(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void AddBitmap(string name, Bitmap bitmap, bool trim)
        {
            if (bitmaps.ContainsKey(name))
                throw new Exception($"AtlasBuilder already has bitmap with name: \"{name}\"");
            bitmaps.Add(name, bitmap);

            if (trim)
                trims[bitmap] = bitmap.GetPixelBounds(0);

            ++packCount;
        }
        public void AddBitmap(string name, string file, bool premultiply, bool trim)
        {
            var bitmap = new Bitmap(file);
            if (premultiply)
                bitmap.Premultiply();
            AddBitmap(name, bitmap, trim);
        }
        public void AddBitmap(string file, bool premultiply, bool trim)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            AddBitmap(name, file, premultiply, trim);
        }

        public void AddFont(string name, FontSize font, bool premultiply)
        {
            if (fonts.ContainsKey(name))
                throw new Exception($"AtlasBuilder already has font with name: \"{name}\"");
            fonts.Add(name, font);
            if (premultiply)
                fontsToPremultiply.Add(font);
            packCount += font.CharCount;
        }

        public Atlas Build(int pad)
        {
            var packer = new RectanglePacker(maxSize, maxSize, packCount);

            //This ID is used to keep the rectangles ordered (we need to unpack in the same order we packed)
            int nextID = 0;

            //Add all the bitmaps (padding them)
            foreach (var pair in bitmaps)
            {
                RectangleI rect;
                if (!trims.TryGetValue(pair.Value, out rect))
                    rect = new RectangleI(pair.Value.Width, pair.Value.Height);
                packer.Add(++nextID, rect.W + pad, rect.H + pad, true);
            }

            //Add all the font characters (padding them)
            foreach (var pair in fonts)
            {
                FontChar chr;
                for (int i = 0; i < pair.Value.CharCount; ++i)
                {
                    pair.Value.GetCharInfoAt(i, out chr);
                    if (!pair.Value.IsEmpty(chr.Char))
                        packer.Add(++nextID, chr.Width + pad, chr.Height + pad, true);
                }
            }

            //Pack the rectangles
            if (!packer.Pack())
                return null;

            //Sort the packed rectangles so they're in the same order we added them
            var packed = new Packed[packer.PackedCount];
            for (int i = 0; i < packed.Length; ++i)
                packer.GetPacked(i, out packed[i].ID, out packed[i].Rect);
            Array.Sort(packed, (a, b) => a.ID.CompareTo(b.ID));

            //Get the atlas size
            int atlasW, atlasH;
            packer.GetBounds(out atlasW, out atlasH);
            atlasW = atlasW.ToPowerOf2();
            atlasH = atlasH.ToPowerOf2();

            ///Create the atlas with an empty texture for now
            var atlas = new Atlas(new Texture2D(atlasW, atlasH, TextureFormat.RGBA));
            var atlasBitmap = new Bitmap(atlasW, atlasH);
            var rotBitmap = new Bitmap(1, 1);
            var trimBitmap = new Bitmap(1, 1);

            //Reset the ID so we get the correct packed rectangles as we go
            nextID = 0;

            //Add the images
            foreach (var pair in bitmaps)
            {
                var bitmap = pair.Value;

                RectangleI trim;
                if (!trims.TryGetValue(bitmap, out trim))
                    trim = new RectangleI(bitmap.Width, bitmap.Height);

                //Get the rectangle and unpad it
                var rect = packed[nextID++].Rect;
                rect.W -= pad;
                rect.H -= pad;

                atlas.AddImage(pair.Key, bitmap.Width, bitmap.Height, trim.X, trim.Y, trim.W, trim.H, rect, trim.W != rect.W);

                //Blit the bitmap onto the atlas, optionally rotating it
                if (trim.W != rect.W)
                {
                    //Rotate the bitmap, trimming first if the bitmap was trimmed
                    if (trim.W < bitmap.Width || trim.H < bitmap.Height)
                    {
                        bitmap.GetSubRect(trimBitmap, trim);
                        trimBitmap.RotateRight(rotBitmap);
                    }
                    else
                        bitmap.RotateRight(rotBitmap);

                    atlasBitmap.CopyPixels(rotBitmap, rect.X, rect.Y);
                }
                else
                    atlasBitmap.CopyPixels(bitmap, trim.X, trim.Y, trim.W, trim.H, rect.X, rect.Y);
            }

            //Add the fonts
            Bitmap charBitmap = null;
            foreach (var pair in fonts)
            {
                var size = pair.Value;

                //This is the bitmap we'll render the character onto
                if (charBitmap == null)
                    charBitmap = new Bitmap(size.MaxCharW, size.MaxCharH);
                else
                    charBitmap.Resize(size.MaxCharW, size.MaxCharH);

                //Create an atlas font to populate with the characters
                var font = atlas.AddFont(pair.Key, size.Ascent, size.Descent, size.LineGap);
                FontChar chr;
                RectangleI rect;
                for (int i = 0; i < size.CharCount; ++i)
                {
                    size.GetCharInfoAt(i, out chr);

                    if (!size.IsEmpty(chr.Char))
                    {
                        //Get the packed rectangle and unpad it
                        rect = packed[nextID++].Rect;
                        rect.W -= pad;
                        rect.H -= pad;

                        //Rasterize the character and optionally rotate it before blitting
                        size.GetPixels(chr.Char, charBitmap, fontsToPremultiply.Contains(size));
                        if (chr.Width != rect.W)
                        {
                            charBitmap.RotateRight(rotBitmap);
                            atlasBitmap.CopyPixels(rotBitmap, rect.X, rect.Y);
                        }
                        else
                            atlasBitmap.CopyPixels(charBitmap, rect.X, rect.Y);
                    }
                    else
                        rect = RectangleI.Empty;
                    
                    var atlasChar = font.AddChar(chr.Char, chr.Width, chr.Height, chr.Advance, chr.OffsetX, chr.OffsetY, rect, chr.Width != rect.W);

                    //Set character kerning
                    for (int j = 0; j < size.CharCount; ++j)
                    {
                        char nextChar = size.GetCharAt(j);
                        int kern = size.GetKerning(chr.Char, nextChar);
                        if (kern != 0)
                            atlasChar.SetKerning(nextChar, kern);
                    }
                }
            }

            //Now that the bitmap is rendered, upload it to the texture
            atlas.Texture.SetPixels(atlasBitmap);

            return atlas;
        }
    }
}
