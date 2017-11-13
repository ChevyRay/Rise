using System;
namespace Rise.Imaging
{
    public abstract class FormatDecoder
    {
        public readonly string[] Extensions;

        protected FormatDecoder(params string[] extensions)
        {
            Extensions = extensions;
        }

        public abstract void Decode(byte[] source, out int w, out int h, ref Color[] pixels);

        public Color[] Decode(byte[] source, out int w, out int h)
        {
            Color[] pixels = null;
            Decode(source, out w, out h, ref pixels);
            return pixels;
        }

        public Bitmap Decode(byte[] source)
        {
            int w, h;
            var pixels = Decode(source, out w, out h);
            return new Bitmap(pixels, w, h);
        }

        public void Decode(byte[] source, ref Bitmap bitmap)
        {
            if (bitmap != null)
            {
                int w, h;
                Decode(source, out w, out h, ref bitmap.pixels);
                bitmap.SetPixels(bitmap.pixels, w, h);
            }
            else
                bitmap = Decode(source);
        }
    }
}
