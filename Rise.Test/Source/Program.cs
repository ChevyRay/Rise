using System;
using System.IO;
using Rise;
using Rise.Imaging;
namespace Rise.Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            var decoder = new PngDecoder();
            int w, h;
            var pixels = decoder.Decode(File.ReadAllBytes("Assets/face.png"), out w, out h);

            int i = 0;
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    Console.Write(pixels[i].A > 0 ? 'X' : ' ');
                    ++i;
                }
                Console.WriteLine();
            }
        }
    }
}
