using System;

namespace Rise.GuiTest
{
    static class Program
    {
        static DrawBatch2D batch;
        static Texture2D face;

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
            face = new Texture2D("Assets/star.png", true);
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            batch.Begin();

            batch.DrawTexture(face, Mouse.Position, Color4.White);

            batch.End();
        }
    }
}
