using System;
using System.IO;
namespace Rise.Imaging
{
    enum ChunkType : uint
    {
        IHDR = ((uint)'I' << 24) + ((uint)'H' << 16) + ((uint)'D' << 8) + 'R',
        IDAT = ((uint)'I' << 24) + ((uint)'D' << 16) + ((uint)'A' << 8) + 'T',
        IEND = ((uint)'I' << 24) + ((uint)'E' << 16) + ((uint)'N' << 8) + 'D',
    }

    enum FilterType : byte
    {
        None = 0,
        Sub = 1,
        Up = 2,
        Avg = 3,
        Paeth = 4,
        AvgFirst,
        PaethFirst
    }

    public class PngDecoder : FormatDecoder
    {
        static readonly byte[] signature = { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };

        DeflateDecoder inflater;
        byte[] compressed;
        int compressedSize;
        byte[] filtered;
        int width;
        int height;
        int bitDepth;
        int colorType;
        int compressionMethod;
        int filterMethod;
        int interlaceMethod;

        public PngDecoder() : base(".png")
        {
            
        }

        public override void Decode(byte[] source, out int w, out int h, ref Color4[] pixels)
        {
            //Parse the PNG file to get all our compressed bytes
            ParsePng(source);

            //Make sure the ZLIB data is in good form
            CheckZLibHeader();

            //Inflate the compressed bytes
            if (inflater == null)
                inflater = new DeflateDecoder();
            filtered = inflater.Decode(compressed, 2);

            //Unfilter the filtered scanlines
            Unfilter(ref pixels);

            //Return the size
            w = width;
            h = height;
        }

        void ParsePng(byte[] source)
        {
            compressedSize = 0;

            int ind = 0;
            uint ReadInt()
            {
                return ((uint)source[ind++] << 24) | ((uint)source[ind++] << 16) | ((uint)source[ind++] << 8) | (uint)source[ind++];
            }

            //Parse the PNG signature
            for (int i = 0; i < signature.Length; ++i)
                if (source[ind++] != signature[i])
                    throw new Exception("invalid PNG signature");

            //Read header chunk
            var chunkLength = (int)ReadInt();
            var chunkType = (ChunkType)ReadInt();
            if (chunkType != ChunkType.IHDR)
                throw new Exception("IHDR chunk is not first");

            //Read the size
            width = (int)ReadInt();
            height = (int)ReadInt();
            //Console.WriteLine("width: " + width);
            //Console.WriteLine("height: " + height);

            //Read the bit depth
            bitDepth = source[ind++];
            if (bitDepth != 8)
                throw new Exception("bit depth not supported: " + bitDepth);
            //Console.WriteLine("bitDepth: " + bitDepth);

            //Read the color type
            colorType = source[ind++];
            if (colorType != 2 && colorType != 6)
                throw new Exception("color type not supported: " + colorType);
            //Console.WriteLine("colorType: " + colorType);

            //Read the compression method
            compressionMethod = source[ind++];
            if (compressionMethod != 0)
                throw new Exception("compression method not supported: " + compressionMethod);
            //Console.WriteLine("compressionMethod: " + compressionMethod);

            //Read the filter method
            filterMethod = source[ind++];
            if (filterMethod != 0)
                throw new Exception("filter method not supported: " + filterMethod);
            //Console.WriteLine("filterMethod: " + filterMethod);

            //Read the interlace method
            interlaceMethod = source[ind++];
            if (interlaceMethod != 0)
                throw new Exception("interlace method not supported: " + interlaceMethod);
            //Console.WriteLine("interlaceMethod: " + interlaceMethod);

            //Skip the CRC
            ind += 4;

            //Scan ahead to find out the compressed size
            int prevInd = ind;
            int compLen = 0;
            chunkLength = (int)ReadInt();
            chunkType = (ChunkType)ReadInt();
            while (chunkType != ChunkType.IEND)
            {
                if (chunkType == ChunkType.IDAT)
                    compLen += chunkLength;
                ind += chunkLength + 4;
                chunkLength = (int)ReadInt();
                chunkType = (ChunkType)ReadInt();
            }
            ind = prevInd;

            //Make sure the stream ends on an IEND block
            if (chunkType != ChunkType.IEND)
                throw new Exception("PNG must end with IEND chunk");

            //Make sure we actually had data
            if (compLen <= 0)
                throw new Exception("no IDAT data to decode");

            //Make sure the compressed array is large enough
            if (compressed == null)
                compressed = new byte[compLen];
            else if (compressed.Length < compLen)
                Array.Resize(ref compressed, compLen);

            //Read all the IDAT chunks
            chunkLength = (int)ReadInt();
            chunkType = (ChunkType)ReadInt();
            while (chunkType != ChunkType.IEND)
            {
                if (chunkType == ChunkType.IDAT)
                {
                    Buffer.BlockCopy(source, ind, compressed, compressedSize, chunkLength);
                    compressedSize += chunkLength;
                }
                ind += chunkLength + 4;
                chunkLength = (int)ReadInt();
                chunkType = (ChunkType)ReadInt();
            }
        }

