using System;
using System.Text;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    public class ByteReader
    {
        public int Index { get; private set; }
        public int Count { get { return bytes.Length; } }

        byte[] bytes;
        Convert16 c16;
        Convert32 c32;
        Convert64 c64;
        IntPtr buffer;
        int bufferSize;

        public ByteReader(bool littleEndian)
        {
            if (BitConverter.IsLittleEndian ? littleEndian : !littleEndian)
            {
                ReadShort = ReadShort1;
                ReadUShort = ReadUShort1;
                ReadInt = ReadInt1;
                ReadUInt = ReadUInt1;
                ReadLong = ReadLong1;
                ReadULong = ReadULong1;
            }
            else
            {
                ReadShort = ReadShort2;
                ReadUShort = ReadUShort2;
                ReadInt = ReadInt2;
                ReadUInt = ReadUInt2;
                ReadLong = ReadLong2;
                ReadULong = ReadULong2;
            }
        }
        public ByteReader(bool littleEndian, byte[] bytes) : this(littleEndian)
        {
            Init(bytes);
        }
        ~ByteReader()
        {
            if (bufferSize > 0)
                Marshal.FreeHGlobal(buffer);
        }

        public void Init(byte[] bytes)
        {
            this.bytes = bytes;
            Index = 0;
        }

        public void SkipBytes(int count)
        {
            Index += count;
        }

        public bool ReadBool()
        {
            return bytes[Index++] > 0;
        }

        public byte ReadByte()
        {
            return bytes[Index++];
        }

        public Func<short> ReadShort { get; private set; }
        short ReadShort1()
        {
            c16.Byte0 = bytes[Index++];
            c16.Byte1 = bytes[Index++];
            return c16.Short;
        }
        short ReadShort2()
        {
            c16.Byte1 = bytes[Index++];
            c16.Byte0 = bytes[Index++];
            return c16.Short;
        }

        public Func<ushort> ReadUShort { get; private set; }
        ushort ReadUShort1()
        {
            c16.Byte0 = bytes[Index++];
            c16.Byte1 = bytes[Index++];
            return c16.UShort;
        }
        ushort ReadUShort2()
        {
            c16.Byte1 = bytes[Index++];
            c16.Byte0 = bytes[Index++];
            return c16.UShort;
        }

        public Func<int> ReadInt { get; private set; }
        int ReadInt1()
        {
            c32.Byte0 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte3 = bytes[Index++];
            return c32.Int;
        }
        int ReadInt2()
        {
            c32.Byte3 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte0 = bytes[Index++];
            return c32.Int;
        }

        public Func<uint> ReadUInt { get; private set; }
        uint ReadUInt1()
        {
            c32.Byte0 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte3 = bytes[Index++];
            return c32.UInt;
        }
        uint ReadUInt2()
        {
            c32.Byte3 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte0 = bytes[Index++];
            return c32.UInt;
        }

        public Func<long> ReadLong { get; private set; }
        long ReadLong1()
        {
            c64.Byte0 = bytes[Index++];
            c64.Byte1 = bytes[Index++];
            c64.Byte2 = bytes[Index++];
            c64.Byte3 = bytes[Index++];
            c64.Byte4 = bytes[Index++];
            c64.Byte5 = bytes[Index++];
            c64.Byte6 = bytes[Index++];
            c64.Byte7 = bytes[Index++];
            return c64.Long;
        }
        long ReadLong2()
        {
            c64.Byte7 = bytes[Index++];
            c64.Byte6 = bytes[Index++];
            c64.Byte5 = bytes[Index++];
            c64.Byte4 = bytes[Index++];
            c64.Byte3 = bytes[Index++];
            c64.Byte2 = bytes[Index++];
            c64.Byte1 = bytes[Index++];
            c64.Byte0 = bytes[Index++];
            return c64.Long;
        }

        public Func<ulong> ReadULong { get; private set; }
        ulong ReadULong1()
        {
            c64.Byte0 = bytes[Index++];
            c64.Byte1 = bytes[Index++];
            c64.Byte2 = bytes[Index++];
            c64.Byte3 = bytes[Index++];
            c64.Byte4 = bytes[Index++];
            c64.Byte5 = bytes[Index++];
            c64.Byte6 = bytes[Index++];
            c64.Byte7 = bytes[Index++];
            return c64.ULong;
        }
        ulong ReadULong2()
        {
            c64.Byte7 = bytes[Index++];
            c64.Byte6 = bytes[Index++];
            c64.Byte5 = bytes[Index++];
            c64.Byte4 = bytes[Index++];
            c64.Byte3 = bytes[Index++];
            c64.Byte2 = bytes[Index++];
            c64.Byte1 = bytes[Index++];
            c64.Byte0 = bytes[Index++];
            return c64.ULong;
        }

        public float ReadFloat()
        {
            c32.Byte0 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte3 = bytes[Index++];
            return c32.Float;
        }

        public double ReadDouble()
        {
            c64.Byte0 = bytes[Index++];
            c64.Byte1 = bytes[Index++];
            c64.Byte2 = bytes[Index++];
            c64.Byte3 = bytes[Index++];
            c64.Byte4 = bytes[Index++];
            c64.Byte5 = bytes[Index++];
            c64.Byte6 = bytes[Index++];
            c64.Byte7 = bytes[Index++];
            return c64.Double;
        }

        public string ReadString()
        {
            int byteCount = ReadInt();
            if (byteCount > 0)
                return Encoding.UTF8.GetString(bytes, Index, byteCount);
            return string.Empty;
        }

        public T ReadStructRaw<T>() where T : struct
        {
            int size = Marshal.SizeOf<T>();
            if (size > bufferSize)
            {
                if (bufferSize > 0)
                    Marshal.FreeHGlobal(buffer);
                buffer = Marshal.AllocHGlobal(size);
                bufferSize = size;
            }
            Marshal.Copy(bytes, Index, buffer, size);
            var obj = Marshal.PtrToStructure<T>(buffer);
            Index += size;
            return obj;
        }
    }
}
