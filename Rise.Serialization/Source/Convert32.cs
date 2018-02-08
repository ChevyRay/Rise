using System;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Convert32
    {
        [FieldOffset(0)]
        public int Int;

        [FieldOffset(0)]
        public uint UInt;

        [FieldOffset(0)]
        public float Float;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;
    }
}
