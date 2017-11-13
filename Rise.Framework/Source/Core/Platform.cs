using System;
namespace Rise
{
    public abstract class Platform
    {
        public delegate void KeyEvent(int keyCode, int scanCode);
        public delegate void MouseMoveEvent(int x, int y);
        public delegate void MouseButtonEvent(int buttonID);
        public delegate void MouseScrollEvent(int x, int y);
        public delegate void JoyDeviceEvent(int deviceID);
        public delegate void JoyButtonEvent(int deviceID, int buttonID);
        public delegate void JoyAxisEvent(int deviceID, int axisID, float value);
        public delegate void OtherEvent(int eventID);

#pragma warning disable 0067
        public Action OnQuit;
        public KeyEvent OnKeyDown;
        public KeyEvent OnKeyRepeat;
        public KeyEvent OnKeyUp;
        public MouseMoveEvent OnMouseMove;
        public MouseButtonEvent OnMouseButtonDown;
        public MouseButtonEvent OnMouseButtonUp;
        public MouseScrollEvent OnMouseScroll;
        public JoyDeviceEvent OnJoyDeviceAdd;
        public JoyDeviceEvent OnJoyDeviceRemove;
        public JoyButtonEvent OnJoyButtonDown;
        public JoyButtonEvent OnJoyButtonUp;
        public JoyAxisEvent OnJoyAxisMove;
        public Action OnWinClose;
        public Action OnWinShown;
        public Action OnWinHidden;
        public Action OnWinExposed;
        public Action OnWinMoved;
        public Action OnWinResized;
        public Action OnWinMinimized;
        public Action OnWinMaximized;
        public Action OnWinRestored;
        public Action OnWinEnter;
        public Action OnWinLeave;
        public Action OnWinFocusGained;
        public Action OnWinFocusLost;
        public OtherEvent OnWinOtherEvent;
        public OtherEvent OnOtherEvent;
#pragma warning restore 0067

        public abstract string Title { get; }
        public abstract string AssetPath { get; }
        public abstract string SavePath { get; }
        public abstract bool Fullscreen { get; }
        public abstract bool VSync { get; }

        public abstract void Init(string title, int width, int height);
        public abstract void Quit();

        public abstract IntPtr GetProcAddress(string proc);
        //public abstract uint GetTicks();

        public abstract double GetTime();
        public abstract ulong GetClockFrequency();
        public abstract ulong GetClockValue();

        public abstract void PollEvents();
        public abstract void SwapBuffers();

        public abstract void GetMousePosition(out int x, out int y);
        public abstract void GetPosition(out int x, out int y);
        public abstract void GetSize(out int w, out int h);
        public abstract void GetDrawSize(out int w, out int h);
        public abstract void GetBorderSize(out int top, out int left, out int bottom, out int right);

        public abstract void SetSize(int w, int h);
        public abstract void SetFullScreen(bool enabled);
        public abstract void SetVSync(bool enabled);
        public abstract void SetResizable(bool resizable);
        public abstract void CenterWindow();

        public abstract bool BeginIO();
        public abstract string ReadTextFile(string file);
        public abstract Color[] ReadImageFile(string file, out int w, out int h);
        public abstract bool WriteTextFile(string file, ref string text);
        public abstract bool WriteImageFile(string file, int w, int h, Color[] bytes);
        public abstract bool EndIO();

        public abstract string GetClipboard();
        public abstract void SetClipboard(ref string text);
    }
}
