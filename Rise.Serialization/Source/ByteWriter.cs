using System;
using System.Text;
using System.Runtime.InteropServices;
namespace Rise.Serialization
{
    public class ByteWriter
    {
        byte[] bytes;
        public int Count { get; private set; }

        byte[] stringBytes;
        Convert16 c16;
        Convert32 c32;
        Convert64 c64;

        IntPtr buffer;
        int bufferSize;

        public ByteWriter(int initCapacity)
        {
            bytes = new byte[initCapacity];
        }
        public ByteWriter() : this(4)
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
        public void Write(short value)
        {
            Reserve(2);
            c16.Short = value;
            bytes[Count++] = c16.Byte0;
            bytes[Count++] = c16.Byte1;
        }
        public void Write(ushort value)
        {
            Reserve(2);
            c16.UShort = value;
            bytes[Count++] = c16.Byte0;
            bytes[Count++] = c16.Byte1;
        }
        public void Write(int value)
        {
            Reserve(4);
            c32.Int = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }
        public void Write(uint value)
        {
            Reserve(4);
            c32.UInt = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }
        public void Write(long value)
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
        public void Write(ulong value)
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
        public void Write(float value)
        {
            Reserve(4);
            c32.Float = value;
            bytes[Count++] = c32.Byte0;
            bytes[Count++] = c32.Byte1;
            bytes[Count++] = c32.Byte2;
            bytes[Count++] = c32.Byte3;
        }
        public void Write(double value)
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
        public void Write(byte[] bytes, int count)
        {
            Reserve(count);
            Buffer.BlockCopy(bytes, 0, this.bytes, Count, count);
            Count += count;
        }
        public void Write(byte[] bytes)
        {
            Write(bytes, bytes.Length);
        }
        public void Write(ref string value)
        {
            //If the string is null, write that it's empty
            if (value == null)
            {
                Write(0);
                return;
            }

            //Write how many bytes are in the string
            int byteCount = Encoding.UTF8.GetByteCount(value);
            Write(byteCount);

            //Make sure the string byte array can hold the string value
            if (stringBytes == null)
                stringBytes = new byte[byteCount];
            else if (byteCount > stringBytes.Length)
                Array.Resize(ref stringBytes, byteCount);
            
            //Write the bytes
            Encoding.UTF8.GetBytes(value, 0, value.Length, stringBytes, 0);
            Write(stringBytes, byteCount);
        }
        public void Write(string value)
        {
            Write(ref value);
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
