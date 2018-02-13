using System;
namespace Rise
{
    public class AtlasChar
    {
        public char Char { get; private set; }
        public int Advance { get; private set; }
        public int OffsetX { get; private set; }
        public int OffsetY { get; private set; }
        public AtlasImage Image { get; private set; }

        internal AtlasChar(char chr, int adv, int offX, int offY, AtlasImage img)
        {
            Char = chr;
            Advance = adv;
            OffsetX = offX;
            OffsetY = offY;
            Image = img;
        }
    }
}
