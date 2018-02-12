using System;
using System.Collections.Generic;
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

        public static unsafe void EncodePng(Color4[] pixels, int width, int height, List<byte> result)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");
            fixed (Color4* ptr = pixels)
            {
                result.Clear();
                convert_to_png(ptr, width, height, (ctx, data, size) =>
                {
                    for (int i = 0; i < size; ++i)
                        result.Add(data[i]);
                });
            }
        }

        public static unsafe void EncodeBmp(Color4[] pixels, int width, int height, List<byte> result)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");
            fixed (Color4* ptr = pixels)
            {
                result.Clear();
                convert_to_bmp(ptr, width, height, (ctx, data, size) =>
                {
                    for (int i = 0; i < size; ++i)
                        result.Add(data[i]);
                });
            }
        }

        public static unsafe void EncodeTga(Color4[] pixels, int width, int height, List<byte> result)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");
            fixed (Color4* ptr = pixels)
            {
                result.Clear();
                convert_to_tga(ptr, width, height, (ctx, data, size) =>
                {
                    for (int i = 0; i < size; ++i)
                        result.Add(data[i]);
                });
            }
        }

        public static unsafe void EncodeJpg(Color4[] pixels, int width, int height, int quality, List<byte> result)
        {
            if (pixels.Length < width * height)
                throw new Exception("Not enough pixels for image size.");
            fixed (Color4* ptr = pixels)
            {
                result.Clear();
                convert_to_jpg(ptr, width, height, quality, (ctx, data, size) =>
                {
                    for (int i = 0; i < size; ++i)
                        result.Add(data[i]);
                });
            }
        }
    }
}
