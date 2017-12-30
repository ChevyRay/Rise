using System;
using System.Runtime.InteropServices;
namespace Rise.PlatformSDL.SDL2
{
    public static class SDL
    {
        const string dll = "SDL2.dll";

        public const int ScancodeMask = (1 << 30);
        public const int WindowPosCentered = 0x2FFF0000;

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_ClearError")]
        public static extern void ClearError();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SDL_GetError();
        public static string GetError()
        {
            return Marshal.PtrToStringAnsi(SDL_GetError());
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_Init")]
        public static extern int Init(SubSystem flags);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_Quit")]
        public static extern void Quit();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetVersion")]
        public static extern void GetVersion(out Version version);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetTicks")]
        public static extern UInt32 GetTicks();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetPerformanceFrequency")]
        public static extern UInt64 GetPerformanceFrequency();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetPerformanceCounter")]
        public static extern UInt64 GetPerformanceCounter();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_GetAttribute")]
        public static extern int GLGetAttribute(GLAttr attr, out int value);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_SetAttribute")]
        public static extern int GLSetAttribute(GLAttr attr, int value);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern Window SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, WindowFlags flags);
        public static Window CreateWindow(string title, int x, int y, int w, int h, WindowFlags flags)
        {
            var titlePtr = Marshal.StringToHGlobalAnsi(title);
            var window = SDL_CreateWindow(titlePtr, x, y, w, h, flags);
            Marshal.FreeHGlobal(titlePtr);
            return window;
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_CreateContext")]
        public static extern GLContext GLCreateContext(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_MakeCurrent")]
        public static extern int GLMakeCurrent(Window window, GLContext context);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_SwapWindow")]
        public static extern void GLSwapWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_SetSwapInterval")]
        public static extern int GLSetSwapInterval(int interval);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowID")]
        public static extern UInt32 GetWindowID(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SDL_GetWindowTitle(Window window);
        public static string GetWindowTitle(Window window)
        {
            return Marshal.PtrToStringAnsi(SDL_GetWindowTitle(window));
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern void SDL_SetWindowTitle(Window window, IntPtr title);
        public static void SetWindowTitle(Window window, string title)
        {
            var ptr = Marshal.StringToHGlobalAnsi(title);
            SDL_SetWindowTitle(window, ptr);
            Marshal.FreeHGlobal(ptr);
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowPosition")]
        public static extern void SetWindowPosition(Window window, int x, int y);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowPosition")]
        public static extern void GetWindowPosition(Window window, out int x, out int y);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowSize")]
        public static extern void SetWindowSize(Window window, int w, int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowSize")]
        public static extern void GetWindowSize(Window window, out int w, out int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GL_GetDrawableSize")]
        public static extern void GLGetDrawableSize(Window window, out int w, out int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowMinimumSize")]
        public static extern void SetWindowMinimumSize(Window window, int w, int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowMinimumSize")]
        public static extern void GetWindowMinimumSize(Window window, out int w, out int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowMaximumSize")]
        public static extern void SetWindowMaximumSize(Window window, int w, int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowMaximumSize")]
        public static extern void GetWindowMaximumSize(Window window, out int w, out int h);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowBordersSize")]
        public static extern int GetWindowBorderSize(Window window, out int top, out int left, out int bottom, out int right);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowBordered")]
        public static extern void SetWindowBordered(Window window, bool bordered);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowResizable")]
        public static extern void SetWindowResizable(Window window, bool resizable);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_ShowWindow")]
        public static extern void ShowWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_HideWindow")]
        public static extern void HideWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_RaiseWindow")]
        public static extern void RaiseWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_MaximizeWindow")]
        public static extern void MaximizeWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_MinimizeWindow")]
        public static extern void MinimizeWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_RestoreWindow")]
        public static extern void RestoreWindow(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowFullscreen")]
        public static extern int SetWindowFullscreen(Window window, FullscreenMode mode);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowBrightness")]
        public static extern int SetWindowBrightness(Window window, float brightness);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowBrightness")]
        public static extern float GetWindowBrightness(Window window);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_PollEvent")]
        public static extern int PollEvent(out Event e);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SDL_GL_GetProcAddress(IntPtr proc);
        public static IntPtr GLGetProcAddress(string proc)
        {
            var procPtr = Marshal.StringToHGlobalAnsi(proc);
            var ptr = SDL_GL_GetProcAddress(procPtr);
            Marshal.FreeHGlobal(procPtr);
            return ptr;
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetMouseState")]
        public static extern UInt32 GetMouseState(out int x, out int y);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetGlobalMouseState")]
        public static extern UInt32 GetGlobalMouseState(out int x, out int y);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetScancodeFromKey")]
        public static extern int GetScanCodeFromKey(int key);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetKeyFromScancode")]
        public static extern int GetKeyFromScanCode(int code);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl, EntryPoint =  "SDL_HasClipboardText")]
        public static extern bool HasClipboardText();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr SDL_GetClipboardText();
        public static string GetClipboardText()
        {
            return Marshal.PtrToStringAnsi(SDL_GetClipboardText());
        }

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        static extern int SDL_SetClipboardText(IntPtr text);
        public static int SetClipboardText(string text)
        {
            var ptr = Marshal.StringToHGlobalAnsi(text);
            int result = SDL_SetClipboardText(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
    }

    public enum FullscreenMode : uint
    {
        Off = 0,
        On = 0x00000001,
        Desktop = 0x00001001
    }

    [Flags]
    public enum SubSystem : uint
    {
        Timer = 0x00000001,
        Audio = 0x00000010,
        Video = 0x00000020,
        Joystick = 0x00000200,
        Haptic = 0x00001000,
        GameController = 0x00002000,
        Events = 0x00004000,
        NoParachute = 0x00100000,
        Everything = Timer | Audio | Video | Joystick | Haptic | GameController | Events
    }

    public enum GLAttr
    {
        RedSize,
        GreenSize,
        BlueSize,
        AlphaSize,
        BufferSize,
        DoubleBuffer,
        DepthSize,
        StencilSize,
        AccumRedSize,
        AccumGreenSize,
        AccumBlueSize,
        AccumAlphaSize,
        Stereo,
        MultisampleBuffers,
        MultisampleSamples,
        AcceleratedVisual,
        RetainedBacking,
        ContextMajorVersion,
        ContextMinorVersion,
        ContextEgl,
        ContextFlags,
        ContextProfileMask,
        ShareWithCurrentContext,
        FramebufferSrgbCapable,
        ContextReleaseBehavior
    }

    [Flags]
    public enum GLContextProfile
    {
        Core = 0x0001,
        Compatibility = 0x0002,
        ES = 0x0004
    }

    [Flags]
    public enum GLContextFlag
    {
        Debug = 0x0001,
        ForwardCompatible = 0x0002,
        RobustAccess = 0x0004,
        ResetIsolation = 0x0008
    }

    public enum WindowEventID : byte
    {
        None,
        Shown,         
        Hidden,      
        Exposed,      
        Moved,
        Resized,
        SizeChanged,
        Minimized, 
        Maximized, 
        Restored,
        Enter,
        Leave, 
        FocusGained,
        FocusLost,
        Close, 
        TakeFocus,
        HitTest
    }

    [Flags]
    public enum WindowFlags
    {
        Fullscreen = 0x00000001,
        OpenGL = 0x00000002,
        Shown = 0x00000004,
        Hidden = 0x00000008,
        Borderless = 0x00000010,
        Resizable = 0x00000020,
        Minimized = 0x00000040,
        Maximized = 0x00000080,
        InputGrabbed = 0x00000100,
        InputFocus = 0x00000200,
        MouseFocus = 0x00000400,
        FullscreenDesktop = (Fullscreen | 0x00001000),
        Foreign = 0x00000800,
        AllowHighDpi = 0x00002000,
        MouseCapture = 0x00004000,
        AlwaysOnTop = 0x00008000,
        SkipTaskbar = 0x00010000,
        Utility = 0x00020000,
        Tooltip = 0x00040000,
        PopupMenu = 0x00080000
    }

    public enum EventType : uint
    {
        FirstEvent = 0,
        Quit = 0x100,
        WindowEvent = 0x200,
        SysWMEvent,
        KeyDown = 0x300,
        KeyUp,
        TextEditing,
        TextInput,
        MouseMotion = 0x400,
        MouseButtonDown,
        MouseButtonUp,
        MouseWheel,
        JoyAxisMotion = 0x600,
        JoyBallMotion,
        JoyHatMotion,
        JoyButtonDown,
        JoyButtonUp,
        JoyDeviceAdded,
        JoyDeviceRemoved,
        ControllerAxisMotion = 0x650,
        ControllerButtonDown,
        ControllerButtonUp,
        ControllerDeviceAdded,
        ControllerDeviceRemoved,
        ControllerDeviceMapped,
        FingerDown = 0x700,
        FingerUp,
        FingerMotion,
        DollarGesture = 0x800,
        DollarRecord,
        MultiGesture,
        ClipboardUpdate = 0x900,
        DropFile = 0x1000,
        DropText,
        DropBegin,
        DropComplete,
        AudioDeviceAdded = 0x1100,
        AudioDeviceRemoved,
        RenderTargetsReset = 0x2000,
        RenderDeviceReset,
        UserEvent = 0x8000,
        LastEvent = 0xFFFF
    }

    [Flags]
    public enum KeyMod : ushort
    {
        None = 0x0000,
        LShift = 0x0001,
        RShift = 0x0002,
        LCtrl = 0x0040,
        RCtrl = 0x0080,
        LAlt = 0x0100,
        RAlt = 0x0200,
        LGui = 0x0400,
        RGui = 0x0800,
        Num = 0x1000,
        Caps = 0x2000,
        Mode = 0x4000,
        Reserved = 0x8000,
        Ctrl = (LCtrl | RCtrl),
        Shift = (LShift | RShift),
        Alt = (LAlt | RAlt),
        Gui = (LGui | RGui)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Window
    {
        internal IntPtr ptr;

        public static implicit operator bool(Window window)
        {
            return window.ptr != IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GLContext
    {
        internal IntPtr ptr;

        public static implicit operator bool(GLContext context)
        {
            return context.ptr != IntPtr.Zero;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Version
    {
        public byte Major;
        public byte Minor;
        public byte Patch;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DisplayMode
    {
        public UInt32 Format;
        public int W;
        public int H;
        public int RefreshRate;
        public IntPtr DriverData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public WindowEventID Event;
        byte padding1;
        byte padding2;
        byte padding3;
        public Int32 Data1;
        public Int32 Data2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeyboardEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public byte State;
        public byte Repeat;
        byte padding2;
        byte padding3;
        public KeySym Keysym;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KeySym
    {
        public int ScanCode;
        public int Sym;
        public KeyMod Mod;
        public UInt32 Unicode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TextEditingEvent
    {
        public const int TextSize = 32;

        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public fixed byte Text[TextSize];
        public Int32 Start;
        public Int32 Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct TextInputEvent
    {
        public const int TextSize = 32;

        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public fixed byte Text[TextSize];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseMotionEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public UInt32 Which;
        public byte State;
        byte padding1;
        byte padding2;
        byte padding3;
        public Int32 X;
        public Int32 Y;
        public Int32 XRel;
        public Int32 YRel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseButtonEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public UInt32 Which;
        public byte Button;
        public byte State;
        public byte Clicks;
        byte padding1;
        public Int32 X;
        public Int32 Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MouseWheelEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public UInt32 Which;
        public Int32 X;
        public Int32 Y;
        public UInt32 Direction;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JoyAxisEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Axis;
        byte padding1;
        byte padding2;
        byte padding3;
        public Int16 Value;
        UInt16 padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JoyBallEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Ball;
        byte padding1;
        byte padding2;
        byte padding3;
        public Int16 XRel;
        public Int16 YRel;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JoyHatEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Hat;
        public byte HatValue;
        byte padding1;
        byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JoyButtonEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Button;
        public byte State;
        byte padding1;
        byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JoyDeviceEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerAxisEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Axis;
        byte padding1;
        byte padding2;
        byte padding3;
        public Int16 Value;
        UInt16 padding4;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerButtonEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
        public byte Button;
        public byte State;
        byte padding1;
        byte padding2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerDeviceEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int32 Which;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchFingerEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int64 TouchID;
        public Int64 FingerID;
        public float X;
        public float Y;
        public float DX;
        public float DY;
        public float Pressure;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MultiGestureEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int64 TouchID;
        public float DTheta;
        public float DDist;
        public float X;
        public float Y;
        public UInt16 NumFingers;
        UInt16 padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DollarGestureEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public Int64 TouchID;
        public Int64 GestureID;
        public UInt32 NumFingers;
        public float Error;
        public float X;
        public float Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DropEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public IntPtr File;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct QuitEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public UInt32 WindowID;
        public Int32 Code;
        public IntPtr Data1;
        public IntPtr Data2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SysWMEvent
    {
        public EventType Type;
        public UInt32 TimeStamp;
        public IntPtr Msg;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Event
    {
        [FieldOffset(0)]
        public EventType Type;
        [FieldOffset(0)]
        public WindowEvent Window;
        [FieldOffset(0)]
        public KeyboardEvent Key;
        [FieldOffset(0)]
        public TextEditingEvent Edit;
        [FieldOffset(0)]
        public TextInputEvent Text;
        [FieldOffset(0)]
        public MouseMotionEvent Motion;
        [FieldOffset(0)]
        public MouseButtonEvent Button;
        [FieldOffset(0)]
        public MouseWheelEvent Wheel;
        [FieldOffset(0)]
        public JoyAxisEvent JAxis;
        [FieldOffset(0)]
        public JoyBallEvent JBall;
        [FieldOffset(0)]
        public JoyHatEvent JHat;
        [FieldOffset(0)]
        public JoyButtonEvent JButton;
        [FieldOffset(0)]
        public JoyDeviceEvent JDevice;
        [FieldOffset(0)]
        public ControllerAxisEvent CAxis;
        [FieldOffset(0)]
        public ControllerButtonEvent CButton;
        [FieldOffset(0)]
        public ControllerDeviceEvent CDevice;
        [FieldOffset(0)]
        public QuitEvent Quit;
        [FieldOffset(0)]
        public UserEvent User;
        [FieldOffset(0)]
        public SysWMEvent SysWM;
        [FieldOffset(0)]
        public TouchFingerEvent TFinger;
        [FieldOffset(0)]
        public MultiGestureEvent MGesture;
        [FieldOffset(0)]
        public DollarGestureEvent DGesture;
        [FieldOffset(0)]
        public DropEvent Drop;
    }
}