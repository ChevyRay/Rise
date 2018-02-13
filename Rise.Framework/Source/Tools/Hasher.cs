using System;
namespace Rise
{
    public static class Hasher
    {
        public const uint Start = 0x811C9DC5;

        public static uint Add(uint hash, byte value)
        {
            return (hash * 0x1000193) ^ value;
        }
    }
}
