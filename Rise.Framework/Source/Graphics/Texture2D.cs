using System;
using System.Collections.Generic;
using Rise.OpenGL;
using Rise.Imaging;
namespace Rise
{
    public class Texture2D : Texture
    {
        Texture2D(TextureFormat format) : base(GL.GenTexture(), format, TextureTarget.Texture2D, TextureTarget.Texture2D)
        {
            WrapX = DefaultWrapX;
            WrapY = DefaultWrapY;
            MinFilter = DefaultMinFilter;
            MagFilter = DefaultMagFilter;
        }
        public Texture2D(Bitmap bitmap) : this(TextureFormat.RGBA)
        {
            SetPixels(bitmap);
        }
        public Texture2D(string file, bool premultiply) : this(TextureFormat.RGBA)
        {
            var bitmap = App.ImageLoader.LoadFile(file);
            if (premultiply)
                bitmap.Premultiply();
            SetPixels(bitmap);
        }
        public Texture2D(int width, int height, Color4[] pixels) : this(TextureFormat.RGBA)
        {
            SetPixels(width, height, pixels);
        }
        public Texture2D(int width, int height, Color3[] pixels) : this(TextureFormat.RGB)
        {
            SetPixels(width, height, pixels);
        }
        public Texture2D(int width, int height, TextureFormat format) : this(format)
        {
            SetPixels(width, height, format.PixelFormat(), PixelType.UnsignedByte, IntPtr.Zero);
        }

        protected override void Dispose()
        {
            GL.DeleteTexture(ID);
        }

        public TextureWrap WrapX
        {
            get { return (TextureWrap)GetParam(TextureParam.WrapS); }
            set { SetParam(TextureParam.WrapS, (int)value); }
        }

        public TextureWrap WrapY
        {
            get { return (TextureWrap)GetParam(TextureParam.WrapT); }
            set { SetParam(TextureParam.WrapT, (int)value); }
        }

        public void SetWrap(TextureWrap wrap)
        {
            WrapX = wrap;
            WrapY = wrap;
        }

        public TextureFilter MinFilter
        {
            get { return (TextureFilter)GetParam(TextureParam.MinFilter); }
            set { SetParam(TextureParam.MinFilter, (int)value); }
        }

        public TextureFilter MagFilter
        {
            get { return (TextureFilter)GetParam(TextureParam.MagFilter); }
            set { SetParam(TextureParam.MagFilter, (int)value); }
        }

        public void SetFilter(TextureFilter filter)
        {
            MinFilter = filter;
            MagFilter = filter;
        }

        /*public unsafe void GetBitmap(Bitmap bitmap)
        {
            if (Width != bitmap.Width)
                throw new Exception("Bitmap width does not match.");
            if (Height != bitmap.Height)
                throw new Exception("Bitmap height does not match.");
            MakeCurrent();
            fixed (Color4* ptr = bitmap.Pixels)
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public Bitmap GetBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            GetBitmap(bitmap);
            return bitmap;
        }

        public unsafe void GetPixelsColor4(Color4[] pixels)
        {
            if (pixels.Length < Width * Height)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (Color4* ptr = pixels)
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public Color4[] GetPixelsColor4()
        {
            var pixels = new Color4[Width * Height];
            GetPixelsColor4(pixels);
            return pixels;
        }

        public unsafe void GetPixelsColor3(Color3[] pixels)
        {
            if (pixels.Length < Width * Height)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (Color3* ptr = pixels)
                GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.RGB, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public Color3[] GetPixelsColor3()
        {
            var pixels = new Color3[Width * Height];
            GetPixelsColor3(pixels);
            return pixels;
        }

        unsafe void GetPixels(ref byte[] pixels, int comp, PixelFormat format)
        {
            if (pixels == null)
                pixels = new byte[Width * Height * comp];
            else if (pixels.Length < Width * Height * comp)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (byte* ptr = pixels)
                GL.GetTexImage(TextureTarget.Texture2D, 0, format, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        unsafe void GetPixels(ref float[] pixels, int comp, PixelFormat format)
        {
            if (pixels == null)
                pixels = new float[Width * Height * comp];
            else if (pixels.Length < Width * Height * comp)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (float* ptr = pixels)
                GL.GetTexImage(TextureTarget.Texture2D, 0, format, PixelType.Float, new IntPtr(ptr));
        }

        public void GetPixelsRGBA(ref byte[] pixels)
        {
            GetPixels(ref pixels, 4, PixelFormat.RGBA);
        }
        public void GetPixelsRGBA(ref float[] pixels)
        {
            GetPixels(ref pixels, 4, PixelFormat.RGBA);
        }

        public void GetPixelsRGB(ref byte[] pixels)
        {
            GetPixels(ref pixels, 3, PixelFormat.RGB);
        }
        public void GetPixelsRGB(ref float[] pixels)
        {
            GetPixels(ref pixels, 3, PixelFormat.RGB);
        }

        public void GetPixelsRG(ref byte[] pixels)
        {
            GetPixels(ref pixels, 2, PixelFormat.RG);
        }
        public void GetPixelsRG(ref float[] pixels)
        {
            GetPixels(ref pixels, 2, PixelFormat.RG);
        }

        public void GetPixelsR(ref byte[] pixels)
        {
            GetPixels(ref pixels, 1, PixelFormat.R);
        }
        public void GetPixelsR(ref float[] pixels)
        {
            GetPixels(ref pixels, 1, PixelFormat.R);
        }*/

        public void SetPixels(Bitmap bitmap)
        {
            SetPixels(bitmap.Width, bitmap.Height, bitmap.Pixels);
        }
        public unsafe void SetPixels(int width, int height, Color4[] pixels)
        {
            if (pixels == null)
            {
                SetPixels(width, height, PixelFormat.RGBA, PixelType.UnsignedByte, IntPtr.Zero);
                return;
            }
            if (pixels.Length < (width * height))
                throw new ArgumentException("Pixels array is too small.", nameof(pixels));
            fixed (Color4* ptr = pixels)
                SetPixels(width, height, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public unsafe void SetPixels(int width, int height, Color3[] pixels)
        {
            if (pixels == null)
            {
                SetPixels(width, height, PixelFormat.RGB, PixelType.UnsignedByte, IntPtr.Zero);
                return;
            }
            if (pixels.Length < (width * height))
                throw new ArgumentException("Pixels array is too small.", nameof(pixels));
            fixed (Color3* ptr = pixels)
                SetPixels(width, height, PixelFormat.RGB, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        void SetPixels(int w, int h, PixelFormat pixelFormat, PixelType type, IntPtr data)
        {
            Width = w;
            Height = h;

            if (pixelFormat == PixelFormat.Depth)
                type = PixelType.Float;

            MakeCurrent();
            GL.TexImage2D(BindTarget, 0, Format, w, h, 0, pixelFormat, type, data);
        }

        int GetParam(TextureParam p)
        {
            MakeCurrent();
            int val;
            GL.GetTexParameterI(BindTarget, p, out val);
            return val;
        }

        void SetParam(TextureParam p, int val)
        {
            MakeCurrent();
            GL.TexParameterI(BindTarget, p, val);
        }
    }
}
