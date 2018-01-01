using System;
using Rise.OpenGL;
namespace Rise
{
    public class CubeMap : ResourceHandle
    {
        internal uint id;

        public int Size { get; private set; }
        public TextureFormat Format { get; private set; }

        public CubeMap(int size, TextureFormat format)
        {
            Size = size;
            Format = format;
            id = GL.GenTexture();
            WrapX = Texture.DefaultWrapX;
            WrapY = Texture.DefaultWrapY;
            MinFilter = Texture.DefaultMinFilter;
            MagFilter = Texture.DefaultMagFilter;

            //Populate the cube map
            var pixelFormat = format.PixelFormat();
            GL.TexImage2D(TextureTarget.TextureCubeMapPosX, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegX, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapPosY, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegY, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapPosZ, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegZ, 0, Format, Size, Size, 0, pixelFormat, PixelType.UnsignedByte, IntPtr.Zero);
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

        int GetParam(TextureParam p)
        {
            MakeCurrent();
            int val;
            GL.GetTexParameterI(TextureTarget.TextureCubeMap, p, out val);
            return val;
        }

        void SetParam(TextureParam p, int val)
        {
            MakeCurrent();
            GL.TexParameterI(TextureTarget.TextureCubeMap, p, val);
        }

        void MakeCurrent()
        {
            TextureBindings.MakeCurrent(id, TextureTarget.TextureCubeMap);
        }
    }
}
