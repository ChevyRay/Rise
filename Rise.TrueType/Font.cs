using System;
using System.IO;
using System.Collections.Generic;
using Rise.Serialization;
namespace Rise.TrueType
{
    public enum FontType
    {
        TrueType,
        OpenType
    }

    public class Font
    {
        public FontType Type { get; private set; }

        public Font(string file)
        {
            LoadFile(file);
        }

        void LoadFile(string file)
        {
            var reader = new ByteReader(false, File.ReadAllBytes(file));

            //Load the font type (truetype or opentype)
            uint sfntVersion = reader.ReadUInt();
            if (sfntVersion == 0x00010000)
                Type = FontType.TrueType;
            else if (sfntVersion == 0x4F54544F)
                Type = FontType.OpenType;
            else
                throw new Exception("Invalid sfntVersion.");

            int numTables = reader.ReadUShort();
            Console.WriteLine("numTables: " + numTables);
        }
    }
}
