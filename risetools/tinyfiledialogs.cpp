#include "tinyfiledialogs.h"
#include "extern_decl.h"
#include <vector>
#include <string>
#include <sstream>

using namespace std;

extern "C"
{
    static const size_t MAX_FILTERS = 16;
    static string strings[MAX_FILTERS];
    static const char* filters[MAX_FILTERS];
    static int filter_count;
    
    void parse_filter(const char* filter)
    {
        filter_count = 0;
        stringstream ss(filter);
        while (ss.good() && filter_count < MAX_FILTERS)
        {
            strings[filter_count] = "";
            getline(ss, strings[filter_count], ',');
            filters[filter_count] = strings[filter_count].data();
            ++filter_count;
        }
    }
    
    EXTERN_DECL int message_box(const char* title, const char* msg, int type, int icon_type, int default_button)
    {
        const char* types[4] = { "ok", "okcancel", "yesno", "yesnocancel" };
        const char* icon_types[4] = { "info", "warning", "error", "question" };
        int result = tinyfd_messageBox(title, msg, types[type], icon_types[icon_type], default_button);
        return result;
    }
    
    EXTERN_DECL const char* input_box(const char* title, const char* msg, const char* default_input)
    {
        return tinyfd_inputBox(title, msg, default_input);
    }
    
    EXTERN_DECL const char* save_file_dialog(const char* title, const char* default_path, const char* filter)
    {
        parse_filter(filter);
        return tinyfd_saveFileDialog(title, default_path, filter_count, filters, "");
    }
    
    EXTERN_DECL const char* open_file_dialog(const char* title, const char* default_path, const char* filter)
    {
        parse_filter(filter);
        return tinyfd_openFileDialog(title, default_path, filter_count, filters, nullptr, 0);
    }
    
    EXTERN_DECL const char* open_files_dialog(const char* title, const char* default_path, const char* filter)
    {
        parse_filter(filter);
        return tinyfd_openFileDialog(title, default_path, filter_count, filters, nullptr, 1);
    }
    
    EXTERN_DECL const char* select_folder_dialog(const char* title, const char* default_path)
    {
        return tinyfd_selectFolderDialog(title, default_path);
    }
    
    EXTERN_DECL uint32_t color_chooser(const char* title, uint32_t color)
    {
        uint8_t input[3];
        input[0] = (color>>24)&0xff;
        input[1] = (color>>16)&0xff;
        input[2] = (color>>8)&0xff;
        uint8_t output[3];
        if (tinyfd_colorChooser(title, nullptr, input, output) == nullptr)
            return color;
        return ((uint32_t)output[0] << 24) | ((uint32_t)output[1]<<16) | ((uint32_t)output[2]<<8) | 0xff;
    }
}
