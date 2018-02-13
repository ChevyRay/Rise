using System;
namespace Rise
{
    public class AtlasChar
    {
        public char Char { get; private set; }
        public int Advance { get; private set; }
        public AtlasImage Image { get; private set; }

        internal AtlasChar(char chr, int adv, AtlasImage img)
        {
            Char = chr;
            Advance = adv;
            Image = img;
        }
    }
}
