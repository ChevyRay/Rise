using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
namespace Rise
{
    public static class FontCharSet
    {
        public static readonly string Ascii;
        public static readonly string BasicLatin;
        public static readonly string Latin1Supplement;
        public static readonly string LatinExtendedA;
        public static readonly string LatinExtendedB;

        static FontCharSet()
        {
            var builder = new StringBuilder();
            for (char chr = (char)0x20; chr <= 0x7E; ++chr)
                builder.Append(chr);
            Ascii = builder.ToString();

            builder.Clear();
            for (char chr = (char)32; chr <= 126; ++chr)
                builder.Append(chr);
            BasicLatin = builder.ToString();

            builder.Clear();
            for (char chr = (char)160; chr <= 255; ++chr)
                builder.Append(chr);
            Latin1Supplement = builder.ToString();

            builder.Clear();
            for (char chr = (char)256; chr <= 383; ++chr)
                builder.Append(chr);
            LatinExtendedA = builder.ToString();

            builder.Clear();
            for (char chr = (char)256; chr <= 591; ++chr)
                builder.Append(chr);
            LatinExtendedB = builder.ToString();
        }
    }

    struct FontGlyph
    {
        public int Index;
        public int Advance;
        public int OffsetX;
    }

    public class Font
    {
        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern IntPtr init_font(byte* data);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void free_font(IntPtr info);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int num_glyphs(IntPtr info);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern float scale_for_pixel_height(IntPtr info, float height);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void get_font_vmetrics(IntPtr info, out int ascent, out int descent, out int line_gap);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int get_glyph_index(IntPtr info, int codepoint);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool is_glyph_empty(IntPtr info, int glyph);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void get_font_bbox(IntPtr info, out int x0, out int y0, out int x1, out int y1);
       
        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void get_glyph_bitmap_box(IntPtr info, int glyph, float scale_x, float scale_y, out int x0, out int y0, out int x1, out int y1);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void get_glyph_box(IntPtr info, int glyph, out int x0, out int y0, out int x1, out int y1);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void get_glyph_bitmap(IntPtr info, byte* output, int w, int h, int stride, float scale_x, float scale_y, int glyph);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void get_glyph_hmetrics(IntPtr info, int glyph, out int advance, out int left);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int get_kerning(IntPtr info, int glyph1, int glyph2);

        internal IntPtr info;
        internal char[] chars;
        internal FontGlyph[] glyphs;

        public string Characters { get; private set; }
        public int Ascent { get; private set; }
        public int Descent { get; private set; }
        public int LineGap { get; private set; }
        public int Height { get { return Ascent - Descent; } }
        public int CharacterCount { get { return chars.Length; } }

        public Font(string file) : this(file, null)
        {
            
        }
        public unsafe Font(string file, string characters)
        {
            var data = File.ReadAllBytes(file);
            fixed (byte* ptr = data)
                info = init_font(ptr);

            //Get vertical metrics
            int a, d, l;
            get_font_vmetrics(info, out a, out d, out l);
            Ascent = a;
            Descent = d;
            LineGap = l;

            if (characters == null)
            {
                var builder = new StringBuilder();
                for (char chr = (char)0; chr < char.MaxValue; ++chr)
                    if (get_glyph_index(info, chr) > 0)
                        builder.Append(chr);
                characters = builder.ToString();
                Characters = characters;
            }

            //Put all the characters into a sorted array to allow for binary searching
            chars = characters.ToCharArray();
            Array.Sort(chars);

            //Get the glyph metrics for each character
            glyphs = new FontGlyph[chars.Length];
            for (int i = 0; i < glyphs.Length; ++i)
            {
                glyphs[i].Index = get_glyph_index(info, chars[i]);
                if (glyphs[i].Index > 0)
                    get_glyph_hmetrics(info, glyphs[i].Index, out glyphs[i].Advance, out glyphs[i].OffsetX);
            }
        }
        ~Font()
        {
            free_font(info);
        }

        internal float GetScale(float size)
        {
            return scale_for_pixel_height(info, size);
        }

        internal void GetBitmapBox(int i, float scale, out int x0, out int y0, out int x1, out int y1)
        {
            get_glyph_bitmap_box(info, glyphs[i].Index, scale, scale, out x0, out y0, out x1, out y1);
        }

        internal int GetIndex(char chr)
        {
            int i = Array.BinarySearch(chars, chr);
            if (i < 0)
                throw new Exception(string.Format("Font does not have character: {0}, U+{1:X16}", chr, (UInt16)chr));
            return i;
        }

