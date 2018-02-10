using System;
using Rise.Serialization;
namespace Rise.DataTree
{
    //TODO: allow this to be saved to and loaded from files
    public class SerializedData
    {
        //This is the DataNode type that was serialized
        public Type Type { get; private set; }

        //This is the "path" to the data, so we can find it again when we want to undo/redo
        //For example: "Root.Project.Name"
        public string Path { get; private set; }

        //The object serialized into a sequence of bytes
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

        //From: https://stackoverflow.com/a/8808245
        /*static unsafe bool UnsafeCompare(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
                return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            fixed (byte* p1=a1, p2=a2)
            {
                byte* x1 = p1;
                byte* x2 = p2;
                int len = a1.Length;
                for (int i = 0; i < len / 8; i++, x1 += 8, x2 += 8)
                    if (*((long*)x1) != *((long*)x2))
                        return false;
                if ((len & 4) != 0)
                {
                    if (*((int*)x1) != *((int*)x2))
                        return false;
                    x1 += 4;
                    x2 += 4;
                }
                if ((len & 2) != 0)
                {
                    if (*((short*)x1)!=*((short*)x2))
                        return false;
                    x1 += 2;
                    x2 += 2;
                }
                if ((len & 1) != 0 && *x1 != *x2)
                    return false;
                return true;
            }
        }*/
    }
}
