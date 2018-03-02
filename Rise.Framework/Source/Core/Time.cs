using System;
using System.Collections.Generic;
namespace Rise
{
    public static class Time
    {
        public static float Delta { get; private set; }
        public static float Total { get; private set; }
        public static ulong Frames { get; private set; }
        public static float Rate = 1f;

        //target size
        //viewport
        //ortho matrix

        static Stack<ulong> stack = new Stack<ulong>();

        public static double Seconds
        {
            get { return App.platform.GetTime(); }
        }

        public static ulong ClockFrequency
        {
            get { return App.platform.GetClockFrequency(); }
        }

        public static ulong ClockValue
        {
            get { return App.platform.GetClockValue(); }
        }

        public static ulong ClockMilliseconds
        {
            get { return ClockValue / (ClockFrequency / 1000); }
        }

        internal static void Init(float dt)
        {
            Delta = dt;
        }

        internal static void PreUpdate(float dt)
        {
            Delta = dt * Rate;
            Total += Delta;
            ++Frames;
        }

        //Sin wave back and forth between two numbers
        public static float Wave(float from, float to, float duration, float offsetPercent)
        {
            var range = (to - from) * 0.5f;
            return from + range + Calc.Sin(((Total + duration * offsetPercent) / duration) * Calc.Tau) * range;
        }
        public static float Wave(float from, float to, float duration)
        {
            return Wave(from, to, duration, 0f);
        }

        //Flicker a boolean on and off at intervals
        public static bool Flicker(float onTime, float offTime)
        {
            return (Total % (onTime + offTime)) < onTime;
        }
        public static bool Flicker(float time)
        {
            return Flicker(time, time);
        }

        public static double ClockToSeconds(ulong clock)
        {
            return clock / (double)App.platform.GetClockFrequency();
        }

        public static ulong SecondsToClock(double seconds)
        {
            return (ulong)(App.platform.GetClockFrequency() * seconds);
        }

        /* 
         * Useful for tracking execution time. Eg:
         * 
         * Time.Push();
         * DoBunchOfStuff();
         * Time.Pop("Execution time");      "execution time: 123 ms"
         * 
         */
        public static void Push()
        {
            stack.Push(ClockValue);
        }
        public static double Pop(string message)
        {
            double ms = ClockToSeconds(ClockValue - stack.Pop()) * 1000.0;
            Console.WriteLine("{0}: {1} ms", message, ms);
            return ms;
        }
    }
}
