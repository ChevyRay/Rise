#define STB_IMAGE_WRITE_IMPLEMENTATION
#define STBI_WRITE_NO_STDIO
#include "stb_image_write.h"

#include "extern_decl.h"

extern "C"
{
    //void stbi_write_func(void *context, void *data, int size);
    
    EXTERN_DECL void convert_to_png(uint8_t* data, int w, int h, stbi_write_func* func)
    {
        stbi_write_png_to_func(func, nullptr, w, h, 4, data, w * 4);
    }
    
    EXTERN_DECL void convert_to_bmp(uint8_t* data, int w, int h, stbi_write_func* func)
    {
        stbi_write_bmp_to_func(func, nullptr, w, h, 4, data);
    }
    
    EXTERN_DECL void convert_to_tga(uint8_t* data, int w, int h, stbi_write_func* func)
    {
        stbi_write_tga_to_func(func, nullptr, w, h, 4, data);
    }
    
    //quality = 1-100
    EXTERN_DECL void convert_to_jpg(uint8_t* data, int w, int h, int quality, stbi_write_func* func)
    {
        stbi_write_jpg_to_func(func, nullptr, w, h, 4, data, quality);
    }
}
