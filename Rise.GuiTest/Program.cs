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

            view = new View(Screen.DrawWidth, Screen.DrawHeight);
            view.BackgroundColor = Color4.Grey;
            view.Layout = LayoutMode.Horizontal;
            view.Spacing = 16;
            view.SetPadding(16);

            var left = view.AddChild(new Container());
            //left.BackgroundColor = Color4.Black * 0.25f;
            left.Layout = LayoutMode.Vertical;
            left.FlexX = left.FlexY = true;
            left.Spacing = 16;

            var right = view.AddChild(new Container());
            right.BackgroundColor = Color4.Blue;
            right.Layout = LayoutMode.Vertical;
            right.FlexX = right.FlexY = true;

            var items = left.AddChild(new Container());
            items.BackgroundColor = Color4.Black * 0.25f;
            items.Layout = LayoutMode.Vertical;
            items.FlexX = items.FlexY = true;
            items.Spacing = 1;
            items.ScrollableY = true;

            var padding = left.AddChild(new Container(100, 100));
            padding.BackgroundColor = Color4.White * 0.25f;
            padding.FlexX = true;
            padding.FlexY = false;
            padding.HeightOfChildren = false;

            for (int i = 0; i < 50; ++i)
            {
                var item = items.AddChild(new Element(16, 40, true, false));
                item.OnRender += b => b.DrawRect(item.RectX, item.RectY, item.RectW, item.RectH, Color4.Blue);
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
            batch.SetMatrix(Matrix3x2.Scale(Screen.PixelW, Screen.PixelH));
            batch.DrawTextureFlipped(view.Texture, Vector2.Zero, Color4.White);
            batch.End();
        }
    }
}
