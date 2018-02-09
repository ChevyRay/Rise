using System;
using System.Text;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    public class ByteWriter
    {
        public bool LittleEndian { get; private set; }
        public int Count { get; private set; }

        byte[] bytes;
        byte[] stringBytes;
        Convert16 c16;
        Convert32 c32;
        Convert64 c64;
        IntPtr buffer;
        int bufferSize;

        public ByteWriter(bool littleEndian, int initCapacity)
        {
            LittleEndian = littleEndian;
            bytes = new byte[initCapacity];

            if (BitConverter.IsLittleEndian ? littleEndian : !littleEndian)
            {
                WriteShort = WriteShort1;
                WriteUShort = WriteUShort1;
                WriteInt = WriteInt1;
                WriteUInt = WriteUInt1;
                WriteLong = WriteLong1;
                WriteULong = WriteULong1;
            }
            else
            {
                WriteShort = WriteShort2;
                WriteUShort = WriteUShort2;
                WriteInt = WriteInt2;
                WriteUInt = WriteUInt2;
                WriteLong = WriteLong2;
                WriteULong = WriteULong2;
            }
        }
        public ByteWriter(bool littleEndian) : this(littleEndian, 4)
        {
            
        }
        ~ByteWriter()
        {
            if (bufferSize > 0)
                Marshal.FreeHGlobal(buffer);
        }

        public void Clear()
        {
            Count = 0;
        }

        public byte this[int i]
        {
            get { return bytes[i]; }
            set { bytes[i] = value; }
        }

        public byte[] GetBytes()
        {
            var arr = new byte[Count];
            Buffer.BlockCopy(bytes, 0, arr, 0, Count);
            return arr;
        }

        void Reserve(int count)
        {
            count += Count;
            if (count > bytes.Length)
            {
                int len = bytes.Length;
                while (count > len)
                    len *= 2;
                Array.Resize(ref bytes, len);
            }
        }

        public void Write(bool value)
        {
            Reserve(1);
            bytes[Count++] = (byte)(value ? 1 : 0);
        }

        public void Write(byte value)
        {
            Reserve(1);
            bytes[Count++] = value;
        }

        public Action<short> WriteShort { get; private set; }
        void WriteShort1(short value)
        {
            Reserve(2);
            c16.Short = value;
            bytes[Count++] = c16.Byte0;
            bytes[Count++] = c16.Byte1;
        }
        void WriteShort2(short value)
        {
            Reserve(2);
            c16.Short = value;
            bytes[Count++] = c16.Byte1;
            bytes[Count++] = c16.Byte0;
        }

        public Action<ushort> WriteUShort { get; private set; }
        void WriteUShort1(ushort value)
        {
            Reserve(2);
            c16.UShort = value;
            bytes[Count++] = c16.Byte0;
            bytes[Count++] = c16.Byte1;
        }
        void WriteUShort2(ushort value)
        {
            Reserve(2);
            c16.UShort = value;
            bytes[Count++] = c16.Byte1;
            bytes[Count++] = c16.Byte0;
        }

        public Action<int> WriteInt { get; private set; }
        void WriteInt1(int value)
        {
            Reserve(4);
            c32.Int = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }
        void WriteInt2(int value)
        {
            Reserve(4);
            c32.Int = value;
            bytes[Count++] = c32.Byte3;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte0;
        }

        public Action<uint> WriteUInt { get; private set; }
        void WriteUInt1(uint value)
        {
            Reserve(4);
            c32.UInt = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }
        void WriteUInt2(uint value)
        {
            Reserve(4);
            c32.UInt = value;
            bytes[Count++] = c32.Byte3;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte0;
        }

        public Action<long> WriteLong { get; private set; }
        void WriteLong1(long value)
        {
            Reserve(8);
            c64.Long = value;
            bytes[Count++] = c64.Byte0;
            bytes[Count++] = c64.Byte1;
            bytes[Count++] = c64.Byte2;
            bytes[Count++] = c64.Byte3;
            bytes[Count++] = c64.Byte4;
            bytes[Count++] = c64.Byte5;
            bytes[Count++] = c64.Byte6;
            bytes[Count++] = c64.Byte7;
        }
        void WriteLong2(long value)
        {
            Reserve(8);
            c64.Long = value;
            bytes[Count++] = c64.Byte7;
            bytes[Count++] = c64.Byte6;
            bytes[Count++] = c64.Byte5;
            bytes[Count++] = c64.Byte4;
            bytes[Count++] = c64.Byte3;
            bytes[Count++] = c64.Byte2;
            bytes[Count++] = c64.Byte1;
            bytes[Count++] = c64.Byte0;
        }

        public Action<ulong> WriteULong { get; private set; }
        void WriteULong1(ulong value)
        {
            Reserve(8);
            c64.ULong = value;
            bytes[Count++] = c64.Byte0;
            bytes[Count++] = c64.Byte1;
            bytes[Count++] = c64.Byte2;
            bytes[Count++] = c64.Byte3;
            bytes[Count++] = c64.Byte4;
            bytes[Count++] = c64.Byte5;
            bytes[Count++] = c64.Byte6;
            bytes[Count++] = c64.Byte7;
        }
        void WriteULong2(ulong value)
        {
            Reserve(8);
            c64.ULong = value;
            bytes[Count++] = c64.Byte7;
            bytes[Count++] = c64.Byte6;
            bytes[Count++] = c64.Byte5;
            bytes[Count++] = c64.Byte4;
            bytes[Count++] = c64.Byte3;
            bytes[Count++] = c64.Byte2;
            bytes[Count++] = c64.Byte1;
            bytes[Count++] = c64.Byte0;
        }

        public void WriteFloat(float value)
        {
            Reserve(4);
            c32.Float = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }

        public void WriteDouble(double value)
        {
            Reserve(4);
            c64.Double = value;
            bytes[Count++] = c64.Byte0;
            bytes[Count++] = c64.Byte1;
            bytes[Count++] = c64.Byte2;
            bytes[Count++] = c64.Byte3;
            bytes[Count++] = c64.Byte4;
            bytes[Count++] = c64.Byte5;
            bytes[Count++] = c64.Byte6;
            bytes[Count++] = c64.Byte7;
        }

        public void WriteBytes(byte[] bytes, int count)
        {
            Reserve(count);
            Buffer.BlockCopy(bytes, 0, this.bytes, Count, count);
            Count += count;
        }
        public void WriteBytes(byte[] bytes)
        {
            WriteBytes(bytes, bytes.Length);
        }

        public void WriteString(ref string value)
        {
            //If the string is null, write that it's empty
            if (value == null)
            {
                WriteInt(0);
                return;
            }

            //Write how many bytes are in the string
            int byteCount = Encoding.UTF8.GetByteCount(value);
            WriteInt(byteCount);

            //Make sure the string byte array can hold the string value
            if (stringBytes == null)
                stringBytes = new byte[byteCount];
            else if (byteCount > stringBytes.Length)
                Array.Resize(ref stringBytes, byteCount);
            
            //Write the bytes
            Encoding.UTF8.GetBytes(value, 0, value.Length, stringBytes, 0);
            WriteBytes(stringBytes, byteCount);
        }
        public void WriteString(string value)
        {
            WriteString(ref value);
        }

        public void WriteStructRaw(object obj)
        {
            int size = Marshal.SizeOf(obj);
            if (size > bufferSize)
            {
                if (bufferSize > 0)
                    Marshal.FreeHGlobal(buffer);
                buffer = Marshal.AllocHGlobal(size);
                bufferSize = size;
            }
            Marshal.StructureToPtr(obj, buffer, false);
            Reserve(size);
            Marshal.Copy(buffer, bytes, Count, size);
            Count += size;
        }
    }
}
