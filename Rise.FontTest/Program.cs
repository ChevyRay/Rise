using System;
using System.Collections.Generic;
namespace Rise.FontTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            App.OnInit += () =>
            {
                var font = new Font("assets/Inconsolata.otf");
                var font2 = new Font("assets/Cousine-Regular.ttf");

                var builder = new AtlasBuilder(1024);

                builder.AddFont("font", new FontSize(font, 32f), false);
                builder.AddFont("font2", new FontSize(font, 64f), false);

                var face = new Bitmap("assets/face.png");
                var maritte = new Bitmap("assets/maritte.png");
                var star = new Bitmap("assets/star.png");

                for (int i = 0; i < 300; ++i)
                {
                    builder.AddBitmap("face" + i, face, true);
                    builder.AddBitmap("maritte" + i, maritte, true);
                    builder.AddBitmap("star" + i, star, true);
                }

                builder.AddFont("font3", new FontSize(font2, 32f), false);
                //builder.AddFont("font4", new FontSize(font2, 32f));

                var atlas = builder.Build(1);
                if (atlas != null)
                {
                    Console.WriteLine("Success!");

                    var bitmap = atlas.Texture.GetBitmap();
                    bitmap.SavePng("atlas.png");
                }
                else
                    Console.WriteLine("Failed!");
            };

            App.Init<PlatformSDL.PlatformSDL2>();
            App.Run("Font Test", 640, 360, null);
        }
    }
}
