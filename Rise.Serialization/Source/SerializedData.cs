using System;
namespace Rise.Serialization
{
    public class SerializedData
    {
        public Type Type { get; private set; }
        public string Path { get; private set; }
        public byte[] Bytes { get; private set; }

        internal SerializedData(Type type, string path, byte[] bytes)
        {
            Type = type;
            Path = path;
            Bytes = bytes;
        }
    }
}
