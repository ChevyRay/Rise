using System;
using System.Text;
namespace Rise
{
    public static class Hash
    {
        public static ulong Djb2(byte[] bytes)
        {
            ulong hash = 5381;
            for (int i = 0; i < bytes.Length; ++i)
                hash = ((hash << 5) + hash) + bytes[i];
            return hash;
        }

        public static ulong Djb2(string str)
        {
            ulong hash = 5381;
            for (int i = 0; i < str.Length; ++i)
                hash = ((hash << 5) + hash) + str[i];
            return hash;
        }
    }
}
