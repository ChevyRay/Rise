﻿using System;
using System.Collections.Generic;
namespace Rise
{
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
        int packCount = 1;

        public AtlasBuilder(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void AddBitmap(string name, Bitmap bitmap)
        {
            if (bitmaps.ContainsKey(name))
                throw new Exception($"AtlasBuilder already has bitmap with name: \"{name}\"");
            bitmaps.Add(name, bitmap);
            ++packCount;
        }
        public void AddBitmap(string name, string file, bool premultiply)
        {
            var bitmap = new Bitmap(file);
            if (premultiply)
                bitmap.Premultiply();
            AddBitmap(name, bitmap);
        }

        public void AddFont(string name, FontSize font)
        {
            if (fonts.ContainsKey(name))
                throw new Exception($"AtlasBuilder already has font with name: \"{name}\"");
            fonts.Add(name, font);
            packCount += font.CharCount;
        }

        public Atlas Build(int pad)
        {
            var packer = new RectanglePacker(maxSize, maxSize, packCount);

            //Add the white pixel
            packer.Add(0, 1 + pad, 1 + pad, false);

            //This ID is used to keep the rectangles ordered (we need to unpack in the same order we packed)
            int nextID = 1;

            //Add all the bitmaps (padding them)
            foreach (var pair in bitmaps)
                packer.Add(++nextID, pair.Value.Width + pad, pair.Value.Height + pad, true);

            //Add all the font characters (padding them)
            foreach (var pair in fonts)
            {
                FontChar chr;
                for (int i = 0; i < pair.Value.CharCount; ++i)
                {
                    pair.Value.GetCharAt(i, out chr);
                    if (!pair.Value.IsEmpty(chr.Char))
                        packer.Add(++nextID, chr.Width + pad, chr.Height + pad, true);
                }
            }

            //Pack the rectangles
            if (!packer.Pack())
                throw new Exception("Failed to pack atlas.");

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
            var atlas = new Atlas(new Texture2D(atlasW, atlasH, TextureFormat.RGBA), Vector2.Zero);
            var atlasBitmap = new Bitmap(atlasW, atlasH);
            var rotBitmap = new Bitmap(1, 1);

            //Add the white pixel
            var pixRect = packed[0].Rect;
            pixRect.W -= pad;
            pixRect.H -= pad;
            atlas.WhitePixel = new Vector2(pixRect.CenterX / (float)atlasW, pixRect.CenterY / (float)atlasH);

            //Reset the ID so we get the correct packed rectangles as we go
            nextID = 1;

            //Add the images
            foreach (var pair in bitmaps)
            {
                var bitmap = pair.Value;

                //Get the rectangle and unpad it
                var rect = packed[nextID++].Rect;
                rect.W -= pad;
                rect.H -= pad;

                atlas.AddImage(pair.Key, bitmap.Width, bitmap.Height, rect, bitmap.Width != rect.W);

                //Blit the bitmap onto the atlas, optionally rotating it
                if (bitmap.Width != rect.W)
                {
                    bitmap.RotateRight(rotBitmap);
                    atlasBitmap.CopyPixels(rotBitmap, rect.X, rect.Y);
                }
                else
                    atlasBitmap.CopyPixels(bitmap, rect.X, rect.Y);
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
                    size.GetCharAt(i, out chr);

                    if (!size.IsEmpty(chr.Char))
                    {
                        //Get the packed rectangle and unpad it
                        rect = packed[nextID++].Rect;
                        rect.W -= pad;
                        rect.H -= pad;

                        //Rasterize the character and optionally rotate it before blitting
                        size.GetPixels(chr.Char, charBitmap);
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
                    
                    font.AddChar(chr.Char, chr.Width, chr.Height, chr.Advance, chr.OffsetX, chr.OffsetY, rect, chr.Width == rect.W);
                }
            }

            //Now that the bitmap is rendered, upload it to the texture
            atlas.Texture.SetPixels(atlasBitmap);

            return atlas;
        }
    }
}