#include "extern_decl.h"

#define STB_TRUETYPE_IMPLEMENTATION
#include "stb_truetype.h"

extern "C"
{
    EXTERN_DECL stbtt_fontinfo* init_font(const uint8_t* data)
    {
        stbtt_fontinfo* info = new stbtt_fontinfo();
        if (!stbtt_InitFont(info, data, 0))
        {
            delete info;
            return nullptr;
        }
        return info;
    }
    
    EXTERN_DECL void free_font(stbtt_fontinfo* info)
    {
        delete info;
    }
    
    EXTERN_DECL float scale_for_pixel_height(stbtt_fontinfo* info, float height)
    {
        return stbtt_ScaleForPixelHeight(info, height);
    }
    
    EXTERN_DECL void get_font_vmetrics(stbtt_fontinfo* info, int* ascent, int* descent, int* line_gap)
    {
        stbtt_GetFontVMetrics(info, ascent, descent, line_gap);
    }
    
    EXTERN_DECL int get_glyph_index(stbtt_fontinfo* info, int codepoint)
    {
        return stbtt_FindGlyphIndex(info, codepoint);
    }
    
    EXTERN_DECL bool is_glyph_empty(stbtt_fontinfo* info, int glyph)
    {
        return stbtt_IsGlyphEmpty(info, glyph) != 0;
    }
    
    EXTERN_DECL void get_font_bbox(stbtt_fontinfo* info, int* x0, int* y0, int* x1, int* y1)
    {
        stbtt_GetFontBoundingBox(info, x0, y0, x1, y1);
    }
    
    EXTERN_DECL void get_glyph_bitmap_box(stbtt_fontinfo* info, int glyph, float scale_x, float scale_y, int* x0, int* y0, int* x1, int* y1)
    {
        stbtt_GetGlyphBitmapBox(info, glyph, scale_x, scale_y, x0, y0, x1, y1);
    }
    
    EXTERN_DECL void get_glyph_box(stbtt_fontinfo* info, int glyph, int* x0, int* y0, int* x1, int* y1)
    {
        stbtt_GetGlyphBox(info, glyph, x0, y0, x1, y1);
    }
    
    EXTERN_DECL void get_glyph_bitmap(stbtt_fontinfo* info, uint8_t* output, int w, int h, int stride, float scale_x, float scale_y, int glyph)
    {
        stbtt_MakeGlyphBitmap(info, output, w, h, stride, scale_x, scale_y, glyph);
    }
    
    EXTERN_DECL void get_glyph_hmetrics(stbtt_fontinfo* info, int glyph, int* advance, int* left)
    {
        stbtt_GetGlyphHMetrics(info, glyph, advance, left);
    }
    
    EXTERN_DECL int get_kerning(stbtt_fontinfo* info, int glyph1, int glyph2)
    {
        return stbtt_GetGlyphKernAdvance(info, glyph1, glyph2);
    }
}
