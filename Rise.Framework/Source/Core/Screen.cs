using System;
namespace Rise
{
    public static class Screen
    {
        static RectangleI position;
        static Point2 drawSize;

        public static float PixelW { get; private set; }
        public static float PixelH { get; private set; }

        public static bool Fullscreen { get { return App.platform.Fullscreen; } }
        public static bool VSync { get { return App.platform.VSync; } }

        public static int X { get { return position.X; } }
        public static int Y { get { return position.Y; } }
        public static int Width { get { return position.W; } }
        public static int Height { get { return position.H; } }
        public static RectangleI WindowPos { get { return position; } }

        public static int DrawWidth { get { return drawSize.X; } }
        public static int DrawHeight { get { return drawSize.Y; } }
        //public static Point2 Size { get { return drawSize; } }
        //public static Vector2 Center { get { return new Vector2(drawSize.X / 2, drawSize.Y / 2); } }

        public static Color4 ClearColor = Color4.Black;

        internal static void Init()
        {
            UpdatePosition();
            UpdateSize();

            App.platform.OnWinMoved += UpdatePosition;
            App.platform.OnWinResized += UpdateSize;
            App.platform.OnWinMaximized += UpdatePosition;
            App.platform.OnWinMaximized += UpdateSize;
            App.platform.OnWinRestored += UpdatePosition;
            App.platform.OnWinRestored += UpdateSize;
        }

        static void UpdatePosition()
        {
            App.platform.GetPosition(out position.X, out position.Y);
        }

        static void UpdateSize()
        {
            App.platform.GetSize(out position.W, out position.H);
            App.platform.GetDrawSize(out drawSize.X, out drawSize.Y);
            PixelW = position.W / (float)drawSize.X;
            PixelH = position.H / (float)drawSize.Y;
        }

        public static void Resize(int w, int h)
        {
            App.platform.SetSize(w, h);
            App.platform.CenterWindow();
            UpdatePosition();
            UpdateSize();
        }

        public static void SetFullscreen(bool fullscreen)
        {
            App.platform.SetFullScreen(fullscreen);
            UpdatePosition();
            UpdateSize();
        }

        public static void ToggleFullscreen()
        {
            SetFullscreen(!Fullscreen);
        }

        public static void SetVSync(bool vsync)
        {
            App.platform.SetVSync(vsync);
        }
    }
}
