using System;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Convert64
    {
        [FieldOffset(0)]
        public long Long;

        [FieldOffset(0)]
        public ulong ULong;

        [FieldOffset(0)]
        public double Double;

        [FieldOffset(0)]
        public byte Byte0;

        [FieldOffset(1)]
        public byte Byte1;

        [FieldOffset(2)]
        public byte Byte2;

        [FieldOffset(3)]
        public byte Byte3;

        [FieldOffset(4)]
        public byte Byte4;

        [FieldOffset(5)]
        public byte Byte5;

        [FieldOffset(6)]
        public byte Byte6;

        [FieldOffset(7)]
        public byte Byte7;
    }
}
