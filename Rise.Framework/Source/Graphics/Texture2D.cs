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
            Width = bitmap.Width;
            Height = bitmap.Height;
            SetPixels(bitmap);
        }
        public Texture2D(string file, bool premultiply) : this(TextureFormat.RGBA)
        {
            var bitmap = App.ImageLoader.LoadFile(file);
            if (premultiply)
                bitmap.Premultiply();
            Width = bitmap.Width;
            Height = bitmap.Height;
            SetPixels(bitmap);
        }
        public Texture2D(int width, int height, Color4[] pixels) : this(TextureFormat.RGBA)
        {
            Width = width;
            Height = height;
            SetPixels(pixels);
        }
        public Texture2D(int width, int height, Color3[] pixels) : this(TextureFormat.RGB)
        {
            Width = width;
            Height = height;
            SetPixels(pixels);
        }
        public Texture2D(int width, int height, TextureFormat format) : this(format)
        {
            Width = width;
            Height = height;
            SetPixels(null as byte[], 1, format.PixelFormat());
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
