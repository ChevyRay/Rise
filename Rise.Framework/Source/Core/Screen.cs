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
        public static RectangleI Rect { get { return position; } }

        public static float CenterX { get { return position.X + position.W * 0.5f; } }
        public static float CenterY { get { return position.Y + position.H * 0.5f; } }
        public static Vector2 Center { get { return new Vector2(CenterX, CenterY); } }
        public static Point2 Size { get { return new Point2(position.W, position.H); }}

        public static int DrawWidth { get { return drawSize.X; } }
        public static int DrawHeight { get { return drawSize.Y; } }

        public static Color4 ClearColor = Color4.Black;

        static bool resizable = true;

        internal static void Init()
        {
            App.platform.SetResizable(resizable);

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

        public static bool Resizable
        {
            get { return resizable; }
            set
            {
                if (resizable != value)
                {
                    resizable = value;
                    App.platform.SetResizable(value);
                }
            }
        }
    }
}
