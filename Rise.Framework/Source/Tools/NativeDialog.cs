using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
namespace Rise
{
    public enum MessageBoxType
    {
        Ok = 0,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxIconType
    {
        Info = 0,
        Warning,
        Error,
        Question
    }

    public enum MessageBoxButton
    {
        CancelNo = 0,
        OkYes,
        No //no in yesnocancel
    }

    public static class NativeDialog
    {
        static unsafe string GetString(byte* bytes)
        {
            if (bytes == null)
                return null;
            int count = 0;
            while (bytes[count] != 0)
                ++count;
            return Encoding.UTF8.GetString(bytes, count);
        }

        //int message_box(const char* title, const char* msg, int type, int icon_type, int default_button)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "message_box")]
        public static extern MessageBoxButton MessageBox(string title, string message, MessageBoxType type, MessageBoxIconType iconType, MessageBoxButton defaultButton);

        //const char* input_box(const char* title, const char* msg, const char* default_input)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "input_box")]
        static unsafe extern byte* input_box(string title, string message, string defaultInput);
        public static unsafe string InputBox(string title, string message, string defaultInput)
        {
            return GetString(input_box(title, message, defaultInput));
        }

        //const char* save_file_dialog(const char* title, const char* default_path, const char* filter)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "save_file_dialog")]
        static unsafe extern byte* save_file_dialog(string title, string defaultPath, string filter);
        public static unsafe string SaveFile(string title, string defaultPath, string filter)
        {
            return GetString(save_file_dialog(title, defaultPath, filter));
        }

        //const char* open_file_dialog(const char* title, const char* default_path, const char* filter)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "open_file_dialog")]
        static unsafe extern byte* open_file_dialog(string title, string defaultPath, string filter);
        public static unsafe string OpenFile(string title, string defaultPath, string filter)
        {
            return GetString(open_file_dialog(title, defaultPath, filter));
        }

        //const char* open_files_dialog(const char* title, const char* default_path, const char* filter)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "open_files_dialog")]
        static unsafe extern byte* open_files_dialog(string title, string defaultPath, string filter);
        public static unsafe string OpenFiles(string title, string defaultPath, string filter)
        {
            return GetString(open_files_dialog(title, defaultPath, filter));
        }

        //const char* select_folder_dialog(const char* title, const char* default_path)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "select_folder_dialog")]
        static unsafe extern byte* select_folder_dialog(string title, string defaultPath);
        public static unsafe string SelectFolder(string title, string defaultPath)
        {
            return GetString(select_folder_dialog(title, defaultPath));
        }

        //uint32_t color_chooser(const char* title, uint32_t color)
        [DllImport("risetools.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "color_chooser")]
        public static extern uint ColorChooser(string title, uint color);
    }
}
