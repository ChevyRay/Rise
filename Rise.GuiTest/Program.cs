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

            var font = new Font("Assets/NotoSans-Regular.ttf", CharSet.BasicLatin);
            var size = new FontSize(font, 128f);

            var builder = new AtlasBuilder(2048);
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

            var font = atlas.GetFont("font");
            int h = font.Height + font.LineGap;
            var p = new Vector2(32, font.Ascent);
            batch.DrawText(font, "ATAVAWAYAvAwAyFaFeFoKv", p + new Vector2(0f, 0f), Color4.White);
            batch.DrawText(font, "VoVrVuVyWAWOWaWeWrWvWy", p + new Vector2(0f, h), Color4.White);
            batch.DrawText(font, "TeTiToTrTsTuTyUAVAVaVe", p + new Vector2(0f, h * 2f), Color4.White);
            batch.DrawText(font, "VoVrVuVyWAWOWaWeWrWvWy", p + new Vector2(0f, h * 3f), Color4.White);
            //batch.DrawTexture(face, Mouse.Position, Color4.White);

            batch.End();
        }
    }
}
