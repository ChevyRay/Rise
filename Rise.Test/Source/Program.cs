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
            var bytes = decoder.Decode(File.ReadAllBytes("Assets/face.png"), out w, out h);

            int i = 3;
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    Console.Write(bytes[i] > 0 ? 'X' : ' ');
                    i += 4;
                }
                Console.WriteLine();
            }
        }
    }
}
