using System;
using System.IO;
using Rise.Gui;
namespace Rise.GuiTest
{
    static class Program
    {
        static Shader shader;
        static DrawBatch2D batch;
        static Atlas atlas;
        static GuiView view;

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
            var font = new Font("Assets/NotoSans-Regular.ttf", FontCharSet.BasicLatin);
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
            shader = new Shader(Shader.Basic2D);

            view = new GuiView(shader, Screen.DrawWidth, Screen.DrawHeight, LayoutMode.Horizontal);
            view.BackgroundColor = Color4.Grey;
            view.Spacing = 16;
            view.SetPadding(16);

            var left = view.AddChild(new GuiContainer());
            left.BackgroundColor = Color4.Black * 0.25f;
            left.FlexX = left.FlexY = true;
            left.Spacing = 16;

            var right = view.AddChild(new GuiContainer());
            right.BackgroundColor = Color4.Blue;
            right.FlexX = right.FlexY = true;

            var items = left.AddChild(new GuiContainer());
            items.BackgroundColor = Color4.Black * 0.25f;
            items.FlexX = items.FlexY = true;
            items.Spacing = 1;
            items.ScrollableY = true;

            var padding = left.AddChild(new GuiContainer(100, 100, LayoutMode.Vertical));
            padding.BackgroundColor = Color4.White * 0.25f;
            padding.FlexX = true;
            padding.FlexY = false;

            for (int i = 0; i < 50; ++i)
            {
                var item = items.AddChild(new GuiElement(16, 40));
                item.FlexY = false;
                item.OnRender += b => b.DrawRect(item.RectX, item.RectY, item.RectW, item.RectH, Color4.Red);
            }

            Screen.ClearColor = Color4.Black;
            Screen.OnResized += () => view.SetSize(Screen.DrawWidth, Screen.DrawHeight);
        }

        static void Update()
        {
            view.Update();
        }

        static void Render()
        {
            view.Render();

            batch.Begin();
            batch.SetMatrix(Matrix3x2.Scale(Screen.PixelSize, Screen.PixelSize));
            batch.DrawTextureFlipped(view.Texture, Vector2.Zero, Color4.White);
            batch.End();
        }
    }
}
