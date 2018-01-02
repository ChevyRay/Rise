using System;
using Rise.OpenGL;
namespace Rise
{
    public enum CubeMapSide : uint
    {
        PosX = TextureTarget.TextureCubeMapPosX,
        NegX = TextureTarget.TextureCubeMapNegX,
        PosY = TextureTarget.TextureCubeMapPosY,
        NegY = TextureTarget.TextureCubeMapNegY,
        PosZ = TextureTarget.TextureCubeMapPosZ,
        NegZ = TextureTarget.TextureCubeMapNegZ
    }

    public class CubeMapTexture : Texture
    {
        public CubeMap CubeMap { get; private set; }
        public CubeMapSide Side { get; private set; }

        internal CubeMapTexture(CubeMap cubeMap, CubeMapSide side)
            : base(cubeMap.id, cubeMap.Format, TextureTarget.TextureCubeMap, (TextureTarget)side)
        {
            CubeMap = cubeMap;
            Side = side;
        }
    }

    public class CubeMap : ResourceHandle
    {
        internal uint id;
        CubeMapTexture[] sides = new CubeMapTexture[6];

        public int Size { get; private set; }
        public TextureFormat Format { get; private set; }

        public CubeMapTexture PosX { get { return sides[0]; } }
        public CubeMapTexture NegX { get { return sides[1]; } }
        public CubeMapTexture PosY { get { return sides[2]; } }
        public CubeMapTexture NegY { get { return sides[3]; } }
        public CubeMapTexture PosZ { get { return sides[4]; } }
        public CubeMapTexture NegZ { get { return sides[5]; } }

        public CubeMap(int size, TextureFormat format)
        {
            Size = size;
            Format = format;
            id = GL.GenTexture();
            WrapX = Texture.DefaultWrapX;
            WrapY = Texture.DefaultWrapY;
            MinFilter = Texture.DefaultMinFilter;
            MagFilter = Texture.DefaultMagFilter;

            MakeCurrent();
            var pixelFormat = format.PixelFormat();
            for (int i = 0; i < 6; ++i)
            {
                sides[i] = new CubeMapTexture(this, (CubeMapSide)((int)CubeMapSide.PosX + i));
                sides[i].SetPixels(null as byte[], 1, pixelFormat);
            }
        }

        protected override void Dispose()
        {
            GL.DeleteTexture(id);
        }

        public CubeMapTexture GetSide(CubeMapSide side)
        {
            return sides[side - CubeMapSide.PosX];
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
            Texture.MakeCurrent(id, TextureTarget.TextureCubeMap);
        }
    }
}