        internal int GetKerning(char chr1, char chr2)
        {
            int i = GetIndex(chr1);
            int j = GetIndex(chr2);
            return get_kerning(info, glyphs[i].Index, glyphs[j].Index);
        }
        internal int GetKerning(int i, int j)
        {
            return get_kerning(info, glyphs[i].Index, glyphs[j].Index);
        }

        internal unsafe void GetPixels(int i, byte[] pixels, int w, int h, float scale)
        {
            fixed (byte* ptr = pixels)
            get_glyph_bitmap(info, ptr, w, h, w, scale, scale, glyphs[i].Index);
        }

        public bool IsEmpty(char chr)
        {
            int index = GetIndex(chr);
            return glyphs[index].Index == 0 || is_glyph_empty(info, glyphs[index].Index);
        }
    }

    public struct FontChar
    {
        public char Char;
        public int Advance;
        public int OffsetX;
        public int OffsetY;
        public int Width;
        public int Height;
    }

    public class FontSize
    {
        public Font Font { get; private set; }
        public float Size { get; private set; }
        public int MaxCharW { get; private set; }
        public int MaxCharH { get; private set; }
        public int CharCount { get { return chars.Length; } }
        public int Ascent { get; private set; }
        public int Descent { get; private set; }
        public int LineGap { get; private set; }
        public int Height { get { return Ascent - Descent; } }

        float scale;
        char[] codes;
        FontChar[] chars;
        byte[] buffer;

        public FontSize(Font font, float size)
        {
            Font = font;
            Size = size;
            scale = font.GetScale(size);

            Ascent = (int)(font.Ascent * scale);
            Descent = (int)(font.Descent * scale);
            LineGap = (int)(font.LineGap * scale);
                
            int x0, y0, x1, y1;

            codes = font.chars;
            chars = new FontChar[codes.Length];
            for (int i = 0; i < chars.Length; ++i)
            {
                chars[i].Char = Font.chars[i];
                chars[i].Advance = (int)(Font.glyphs[i].Advance * scale);
                chars[i].OffsetX = (int)(Font.glyphs[i].OffsetX * scale);

                //If the glyph is empty it has no size
                if (!font.IsEmpty(chars[i].Char))
                {
                    font.GetBitmapBox(i, scale, out x0, out y0, out x1, out y1);
                    chars[i].OffsetY = y0;
                    chars[i].Width = x1 - x0;
                    chars[i].Height = y1 - y0;

                    MaxCharW = Math.Max(chars[i].Width, MaxCharW);
                    MaxCharH = Math.Max(chars[i].Height, MaxCharH);
                }
            }

            buffer = new byte[MaxCharW * MaxCharH];
        }

        public bool IsEmpty(char chr)
        {
            return Font.IsEmpty(chr);
        }

        public void GetCharInfoAt(int i, out FontChar info)
        {
            info = chars[i];
        }

        public char GetCharAt(int i)
        {
            return chars[i].Char;
        }

        public void GetCharInfo(char chr, out FontChar info)
        {
            info = chars[Font.GetIndex(chr)];
        }
        public FontChar GetCharInfo(char chr)
        {
            return chars[Font.GetIndex(chr)];
        }

        public int GetKerning(char chr1, char chr2)
        {
            return (int)(Font.GetKerning(chr1, chr2) * scale);
        }
        int GetKerning(int i, int j)
        {
            return (int)(Font.GetKerning(i, j) * scale);
        }

        public void GetPixels(char chr, Bitmap bitmap, bool premultiply)
        {
            int i = Font.GetIndex(chr);

            int w = chars[i].Width;
            int h = chars[i].Height;
            bitmap.Resize(w, h);

            if (bitmap.Width < w)
                throw new Exception("Bitmap is not wide enough.");
            if (bitmap.Height < h)
                throw new Exception("Bitmap is not tall enough.");

            Font.GetPixels(i, buffer, w, h, scale);

            var pixels = bitmap.Pixels;
            int p = 0;
            if (premultiply)
            {
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        pixels[p].R = buffer[p];
                        pixels[p].G = buffer[p];
                        pixels[p].B = buffer[p];
                        pixels[p].A = buffer[p];
                        ++p;
                    }
                }
            }
            else
            {
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        pixels[p].R = 255;
                        pixels[p].G = 255;
                        pixels[p].B = 255;
                        pixels[p].A = buffer[p];
                        ++p;
                    }
                }
            }
        }
    }
}
