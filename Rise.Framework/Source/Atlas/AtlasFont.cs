using System;
using System.Collections.Generic;
namespace Rise
{
    public class AtlasFont
    {
        public Atlas Atlas { get; private set; }
        public string Name { get; private set; }
        public int Ascent { get; private set; }
        public int Descent { get; private set; }
        public int LineGap { get; private set; }
        public int Height { get; private set; }

        Dictionary<char, AtlasChar> chars = new Dictionary<char, AtlasChar>();

        internal AtlasFont(Atlas atlas, ref string name, int ascent, int descent, int lineGap)
        {
            Atlas = atlas;
            Name = name;
            Ascent = ascent;
            Descent = descent;
            LineGap = lineGap;
            Height = ascent - descent;
        }

        public AtlasChar AddChar(char chr, int width, int height, int advance, int offsetX, int offsetY, RectangleI subRect, bool rotate90)
        {
            Rectangle uvRect = subRect;
            uvRect.X /= Atlas.Width;
            uvRect.Y /= Atlas.Height;
            uvRect.W /= Atlas.Width;
            uvRect.H /= Atlas.Height;
            return AddChar(chr, width, height, advance, offsetX, offsetY, uvRect, rotate90);
        }
        public AtlasChar AddChar(char chr, int width, int height, int advance, int offsetX, int offsetY, Rectangle uvRect, bool rotate90)
        {
            if (chars.ContainsKey(chr))
                throw new Exception(string.Format("Font already has character: '{0}' (U+{1:X4})", chr, (UInt16)chr));

            var name = chr.ToString();

            AtlasImage image = null;
            if (width > 0)
                image = new AtlasImage(Atlas, ref name, width, height, offsetX, offsetY, width, height, ref uvRect, rotate90);

            var result = new AtlasChar(this, chr, advance, image);
            chars.Add(chr, result);

            return result;
        }

        public AtlasChar GetChar(char chr)
        {
            return chars[chr];
        }

        public bool TryGetchar(char chr, out AtlasChar result)
        {
            return chars.TryGetValue(chr, out result);
        }

        public int GetWidth(ref string text)
        {
            int w = 0;
            AtlasChar prev = null;
            AtlasChar chr;
            for (int i = 0; i < text.Length; ++i)
            {
                chr = GetChar(text[i]);
                if (prev != null)
                    w += prev.GetKerning(text[i]);
                w += chr.Advance;
                prev = chr;
            }

            //For the final character, we want to slice off the character spacing
            if (prev != null)
                w -= prev.Advance - prev.Image.Width;

            return w;
        }
        public int GetWidth(string text)
        {
            return GetWidth(ref text);
        }
    }
}
