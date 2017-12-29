using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color4 : IEquatable<Color4>
    {
        public static readonly Color4 Transparent = 0x00000000;
        public static readonly Color4 Black = 0x000000FF;
        public static readonly Color4 White = 0xFFFFFFFF;
        public static readonly Color4 Red = 0xFF0000FF;
        public static readonly Color4 Green = 0x00FF00FF;
        public static readonly Color4 Blue = 0x0000FFFF;
        public static readonly Color4 Yellow = 0xFFFF00FF;
        public static readonly Color4 Cyan = 0x00FFFFFF;
        public static readonly Color4 Fuchsia = 0xFF00FFFF;
        public static readonly Color4 Grey = 0x808080FF;

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color4(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public Color4(UInt32 val)
        {
            R = (byte)((val >> 24) & 0xff);
            G = (byte)((val >> 16) & 0xff);
            B = (byte)((val >> 8) & 0xff);
            A = (byte)(val & 0xff);
        }

        public override bool Equals(object obj)
        {
            return obj is Color4 && Equals((Color4)obj);
        }
        public bool Equals(Color4 other)
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

        public Color3 RGB
        {
            get { return new Color3(R, G, B); }
        }

        public static Color4 Lerp(Color4 a, Color4 b, float p)
        {
            return new Color4((byte)(a.R + (b.R - a.R) * p), (byte)(a.G + (b.G - a.G) * p), (byte)(a.B + (b.B - a.B) * p), (byte)(a.A + (b.A - a.A) * p));
        }

        public static Color4 Parse(string str)
        {
            UInt32 val = Convert.ToUInt32(str, 16);
            if (str.Length > 6)
                return new Color4(val);
            return new Color4((val << 8) | 0xff);
        }

        public static implicit operator UInt32(Color4 val)
        {
            return ((UInt32)val.R << 24) | ((UInt32)val.G << 16) | ((UInt32)val.B << 8) | val.A;
        }
        public static implicit operator Color4(UInt32 val)
        {
            return new Color4(
                (byte)((val >> 24) & 0xff),
                (byte)((val >> 16) & 0xff),
                (byte)((val >> 8) & 0xff),
                (byte)(val & 0xff)
            );
        }

        public static Color4 Floats(float r, float g, float b, float a)
        {
            return new Color4((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f), (byte)(a * 255f));
        }
        public static Color4 Floats(float r, float g, float b)
        {
            return new Color4((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f), 255);
        }

        public static Color4 FromHue(float h)
        {
            h = (h % 1f) * 360f;
            if (h < 0f)
                h += 360f;
            if (h < 60f)
                return Floats(1f, h / 60f, 0f);
            else if (h < 120f)
                return Floats(1f - (h - 60f) / 60f, 1f, 0f);
            else if (h < 180f)
                return Floats(0f, 1f, (h - 120f) / 60f);
            else if (h < 240f)
                return Floats(0f, 1f - (h - 180f) / 60f, 1f);
            else if (h < 300f)
                return Floats((h - 240f) / 60f, 0f, 1f);
            else
                return Floats(1f, 0f, 1f - (h - 300f) / 60f);
        }

        public static bool operator ==(Color4 a, Color4 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Color4 a, Color4 b)
        {
            return !a.Equals(b);
        }

        public static Color4 operator *(Color4 a, Color4 b)
        {
            a.R *= b.R;
            a.G *= b.G;
            a.B *= b.B;
            a.A *= b.A;
            return a;
        }

        public static Color4 operator *(Color4 c, float n)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            c.A = (byte)(c.A * n);
            return c;
        }

        public static Color4 operator *(float n, Color4 c)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            c.A = (byte)(c.A * n);
            return c;
        }

        public static Color4 operator /(Color4 c, float n)
        {
            c.R = (byte)(c.R / n);
            c.G = (byte)(c.G / n);
            c.B = (byte)(c.B / n);
            c.A = (byte)(c.A / n);
            return c;
        }
    }
}
