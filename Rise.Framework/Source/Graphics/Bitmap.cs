using System;
using System.IO;
using System.Collections.Generic;
namespace Rise
{
    public class Bitmap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int PixelCount { get; private set; }

        Color4[] pixels;

        public Bitmap(string file)
        {
            var bytes = File.ReadAllBytes(file);
            int w, h;
            pixels = ImageDecoder.Decode(bytes, out w, out h);
            Width = w;
            Height = h;
            PixelCount = w * h;
        }
        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            PixelCount = width * height;
            pixels = new Color4[PixelCount];
        }
        public Bitmap(int width, int height, Color4 color) : this(width, height)
        {
            Clear(color);
        }
        internal Bitmap(Color4[] pixels, int width, int height)
        {
            SetPixels(pixels, width, height);
        }

        public void SavePng(string file)
        {
            var bytes = new List<byte>();
            ImageEncoder.EncodePng(pixels, Width, Height, bytes);
            File.WriteAllBytes(file, bytes.ToArray());
        }

        public void SaveBmp(string file)
        {
            var bytes = new List<byte>();
            ImageEncoder.EncodeBmp(pixels, Width, Height, bytes);
            File.WriteAllBytes(file, bytes.ToArray());
        }

        public void SaveTga(string file)
        {
            var bytes = new List<byte>();
            ImageEncoder.EncodeTga(pixels, Width, Height, bytes);
            File.WriteAllBytes(file, bytes.ToArray());
        }

        public void SaveJpg(string file, int quality)
        {
            var bytes = new List<byte>();
            ImageEncoder.EncodeJpg(pixels, Width, Height, quality, bytes);
            File.WriteAllBytes(file, bytes.ToArray());
        }

        public void Resize(int width, int height)
        {
            if (width != Width || height != Height)
            {
                Width = width;
                Height = height;
                if (PixelCount != width * height)
                {
                    PixelCount = width * height;
                    if (pixels.Length < PixelCount)
                        Array.Resize(ref pixels, PixelCount);
                }
            }
        }

        public Color4[] Pixels
        {
            get { return pixels; }
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

        public Color4 GetPixel(int x, int y)
        {
            return pixels[y * Width + x];
        }

        public void SetPixel(int x, int y, Color4 color)
        {
            pixels[y * Width + x] = color;
        }

        internal void SetPixels(Color4[] pixels, int width, int height)
        {
            if (width * height != pixels.Length)
                throw new Exception("Pixel array length does not match bitmap size.");
            this.pixels = pixels;
            Width = width;
            Height = height;
            PixelCount = width * height;
        }

        public void SetRect(int x, int y, int w, int h, Color4 color)
        {
            int ii = x + w;
            int jj = y + h;
            for (int j = y; j < jj; ++j)
                for (int i = x; i < ii; ++i)
                    pixels[j * Width + i] = color;
        }
        public void SetRect(ref RectangleI rect, Color4 color)
        {
            SetRect(rect.X, rect.Y, rect.W, rect.H, color);
        }
        public void SetRect(RectangleI rect, Color4 color)
        {
            SetRect(rect.X, rect.Y, rect.W, rect.H, color);
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

        public void Clear(Color4 color)
        {
            for (int i = 0; i < PixelCount; ++i)
                pixels[i] = color;
        }
        public void Clear()
        {
            Clear(Color4.Transparent);
        }

        public void Premultiply()
        {
            for (int i = 0; i < PixelCount; ++i)
            {
                float m = pixels[i].A / 255f;
                pixels[i].R = (byte)(pixels[i].R * m);
                pixels[i].G = (byte)(pixels[i].G * m);
                pixels[i].B = (byte)(pixels[i].B * m);
            }
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
