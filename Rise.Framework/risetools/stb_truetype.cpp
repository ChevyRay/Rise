#include "extern_decl.h"

#define STB_TRUETYPE_IMPLEMENTATION
#include "stb_truetype.h"

extern "C"
{
    EXTERN_DECL int font_info_size()
    {
        return (int)sizeof(stbtt_fontinfo);
    }
    
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
    
    EXTERN_DECL uint8_t* get_char_bitmap(stbtt_fontinfo* info, float scale_x, float scale_y, int codepoint, int* w, int *h, int* xoff, int* yoff)
    {
        return stbtt_GetCodepointBitmap(info, scale_x, scale_y, codepoint, w, h, xoff, yoff);
    }
    
    EXTERN_DECL void get_char_hmetrics(stbtt_fontinfo* info, int codepoint, int* advance, int* left)
    {
        stbtt_GetCodepointHMetrics(info, codepoint, advance, left);
    }
    
    EXTERN_DECL int get_kerning(stbtt_fontinfo* info, int ch1, int ch2)
    {
        return stbtt_GetCodepointKernAdvance(info, ch1, ch2);
    }
}
