using System;
namespace Rise
{
    public static class Rand
    {
        static int seed;
        static Random rand;
        static object randLock = new object();
        static byte[] bytes = new byte[4];

        static Rand()
        {
            seed = DateTime.Now.Millisecond;
            rand = new Random(seed);
        }

        public static int Seed
        {
            get { return seed; }
        }

        public static void SetSeed(int newSeed)
        {
            lock (randLock)
            {
                seed = newSeed;
                rand = new Random(seed);
            }
        }

        public static byte NextByte(this Random rand)
        {
            return (byte)(rand.Next() % 256);
        }

        public static byte NextByte(this Random rand, byte max)
        {
            return (byte)(rand.Next(max) % 256);
        }

        public static byte NextByte(this Random rand, byte min, byte max)
        {
            return (byte)(rand.Next(min, max) % 256);
        }

        public static byte Byte()
        {
            lock (randLock)
                return rand.NextByte();
        }

        public static byte Byte(byte max)
        {
            lock (randLock)
                return rand.NextByte(max);
        }

        public static byte Byte(byte min, byte max)
        {
            lock (randLock)
                return rand.NextByte(min, max);
        }

        public static int Int()
        {
            lock (randLock)
                return rand.Next();
        }

        public static int Int(int max)
        {
            lock (randLock)
                return rand.Next(max);
        }

        public static int Int(int min, int max)
        {
            lock (randLock)
                return rand.Next(min, max);
        }

        public static double Double()
        {
            lock (randLock)
                return rand.NextDouble();
        }

        public static double Double(double max)
        {
            lock (randLock)
                return rand.NextDouble() * max;
        }

        public static double Double(double min, double max)
        {
            lock (randLock)
                return min + rand.NextDouble() * (max - min);
        }

        public static float NextFloat(this Random rand)
        {
            return (float)rand.NextDouble();
        }

        public static float NextFloat(this Random rand, float max)
        {
            return (float)rand.NextDouble() * max;
        }

        public static float NextFloat(this Random rand, float min, float max)
        {
            return min + (float)rand.NextDouble() * (max - min);
        }

        public static float Float()
        {
            lock (randLock)
                return rand.NextFloat();
        }

        public static float Float(float max)
        {
            lock (randLock)
                return rand.NextFloat(max);
        }

        public static float Float(float min, float max)
        {
            lock (randLock)
                return rand.NextFloat(min, max);
        }

        public static float NextAngle(this Random rand)
        {
            return (float)(rand.NextDouble() * Math.PI * 2.0);
        }

        public static float Angle()
        {
            lock (randLock)
                return rand.NextAngle();
        }

        public static Vector2 NextDirection(this Random rand)
        {
            double r = rand.NextDouble() * Math.PI * 2.0;
            return new Vector2((float)Math.Cos(r), (float)Math.Sin(r));
        }

        public static Vector2 Direction()
        {
            lock (randLock)
                return rand.NextDirection();
        }

        public static Color4 NextColor(this Random rand)
        {
            rand.NextBytes(bytes);
            return new Color4(bytes[0], bytes[1], bytes[2], bytes[3]);
        }

        public static Color4 Color()
        {
            lock (randLock)
                return rand.NextColor();
        }

        public static void Bytes(byte[] buffer)
        {
            lock (randLock)
                rand.NextBytes(buffer);
        }

        public static bool NextBool(this Random rand)
        {
            return rand.Next(2) == 1;
        }

        public static bool Bool()
        {
            lock (randLock)
                return rand.NextBool();
        }
    }
}

