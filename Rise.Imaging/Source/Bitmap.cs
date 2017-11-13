using System;
using System.Collections.Generic;
namespace Rise.Imaging
{
    public class Bitmap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int PixelCount { get; private set; }

        internal Color[] pixels;

        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            PixelCount = width * height;
            pixels = new Color[PixelCount];
        }
        public Bitmap(int width, int height, Color color) : this(width, height)
        {
            Clear(color);
        }
        internal Bitmap(Color[] pixels, int width, int height)
        {
            SetPixels(pixels, width, height);
        }

        public bool IsTransparent
        {
            get 
            {
                for (int i = 0; i < PixelCount; ++i)
                    if (pixels[i].A > 0)
                        return false;
                return true;
            }
        }

        public Bitmap Clone()
        {
            var bitmap = new Bitmap(Width, Height);
            bitmap.CopyPixels(this);
            return bitmap;
        }

        public Color GetPixel(int x, int y)
        {
            return pixels[y * Width + x];
        }

        public void SetPixel(int x, int y, Color color)
        {
            pixels[y * Width + x] = color;
        }

        internal void SetPixels(Color[] pixels, int width, int height)
        {
            if (width * height != pixels.Length)
                throw new Exception("Pixel array length does not match bitmap size.");
            this.pixels = pixels;
            Width = width;
            Height = height;
            PixelCount = width * height;
        }

        public void SetRect(int x, int y, int w, int h, Color color)
        {
            int ii = x + w;
            int jj = y + h;
            for (int j = y; j < jj; ++j)
                for (int i = x; i < ii; ++i)
                    pixels[j * Width + i] = color;
        }

        public void CopyPixels(Bitmap source)
        {
            if (PixelCount != source.PixelCount)
                throw new Exception("Bitmaps are not the same size.");

            Array.Copy(source.pixels, pixels, PixelCount);
        }
        public void CopyPixels(Bitmap source, int sourceX, int sourceY, int width, int height, int destX, int destY)
        {
            int s = sourceY * source.Width + sourceX;
            int d = destY * Width + destX;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                    pixels[d + x] = source.pixels[s + x];
                s += source.Width;
                d += Width;
            }
        }
        public void CopyPixels(Bitmap source, RectangleI src, Point2 dst)
        {
            CopyPixels(source, src.X, src.Y, src.W, src.H, dst.X, dst.Y);
        }
        public void CopyPixels(Bitmap source, int destX, int destY)
        {
            CopyPixels(source, 0, 0, source.Width, source.Height, destX, destY);
        }
        public void CopyPixels(Bitmap source, Point2 dest)
        {
            CopyPixels(source, dest.X, dest.Y);
        }

        public void Clear(Color color)
        {
            for (int i = 0; i < PixelCount; ++i)
                pixels[i] = color;
        }
        public void Clear()
        {
            Clear(Color.Transparent);
        }

        public RectangleI GetPixelBounds(byte alphaThreshold)
        {
            int minX = Width - 1;
            int minY = Height - 1;
            int maxX = 0;
            int maxY = 0;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (pixels[y * Width + x].A > alphaThreshold)
                    {
                        if (x < minX)
                            minX = x;
                        if (y < minY)
                            minY = y;
                        if (x > maxX)
                            maxX = x;
                        if (y > maxY)
                            maxY = y;
                    }
                }
            }
            if (maxX >= minX && maxY >= minY)
                return new RectangleI(minX, minY, (maxX - minX) + 1, (maxY - minY) + 1);
            return RectangleI.Empty;
        }
    }
}
