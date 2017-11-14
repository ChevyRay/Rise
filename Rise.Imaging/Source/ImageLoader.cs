using System;
using System.IO;
using System.Collections.Generic;
namespace Rise.Imaging
{
    public class ImageLoader
    {
        Dictionary<string, FormatDecoder> decoders = new Dictionary<string, FormatDecoder>(StringComparer.OrdinalIgnoreCase);

        public ImageLoader()
        {
            AddDecoder(new PngDecoder());
        }

        public void AddDecoder(FormatDecoder decoder)
        {
            for (int i = 0; i < decoder.Extensions.Length; ++i)
            {
                if (decoders.ContainsKey(decoder.Extensions[i]))
                    throw new Exception("ImageLoader already has decoder for extension: " + decoder.Extensions[i]);
                decoders[decoder.Extensions[i]] = decoder;
            }
        }

        public void LoadFile(string file, ref Bitmap bitmap)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (!File.Exists(file))
                throw new FileNotFoundException("File not found.", file);

            var ext = Path.GetExtension(file);
            FormatDecoder decoder;
            if (!decoders.TryGetValue(ext, out decoder))
                throw new Exception("Could not load image, there is no decoder for extension: " + ext);

            var bytes = File.ReadAllBytes(file);
            decoder.Decode(bytes, ref bitmap);
        }
        public Bitmap LoadFile(string file)
        {
            Bitmap bitmap = null;
            LoadFile(file, ref bitmap);
            return bitmap;
        }
    }
}
