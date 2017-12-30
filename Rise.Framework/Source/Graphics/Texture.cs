using System;
using System.Collections.Generic;
using Rise.OpenGL;
using Rise.Imaging;
namespace Rise
{
    public class Texture : ResourceHandle
    {
        public static TextureFilter DefaultMinFilter = TextureFilter.Linear;
        public static TextureFilter DefaultMagFilter = TextureFilter.Linear;
        public static TextureWrap DefaultWrapX = TextureWrap.ClampToEdge;
        public static TextureWrap DefaultWrapY = TextureWrap.ClampToEdge;

        static Texture[] binded = new Texture[GL.MaxTextureUnits];
        static HashSet<Texture> unbind = new HashSet<Texture>();

        internal uint id;

        public TextureFormat Format { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        Texture(TextureFormat format)
        {
            id = GL.GenTexture();
            WrapX = DefaultWrapX;
            WrapY = DefaultWrapY;
            MinFilter = DefaultMinFilter;
            MagFilter = DefaultMagFilter;
            Format = format;
        }
        public Texture(Bitmap bitmap) : this(TextureFormat.RGBA)
        {
            SetPixels(bitmap);
        }
        public Texture(string file, bool premultiply) : this(TextureFormat.RGBA)
        {
            var bitmap = App.ImageLoader.LoadFile(file);
            if (premultiply)
                bitmap.Premultiply();
            SetPixels(bitmap);
        }
        public Texture(int width, int height, Color4[] pixels) : this(TextureFormat.RGBA)
        {
            SetPixels(width, height, pixels);
        }
        public Texture(int width, int height, Color3[] pixels) : this(TextureFormat.RGB)
        {
            SetPixels(width, height, pixels);
        }
        public Texture(int width, int height, TextureFormat format) : this(format)
        {
            SetPixels(width, height, format, format.PixelFormat(), PixelType.UnsignedByte, IntPtr.Zero);
        }

        protected override void Dispose()
        {
            GL.DeleteTexture(id);
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

        public unsafe void GetBitmap(Bitmap bitmap)
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
        }

        public void SetPixels(Bitmap bitmap)
        {
            SetPixels(bitmap.Width, bitmap.Height, bitmap.Pixels);
        }
        public unsafe void SetPixels(int width, int height, Color4[] pixels)
        {
            if (pixels == null)
            {
                SetPixels(width, height, TextureFormat.RGBA, PixelFormat.RGBA, PixelType.UnsignedByte, IntPtr.Zero);
                return;
            }
            if (pixels.Length < (width * height))
                throw new ArgumentException("Pixels array is too small.", nameof(pixels));
            fixed (Color4* ptr = pixels)
                SetPixels(width, height, TextureFormat.RGBA, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public unsafe void SetPixels(int width, int height, Color3[] pixels)
        {
            if (pixels == null)
            {
                SetPixels(width, height, TextureFormat.RGB, PixelFormat.RGB, PixelType.UnsignedByte, IntPtr.Zero);
                return;
            }
            if (pixels.Length < (width * height))
                throw new ArgumentException("Pixels array is too small.", nameof(pixels));
            fixed (Color3* ptr = pixels)
                SetPixels(width, height, TextureFormat.RGB, PixelFormat.RGB, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        void SetPixels(int w, int h, TextureFormat textureFormat, PixelFormat pixelFormat, PixelType type, IntPtr data)
        {
            Format = textureFormat;
            Width = w;
            Height = h;

            if (pixelFormat == PixelFormat.Depth)
                type = PixelType.Float;

            MakeCurrent();
            GL.TexImage2D(TextureTarget.Texture2D, 0, Format, w, h, 0, pixelFormat, type, data);
        }

        int GetParam(TextureParam p)
        {
            MakeCurrent();
            int val;
            GL.GetTexParameterI(TextureTarget.Texture2D, p, out val);
            return val;
        }

        void SetParam(TextureParam p, int val)
        {
            MakeCurrent();
            GL.TexParameterI(TextureTarget.Texture2D, p, val);
        }

        void MakeCurrent()
        {
            if (binded[0] != this)
            {
                binded[0] = this;
                GL.ActiveTexture(0);
                GL.BindTexture(TextureTarget.Texture2D, id);
            }
        }

        internal int Bind()
        {
            //If we're marked for unbinding, unmark us
            unbind.Remove(this);

            //If we're already binded, return our slot
            int i = Array.IndexOf(binded, this);
            if (i >= 0)
                return i;

            //If we're not already binded, bind us and return the slot
            for (i = 0; i < binded.Length; ++i)
            {
                if (binded[i] == null)
                {
                    binded[i] = this;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(TextureTarget.Texture2D, id);
                    return i;
                }
            }

            throw new Exception("You have exceeded the maximum amount of texture bindings: " + GL.MaxTextureUnits);
        }

        internal static void MarkAllForUnbinding()
        {
            for (int i = 0; i < binded.Length; ++i)
                if (binded[i] != null)
                    unbind.Add(binded[i]);
        }

        internal static void UnbindMarked()
        {
            for (int i = 0; i < binded.Length; ++i)
            {
                if (binded[i] != null && unbind.Contains(binded[i]))
                {
                    binded[i] = null;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
            }
        }

        internal static void UnbindAll()
        {
            unbind.Clear();
            for (int i = 0; i < binded.Length; ++i)
            {
                if (binded[i] != null)
                {
                    binded[i] = null;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                }
            }
        }
    }
}
