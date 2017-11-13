using System;
using System.Collections.Generic;
namespace Rise.Imaging
{
    public class ImageDecoder
    {
        Dictionary<string, FormatDecoder> decoders = new Dictionary<string, FormatDecoder>(StringComparer.OrdinalIgnoreCase);

        public ImageDecoder()
        {
            
        }
    }
}
