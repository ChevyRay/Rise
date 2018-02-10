using System;
using System.Runtime.InteropServices;
namespace Rise
{
    public static class ImageEncoder
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate void WriteFunc(IntPtr context, byte* data, int size);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void convert_to_png(Color4* pixels, int w, int h, WriteFunc func);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void convert_to_bmp(Color4* pixels, int w, int h, WriteFunc func);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void convert_to_tga(Color4* pixels, int w, int h, WriteFunc func);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void convert_to_jpg(Color4* pixels, int w, int h, int quality, WriteFunc func);

        public static unsafe byte[] EncodePng(Color4[] pixels, int width, int height)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");

            fixed (Color4* ptr = pixels)
            {
                byte[] result = null;
                convert_to_png(ptr, width, height, (ctx, data, size) =>
                {
                    result = new byte[size];
                    for (int i = 0; i < size; ++i)
                        result[i] = data[i];
                });
                if (result == null)
                    throw new Exception("Failed to encode PNG.");
                return result;
            }
        }

        public static unsafe byte[] EncodeBmp(Color4[] pixels, int width, int height)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");

            fixed (Color4* ptr = pixels)
            {
                byte[] result = null;
                convert_to_bmp(ptr, width, height, (ctx, data, size) =>
                {
                    result = new byte[size];
                    for (int i = 0; i < size; ++i)
                        result[i] = data[i];
                });
                if (result == null)
                    throw new Exception("Failed to encode BMP.");
                return result;
            }
        }

        public static unsafe byte[] EncodeTga(Color4[] pixels, int width, int height)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");

            fixed (Color4* ptr = pixels)
            {
                byte[] result = null;
                convert_to_tga(ptr, width, height, (ctx, data, size) =>
                {
                    result = new byte[size];
                    for (int i = 0; i < size; ++i)
                        result[i] = data[i];
                });
                if (result == null)
                    throw new Exception("Failed to encode TGA.");
                return result;
            }
        }

        public static unsafe byte[] EncodeJpg(Color4[] pixels, int width, int height, int quality)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");
            if (quality < 1 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be in range 1-100.");

            fixed (Color4* ptr = pixels)
            {
                byte[] result = null;
                convert_to_jpg(ptr, width, height, quality, (ctx, data, size) =>
                {
                    result = new byte[size];
                    for (int i = 0; i < size; ++i)
                        result[i] = data[i];
                });
                if (result == null)
                    throw new Exception("Failed to encode JPG.");
                return result;
            }
        }
    }
}
