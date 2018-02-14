using System;
using System.Collections.Generic;
namespace Rise
{
    public class AtlasChar
    {
        public AtlasFont Font { get; private set; }
        public char Char { get; private set; }
        public int Advance { get; private set; }
        public AtlasImage Image { get; private set; }

        Dictionary<char, int> kerning;

        internal AtlasChar(AtlasFont font, char chr, int adv, AtlasImage img)
        {
            Font = font;
            Char = chr;
            Advance = adv;
            Image = img;
        }

        public void SetKerning(char nextChar, int value)
        {
            if (kerning == null)
                kerning = new Dictionary<char, int>();
            kerning[nextChar] = value;
        }

        public int GetKerning(char nextChar)
        {
            if (kerning == null)
                return 0;
            int kern;
            kerning.TryGetValue(nextChar, out kern);
            return kern;
        }
    }
}
