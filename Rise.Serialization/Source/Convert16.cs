using System;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Convert16
    {
        [FieldOffset(0)]
        public short Short;

        [FieldOffset(0)]
        public ushort UShort;

        [FieldOffset(0)]
        public float Float;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;
    }
}
