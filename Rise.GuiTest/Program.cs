using System;

namespace Rise.GuiTest
{
    static class Program
    {
        static DrawBatch2D batch;
        static Atlas atlas;

        public static void Main(string[] args)
        {
            App.Init<PlatformSDL.PlatformSDL2>();
            App.OnInit += Init;
            App.OnUpdate += Update;
            App.OnRender += Render;
            App.Run("Gui Test", 960, 540, null);
        }

        static void Init()
        {
            batch = new DrawBatch2D();

            var font = new Font("Assets/Inconsolata.otf");
            var size = new FontSize(font, 32f);

            var builder = new AtlasBuilder(512);
            builder.AddBitmap("Assets/star.png", true, true);
            builder.AddBitmap("Assets/face.png", true, true);
            builder.AddBitmap("Assets/maritte.png", true, true);
            builder.AddFont("font", size, true);

            atlas = builder.Build(1);
            if (atlas == null)
                throw new Exception("Failed to build atlas.");

            var bitmap = atlas.Texture.GetBitmap();
            bitmap.SavePng("atlas.png");
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            batch.Begin();

            var star = atlas.GetImage("star");
            batch.DrawImage(star, Mouse.Position, Color4.White);
            //batch.DrawTexture(face, Mouse.Position, Color4.White);

            batch.End();
        }
    }
}
