#define STB_IMAGE_IMPLEMENTATION
#define STBI_NO_STDIO
#include "stb_image.h"

#include "extern_decl.h"

extern "C"
{
    EXTERN_DECL uint8_t* load_image(uint8_t* data, int length, int* w, int* h)
    {
        int comp;
        return stbi_load_from_memory(data, length, w, h, &comp, STBI_rgb_alpha);
    }
    
    EXTERN_DECL void free_image(uint8_t* image)
    {
        stbi_image_free(image);
    }
}
