using System;
namespace Rise.Serialization
{
    //TODO: allow this to be saved to and loaded from files
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

        //If you serialize before & after some code, you can use this to check if it changed
        public bool IsEqualTo(SerializedData data)
        {
            if (Type != data.Type || Path != data.Path || Bytes.Length != data.Bytes.Length)
                return false;
            for (int i = 0; i < Bytes.Length; ++i)
                if (Bytes[i] != data.Bytes[i])
                    return false;
            return true;
        }
    }
}
