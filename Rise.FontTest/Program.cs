using System;

namespace Rise.FontTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            /*var font = new Font("Cousine-Regular.ttf", Font.AsciiChars);
            var size = new FontSize(font, 128f);

            var bitmap = new Bitmap(size.MaxCharW, size.MaxCharH);
            size.GetPixels('a', bitmap);
            bitmap.SavePng("letter.png");
            bitmap.SaveTga("letter.tga");
            bitmap.SaveBmp("letter.bmp");
            bitmap.SaveJpg("letter.jpg", 50);*/

            var packer = new RectanglePacker(512, 512);
            Rand.SetSeed(123);
            for (int i = 0; i < 860; ++i)
            {
                int w = Rand.Int(4, 32);
                int h = Rand.Int(4, 32);
                packer.Add(i, w, h, false);
            }
            if (packer.Pack())
            {
                Console.WriteLine("Packed: " + packer.PackedCount);

                var bitmap = new Bitmap(packer.Width, packer.Height, Color4.Black);
                for (int i = 0; i < packer.PackedCount; ++i)
                {
                    int id;
                    RectangleI rect;
                    packer.GetPacked(i, out id, out rect);
                    rect.W--;
                    rect.H--;
                    bitmap.SetRect(ref rect, Color4.White);
                }
                bitmap.SavePng("packer.png");
            }
            else
                Console.WriteLine("Fail: " + packer.PackedCount);
        }
    }
}
