using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Transparent = 0x00000000;
        public static readonly Color Black = 0x000000FF;
        public static readonly Color White = 0xFFFFFFFF;
        public static readonly Color Red = 0xFF0000FF;
        public static readonly Color Green = 0x00FF00FF;
        public static readonly Color Blue = 0x0000FFFF;
        public static readonly Color Yellow = 0xFFFF00FF;
        public static readonly Color Cyan = 0x00FFFFFF;
        public static readonly Color Fuchsia = 0xFF00FFFF;
        public static readonly Color Grey = 0x808080FF;

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public Color(UInt32 val)
        {
            R = (byte)((val >> 24) & 0xff);
            G = (byte)((val >> 16) & 0xff);
            B = (byte)((val >> 8) & 0xff);
            A = (byte)(val & 0xff);
        }

        public override bool Equals(object obj)
        {
            return obj is Color && Equals((Color)obj);
        }
        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + R;
                hash = hash * 23 + G;
                hash = hash * 23 + B;
                hash = hash * 23 + A;
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:x8}", (UInt32)this);
        }

        public static Color Lerp(Color a, Color b, float p)
        {
            return new Color((byte)(a.R + (b.R - a.R) * p), (byte)(a.G + (b.G - a.G) * p), (byte)(a.B + (b.B - a.B) * p), (byte)(a.A + (b.A - a.A) * p));
        }

        public static Color Parse(string str)
        {
            UInt32 val = Convert.ToUInt32(str, 16);
            if (str.Length > 6)
                return new Color(val);
            return new Color((val << 8) | 0xff);
        }

        public static implicit operator UInt32(Color val)
        {
            return ((UInt32)val.R << 24) | ((UInt32)val.G << 16) | ((UInt32)val.B << 8) | val.A;
        }
        public static implicit operator Color(UInt32 val)
        {
            return new Color(
                (byte)((val >> 24) & 0xff),
                (byte)((val >> 16) & 0xff),
                (byte)((val >> 8) & 0xff),
                (byte)(val & 0xff)
            );
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Color a, Color b)
        {
            return !a.Equals(b);
        }

        public static Color operator *(Color a, Color b)
        {
            a.R *= b.R;
            a.G *= b.G;
            a.B *= b.B;
            a.A *= b.A;
            return a;
        }

        public static Color operator *(Color c, float n)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            c.A = (byte)(c.A * n);
            return c;
        }

        public static Color operator *(float n, Color c)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            c.A = (byte)(c.A * n);
            return c;
        }

        public static Color operator /(Color c, float n)
        {
            c.R = (byte)(c.R / n);
            c.G = (byte)(c.G / n);
            c.B = (byte)(c.B / n);
            c.A = (byte)(c.A / n);
            return c;
        }
    }
}
