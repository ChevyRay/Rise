using System;
using System.Text;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    public class ByteReader
    {
        byte[] bytes;
        public int Index { get; private set; }
        public int Count { get { return bytes.Length; } }

        byte[] stringBytes;
        Convert16 c16;
        Convert32 c32;
        Convert64 c64;

        IntPtr buffer;
        int bufferSize;

        public ByteReader()
        {
            
        }
        public ByteReader(byte[] bytes)
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

        public bool ReadBool()
        {
            return bytes[Index++] > 0;
        }

        public byte ReadByte()
        {
            return bytes[Index++];
        }

        public short ReadShort()
        {
            c16.Byte0 = bytes[Index++];
            c16.Byte1 = bytes[Index++];
            return c16.Short;
        }

        public ushort ReadUShort()
        {
            c16.Byte0 = bytes[Index++];
            c16.Byte1 = bytes[Index++];
            return c16.UShort;
        }

        public int ReadInt()
        {
            c32.Byte0 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte3 = bytes[Index++];
            return c32.Int;
        }

        public uint ReadUInt()
        {
            c32.Byte0 = bytes[Index++];
            c32.Byte1 = bytes[Index++];
            c32.Byte2 = bytes[Index++];
            c32.Byte3 = bytes[Index++];
            return c32.UInt;
        }

        public long ReadLong()
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

        public ulong ReadULong()
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
            return Encoding.UTF8.GetString(bytes, Index, byteCount);
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
