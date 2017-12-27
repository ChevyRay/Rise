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
        public Texture(int width, int height, Color[] pixels) : this(TextureFormat.RGBA)
        {
            SetPixels(width, height, pixels);
        }
        public Texture(int width, int height, TextureFormat format) : this(format)
        {
            SetSize(width, height);
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

        public unsafe void GetPixels(Bitmap bitmap)
        {
            if (Width != bitmap.Width)
                throw new Exception("Bitmap width does not match.");
            if (Height != bitmap.Height)
                throw new Exception("Bitmap height does not match.");

            GL.BindTexture(TextureTarget.Texture2D, id);
            fixed (Color* ptr = bitmap.Pixels)
            {
                GL.GetTexImage(TextureTarget.Texture2D, 0, TextureFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        public Bitmap GetPixels()
        {
            var bitmap = new Bitmap(Width, Height);
            GetPixels(bitmap);
            return bitmap;
        }

        public void SetPixels(Bitmap bitmap)
        {
            SetPixels(bitmap.Width, bitmap.Height, bitmap.Pixels);
        }
        public unsafe void SetPixels(int width, int height, Color[] pixels)
        {
            Format = TextureFormat.RGBA;

            if (pixels == null)
            {
                SetPixels(width, height, PixelFormat.RGBA, PixelType.UnsignedByte, IntPtr.Zero);
                return;
            }

            if (pixels.Length < (width * height))
                throw new ArgumentException("Pixels array is too small.", nameof(pixels));

            fixed (Color* ptr = pixels)
                SetPixels(width, height, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public unsafe void SetPixels(int width, int height, PixelFormat format, byte[] pixels)
        {
            if (pixels.Length < width * height * format.Size())
                throw new Exception("Pixels array is too small.");
            fixed (byte* ptr = pixels)
                SetPixels(width, height, format, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        void SetPixels(int w, int h, PixelFormat format, PixelType type, IntPtr data)
        {
            Width = w;
            Height = h;
            MakeCurrent();
            GL.TexImage2D(TextureTarget.Texture2D, 0, Format, w, h, 0, format, type, data);
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            MakeCurrent();
            GL.TexImage2D(TextureTarget.Texture2D, 0, Format, width, height, 0, PixelFormat.RGBA, PixelType.UnsignedByte, IntPtr.Zero);
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
