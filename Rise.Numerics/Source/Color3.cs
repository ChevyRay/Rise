using System;
using System.Runtime.InteropServices;
namespace Rise
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Color3 : IEquatable<Color3>
    {
        public static readonly Color3 Transparent = 0x000000;
        public static readonly Color3 Black = 0x000000;
        public static readonly Color3 White = 0xFFFFFF;
        public static readonly Color3 Red = 0xFF0000;
        public static readonly Color3 Green = 0x00FF00;
        public static readonly Color3 Blue = 0x0000FF;
        public static readonly Color3 Yellow = 0xFFFF00;
        public static readonly Color3 Cyan = 0x00FFFF;
        public static readonly Color3 Fuchsia = 0xFF00FF;
        public static readonly Color3 Grey = 0x808080;

        public byte R;
        public byte G;
        public byte B;

        public Color3(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public Color3(UInt32 val)
        {
            R = (byte)((val >> 24) & 0xff);
            G = (byte)((val >> 16) & 0xff);
            B = (byte)((val >> 8) & 0xff);
        }

        public override bool Equals(object obj)
        {
            return obj is Color3 && Equals((Color3)obj);
        }
        public bool Equals(Color3 other)
        {
            return R == other.R && G == other.G && B == other.B;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + R;
                hash = hash * 23 + G;
                hash = hash * 23 + B;
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("{0:x8}", (UInt32)this);
        }

        public static Color3 Lerp(Color3 a, Color3 b, float p)
        {
            return new Color3((byte)(a.R + (b.R - a.R) * p), (byte)(a.G + (b.G - a.G) * p), (byte)(a.B + (b.B - a.B) * p));
        }

        public static Color3 Parse(string str)
        {
            return new Color3(Convert.ToUInt32(str, 16));
        }

        public static implicit operator UInt32(Color3 val)
        {
            return ((UInt32)val.R << 16) | ((UInt32)val.G << 8) | val.B;
        }
        public static implicit operator Color3(UInt32 val)
        {
            return new Color3(
                (byte)((val >> 16) & 0xff),
                (byte)((val >> 8) & 0xff),
                (byte)(val & 0xff)
            );
        }

        public static Color3 Floats(float r, float g, float b)
        {
            return new Color3((byte)(r * 255f), (byte)(g * 255f), (byte)(b * 255f));
        }

        public static Color3 FromHue(float h)
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

        public static bool operator ==(Color3 a, Color3 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Color3 a, Color3 b)
        {
            return !a.Equals(b);
        }

        public static Color3 operator *(Color3 a, Color3 b)
        {
            a.R *= b.R;
            a.G *= b.G;
            a.B *= b.B;
            return a;
        }

        public static Color3 operator *(Color3 c, float n)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            return c;
        }

        public static Color3 operator *(float n, Color3 c)
        {
            c.R = (byte)(c.R * n);
            c.G = (byte)(c.G * n);
            c.B = (byte)(c.B * n);
            return c;
        }

        public static Color3 operator /(Color3 c, float n)
        {
            c.R = (byte)(c.R / n);
            c.G = (byte)(c.G / n);
            c.B = (byte)(c.B / n);
            return c;
        }

        public static implicit operator Color4(Color3 col)
        {
            return new Color4(col.R, col.G, col.B, 255);
        }
    }
}
