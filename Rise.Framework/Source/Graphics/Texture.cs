using System;
using System.Collections.Generic;
using Rise.OpenGL;
namespace Rise
{
    public abstract class Texture : ResourceHandle
    {
        struct Binding
        {
            public uint ID;
            public TextureTarget Target;
        }

        static Binding[] binded = new Binding[GL.MaxTextureUnits];
        static HashSet<uint> unbind = new HashSet<uint>();

        public static TextureFilter DefaultMinFilter = TextureFilter.Linear;
        public static TextureFilter DefaultMagFilter = TextureFilter.Linear;
        public static TextureWrap DefaultWrapX = TextureWrap.ClampToEdge;
        public static TextureWrap DefaultWrapY = TextureWrap.ClampToEdge;

        internal uint ID { get; private set; }
        public TextureFormat Format { get; private set; }
        internal TextureTarget BindTarget { get; private set; }
        internal TextureTarget DataTarget { get; private set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        internal Texture(uint id, TextureFormat format, TextureTarget bindTarget, TextureTarget dataTarget)
        {
            ID = id;
            Format = format;
            BindTarget = bindTarget;
            DataTarget = dataTarget;
        }

        protected override void Dispose()
        {
            
        }

        public void SetPixels(Bitmap bitmap)
        {
            if (Width != bitmap.Width || Height != bitmap.Height)
                throw new Exception("Bitmap size does not match texture.");
            SetPixels(bitmap.Pixels);
        }
        public unsafe void SetPixels(Color4[] pixels)
        {
            if (pixels.Length < Width * Height)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (Color4* ptr = pixels)
            GL.TexImage2D(DataTarget, 0, Format, Width, Height, 0, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        public unsafe void SetPixels(Color3[] pixels)
        {
            if (pixels.Length < Width * Height)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (Color3* ptr = pixels)
            GL.TexImage2D(DataTarget, 0, Format, Width, Height, 0, PixelFormat.RGB, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        internal unsafe void SetPixels(byte[] pixels, int comp, PixelFormat format)
        {
            if (pixels != null && pixels.Length < Width * Height * comp)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (byte* ptr = pixels)
            GL.TexImage2D(DataTarget, 0, Format, Width, Height, 0, format, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        internal unsafe void SetPixels(float[] pixels, int comp, PixelFormat format)
        {
            if (pixels != null && pixels.Length < Width * Height * comp)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (float* ptr = pixels)
            GL.TexImage2D(DataTarget, 0, Format, Width, Height, 0, format, PixelType.Float, new IntPtr(ptr));
        }

        public void SetPixelsRGBA(byte[] pixels)
        {
            SetPixels(pixels, 4, PixelFormat.RGBA);
        }
        public void SetPixelsRGBA(float[] pixels)
        {
            SetPixels(pixels, 4, PixelFormat.RGBA);
        }

        public void SetPixelsRGB(byte[] pixels)
        {
            SetPixels(pixels, 3, PixelFormat.RGB);
        }
        public void SetPixelsRGB(float[] pixels)
        {
            SetPixels(pixels, 3, PixelFormat.RGB);
        }

        public void SetPixelsRG(byte[] pixels)
        {
            SetPixels(pixels, 2, PixelFormat.RG);
        }
        public void SetPixelsRG(float[] pixels)
        {
            SetPixels(pixels, 2, PixelFormat.RG);
        }

        public void SetPixelsR(byte[] pixels)
        {
            SetPixels(pixels, 1, PixelFormat.R);
        }
        public void SetPixelsR(float[] pixels)
        {
            SetPixels(pixels, 1, PixelFormat.R);
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
            GL.GetTexImage(DataTarget, 0, format, PixelType.UnsignedByte, new IntPtr(ptr));
        }
        unsafe void GetPixels(ref float[] pixels, int comp, PixelFormat format)
        {
            if (pixels == null)
                pixels = new float[Width * Height * comp];
            else if (pixels.Length < Width * Height * comp)
                throw new Exception("Pixels array is not large enough.");
            MakeCurrent();
            fixed (float* ptr = pixels)
            GL.GetTexImage(DataTarget, 0, format, PixelType.Float, new IntPtr(ptr));
        }

        public unsafe Color4[] GetPixels()
        {
            var pixels = new Color4[Width * Height];
            MakeCurrent();
            fixed (Color4* ptr = pixels)
            GL.GetTexImage(DataTarget, 0, PixelFormat.RGBA, PixelType.UnsignedByte, new IntPtr(ptr));
            return pixels;
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

        internal void MakeCurrent()
        {
            MakeCurrent(ID, BindTarget);
        }
        internal static void MakeCurrent(uint id, TextureTarget target)
        {
            if (binded[0].ID != id)
            {
                GL.ActiveTexture(0);

                //If a texture is already binded to slot 0, unbind it
                if (binded[0].ID != 0 && binded[0].Target != target)
                    GL.BindTexture(binded[0].Target, 0);

                binded[0].ID = id;
                binded[0].Target = target;
                GL.BindTexture(target, id);
            }
        }

        internal static int Bind(uint id, TextureTarget target)
        {
            //If we're marked for unbinding, unmark us
            unbind.Remove(id);

            //If we're already binded, return our slot
            for (int i = 0; i < binded.Length; ++i)
                if (binded[i].ID == id)
                    return i;

            //If we're not already binded, bind us and return the slot
            for (int i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID == 0)
                {
                    binded[i].ID = id;
                    binded[i].Target = target;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(target, id);
                    return i;
                }
            }

            throw new Exception("You have exceeded the maximum amount of texture bindings: " + GL.MaxTextureUnits);
        }

        internal static void MarkAllForUnbinding()
        {
            for (int i = 0; i < binded.Length; ++i)
                if (binded[i].ID != 0)
                    unbind.Add(binded[i].ID);
        }

        internal static void UnbindMarked()
        {
            for (uint i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID != 0 && unbind.Contains(binded[i].ID))
                {
                    GL.ActiveTexture(i);
                    GL.BindTexture(binded[i].Target, 0);
                    binded[i].ID = 0;
                }
            }
            unbind.Clear();
        }

        internal static void UnbindAll()
        {
            unbind.Clear();
            for (uint i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID != 0)
                {
                    GL.ActiveTexture(i);
                    GL.BindTexture(binded[i].Target, 0);
                    binded[i].ID = 0;
                }
            }
        }
    }
}
