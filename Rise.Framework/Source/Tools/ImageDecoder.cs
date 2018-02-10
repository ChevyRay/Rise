using System;
using System.Runtime.InteropServices;
namespace Rise
{
    public static class ImageDecoder
    {
        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern byte* load_image(byte* data, int length, out int w, out int h);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void free_image(byte* image);

        public static unsafe Color4[] Decode(byte[] data, out int width, out int height)
        {
            fixed (byte* ptr = data)
            {
                byte* image = load_image(ptr, data.Length, out width, out height);

                var pixels = new Color4[width * height];
                for (int i = 0, j = 0; i < pixels.Length; ++i)
                {
                    pixels[i].R = image[j++];
                    pixels[i].G = image[j++];
                    pixels[i].B = image[j++];
                    pixels[i].A = image[j++];
                }

                free_image(image);
                return pixels;
            }
        }
    }
}
