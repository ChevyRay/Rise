using System;

namespace Rise.FontTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            var font = new Font("Cousine-Regular.ttf", Font.AsciiChars);
            var size = new FontSize(font, 128f);

            var bitmap = new Bitmap(size.MaxCharW, size.MaxCharH);
            size.GetPixels('a', bitmap);
            bitmap.SavePng("letter.png");
            bitmap.SaveTga("letter.tga");
            bitmap.SaveBmp("letter.bmp");
            bitmap.SaveJpg("letter.jpg", 50);
        }
    }
}
