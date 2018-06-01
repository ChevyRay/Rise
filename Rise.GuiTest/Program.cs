using System;
using Rise.Gui;
namespace Rise.GuiTest
{
    static class Program
    {
        static DrawBatch2D batch;
        static Atlas atlas;
        static View view;

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

            batch = new DrawBatch2D();

            view = new View(Screen.Width, Screen.Height);
            view.BackgroundColor = Color4.Grey;
            view.Layout = LayoutMode.Horizontal;
            view.Spacing = 16;
            view.SetPadding(16);

            var left = view.AddChild(new Container());
            left.BackgroundColor = Color4.Red;
            left.Layout = LayoutMode.Vertical;
            left.FlexX = left.FlexY = true;

            var right = view.AddChild(new Container());
            right.BackgroundColor = Color4.Blue;
            right.Layout = LayoutMode.Vertical;
            right.FlexX = right.FlexY = true;

            Screen.ClearColor = Color4.Black;
        }

        static void Update()
        {
            view.SetSize(Screen.Width, Screen.Height);
            view.Update();
        }

        static void Render()
        {
            view.Render();

            batch.Begin();
            batch.DrawTexture(view.Texture, Vector2.Zero, Color4.White);
            batch.End();
        }
    }
}