        void CheckZLibHeader()
        {
            byte cmf = compressed[0];
            byte flg = compressed[1];

            //Check checksum
            if (((256 * cmf + flg) % 31) != 0)
                throw new Exception("invalid checksum");

            //Check method
            if ((cmf & 0x0f) != 8)
                throw new Exception("invalid compression method: " + (cmf & 0x0f));

            //Check window size
            if ((cmf >> 4) > 7)
                throw new Exception("invalid window size: " + (cmf >> 4));

            //Make sure there's no preset dictionary
            if ((flg & 0x20) != 0)
                throw new Exception("preset dictionary not allowed");
        }

        static byte ByteCast(int val)
        {
            return (byte)(val & 255);
        }

        static int Paeth(int a, int b, int c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);
            if (pa <= pb && pa <= pc)
                return a;
            if (pb <= pc)
                return b;
            return c;
        }

        unsafe void Unfilter(ref Color4[] pixels)
        {
            int bpp = colorType == 2 ? 3 : 4;
            const int rgba = 4;
            int bdiff = rgba - bpp;
            int pixelCount = width * height;

            if (pixels == null)
                pixels = new Color4[pixelCount];
            else if (pixels.Length < pixelCount)
                Array.Resize(ref pixels, pixelCount);

            //If we're loading a RGB image with no alpha channel, load with full opacity
            if (bpp < rgba)
                for (int i = 0; i < pixelCount; ++i)
                    pixels[i] = Color4.White;

            fixed (byte* cur = &pixels[0].R)
            {
                var raw = filtered;
                int rawi = 0;
                int curi = 0;
                int prei = curi - (width * 4);

                //For each scanline
                for (int y = 0; y < height; ++y)
                {
                    var filter = (FilterType)raw[rawi++];
                    //Console.WriteLine(y + ": " + filter);

                    //Use special filters for the first scanline
                    if (y == 0)
                    {
                        switch (filter)
                        {
                            case FilterType.Up:
                                filter = FilterType.None;
                                break;
                            case FilterType.Avg:
                                filter = FilterType.AvgFirst;
                                break;
                            case FilterType.Paeth:
                                filter = FilterType.PaethFirst;
                                break;
                        }
                    }

                    //Handle the first pixel on the scanline
                    switch (filter)
                    {
                        case FilterType.None:
                        case FilterType.Sub:
                        case FilterType.AvgFirst:
                        case FilterType.PaethFirst:
                            for (int i = 0; i < bpp; ++i)
                                cur[curi + i] = raw[rawi + i];
                            break;
                        case FilterType.Up:
                            for (int i = 0; i < bpp; ++i)
                                cur[curi + i] = ByteCast(raw[rawi + i] + cur[prei + i]);
                            break;
                        case FilterType.Avg:
                            for (int i = 0; i < bpp; ++i)
                                cur[curi + i] = ByteCast(raw[rawi + i] + (cur[prei + i] >> 1));
                            break;
                        case FilterType.Paeth:
                            for (int i = 0 ; i < bpp; ++i)
                                cur[curi + i] = ByteCast(raw[rawi + i] + Paeth(0, cur[prei + i], 0));
                            break;
                    }
                    rawi += bpp;
                    curi += rgba;
                    prei += rgba;

                    //Handle the rest of the pixels on the scanline
                    switch (filter)
                    {
                        case FilterType.None:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = raw[rawi];
                            break;
                        case FilterType.Sub:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + cur[curi - rgba]);
                            break;
                        case FilterType.Up:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + cur[prei]);
                            break;
                        case FilterType.Avg:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + ((cur[prei] + cur[curi - rgba]) >> 1));
                            break;
                        case FilterType.Paeth:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + Paeth(cur[curi - rgba], cur[prei], cur[prei - rgba]));
                            break;
                        case FilterType.AvgFirst:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + (cur[curi - rgba] >> 1));
                            break;
                        case FilterType.PaethFirst:
                            for (int i = 0, n = width - 1; i < n; ++i, curi += bdiff, prei += bdiff)
                                for (int j = 0; j < bpp; ++j, ++curi, ++prei, ++rawi)
                                    cur[curi] = ByteCast(raw[rawi] + Paeth(cur[curi - rgba], 0, 0));
                            break;
                    }
                }
            }
        }
    }
}
