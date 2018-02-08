using System;
using System.Text;
namespace Rise.Serialization
{
    public class ByteWriter
    {
        byte[] bytes;
        public int Count { get; private set; }

        byte[] stringBytes;

        public ByteWriter(int initCapacity)
        {
            bytes = new byte[initCapacity];
        }
        public ByteWriter() : this(4)
        {
            
        }

        void Reserve(int count)
        {
            count += Count;
            if (count > bytes.Length)
            {
                int len = bytes.Length;
                while (count > bytes.Length)
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
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(ushort value)
        {
            Reserve(2);
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(int value)
        {
            Reserve(4);
            bytes[Count++] = (byte)((value >> 24) & 0xff);
            bytes[Count++] = (byte)((value >> 16) & 0xff);
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(uint value)
        {
            Reserve(4);
            bytes[Count++] = (byte)((value >> 24) & 0xff);
            bytes[Count++] = (byte)((value >> 16) & 0xff);
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(long value)
        {
            Reserve(8);
            bytes[Count++] = (byte)((value >> 56) & 0xff);
            bytes[Count++] = (byte)((value >> 48) & 0xff);
            bytes[Count++] = (byte)((value >> 40) & 0xff);
            bytes[Count++] = (byte)((value >> 32) & 0xff);
            bytes[Count++] = (byte)((value >> 24) & 0xff);
            bytes[Count++] = (byte)((value >> 16) & 0xff);
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(ulong value)
        {
            Reserve(8);
            bytes[Count++] = (byte)((value >> 56) & 0xff);
            bytes[Count++] = (byte)((value >> 48) & 0xff);
            bytes[Count++] = (byte)((value >> 40) & 0xff);
            bytes[Count++] = (byte)((value >> 32) & 0xff);
            bytes[Count++] = (byte)((value >> 24) & 0xff);
            bytes[Count++] = (byte)((value >> 16) & 0xff);
            bytes[Count++] = (byte)((value >> 8) & 0xff);
            bytes[Count++] = (byte)((value) & 0xff);
        }
        public void Write(byte[] bytes, int count)
        {
            Reserve(bytes.Length);
            Buffer.BlockCopy(bytes, this.bytes, count);
        }
        public void Write(byte[] bytes)
        {
            Write(bytes, bytes.Length);
        }
        public void Write(ref string value)
        {
            //Make sure the string byte array can hold the string value
            int byteCount = Encoding.UTF8.GetByteCount(value);
            if (stringBytes == null)
                stringBytes = new byte[byteCount];
            else if (byteCount > stringBytes.Length)
                Array.Resize(ref stringBytes, byteCount);

            Reserve(byteCount);
            Write(stringBytes, byteCount);
        }
        public void Write(float value)
        {
            
        }
    }
}
