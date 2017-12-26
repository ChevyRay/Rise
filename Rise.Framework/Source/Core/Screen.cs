using System;
namespace Rise
{
    public static class Screen
    {
        static int x;
        static int y;
        static int width;
        static int height;
        static int drawWidth;
        static int drawHeight;

        public static float PixelW { get; private set; }
        public static float PixelH { get; private set; }

        public static bool Fullscreen { get { return App.platform.Fullscreen; } }
        public static bool VSync { get { return App.platform.VSync; } }

        public static int X { get { return x; } }
        public static int Y { get { return y; } }
        public static int Width { get { return width; } }
        public static int Height { get { return height; } }

        public static int DrawWidth { get { return drawWidth; } }
        public static int DrawHeight { get { return drawHeight; } }

        public static RectangleI Position { get { return new RectangleI(x, y, width, height); } }

        public static Color ClearColor = Color.Black;

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
            App.platform.GetPosition(out x, out y);
        }

        static void UpdateSize()
        {
            App.platform.GetSize(out width, out height);
            App.platform.GetDrawSize(out drawWidth, out drawHeight);
            PixelW = width / (float)drawWidth;
            PixelH = height / (float)drawHeight;
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
