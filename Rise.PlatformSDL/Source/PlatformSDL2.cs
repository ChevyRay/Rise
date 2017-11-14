using System;
using System.IO;
using Rise.PlatformSDL.SDL2;
namespace Rise.PlatformSDL
{
    public class PlatformSDL2 : Platform
    {
        Window window;
        uint windowID;
        GLContext context;
        bool fullscreen;
        bool vsync;
        Event ev;

        public override string Title
        {
            get { return SDL.GetWindowTitle(window); }
        }

        public override string AssetPath
        {
            get { return string.Empty; }
        }

        public override string SavePath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }

        public override bool Fullscreen
        {
            get { return fullscreen; }
        }

        public override bool VSync
        {
            get { return vsync; }
        }

        public override void Init(string title, int width, int height)
        {
            //Initialize SDL
            var initFlags = SubSystem.Video | SubSystem.Timer | SubSystem.Events | SubSystem.Joystick | SubSystem.Haptic;
            if (SDL.Init(initFlags) != 0)
            {
                SDL.Quit();
                throw new Exception(SDL.GetError());
            }

            //OpenGL attributes
            SDL.GLSetAttribute(GLAttr.ContextMajorVersion, 3);
            SDL.GLSetAttribute(GLAttr.ContextMinorVersion, 3);
            SDL.GLSetAttribute(GLAttr.ContextProfileMask, (int)GLContextProfile.Core);
            SDL.GLSetAttribute(GLAttr.ContextFlags, (int)GLContextFlag.ForwardCompatible);
            SDL.GLSetAttribute(GLAttr.DoubleBuffer, 1);
            //SDL.GLSetAttribute(GLAttr.MultisampleBuffers, 1);
            //SDL.GLSetAttribute(GLAttr.MultisampleSamples, 3);

            //Create the window
            var windowFlags = WindowFlags.Shown | WindowFlags.OpenGL | WindowFlags.AllowHighDpi | WindowFlags.Resizable;
            window = SDL.CreateWindow(title, SDL.WindowPosCentered, SDL.WindowPosCentered, width, height, windowFlags);
            if (!window)
            {
                SDL.Quit();
                throw new Exception(SDL.GetError());
            }
            windowID = SDL.GetWindowID(window);

            //Create the OpenGL context
            context = SDL.GLCreateContext(window);
            if (!context)
            {
                SDL.Quit();
                throw new Exception(SDL.GetError());
            }
            SDL.GLMakeCurrent(window, context);
            SetVSync(true);
        }

        public override void Quit()
        {
            SDL.Quit();
        }

        public override IntPtr GetProcAddress(string proc)
        {
            return SDL.GLGetProcAddress(proc);
        }

        public override double GetTime()
        {
            return SDL.GetTicks() / 1000.0;
        }

        public override ulong GetClockFrequency()
        {
            return SDL.GetPerformanceFrequency();
        }

        public override ulong GetClockValue()
        {
            return SDL.GetPerformanceCounter();
        }

        public override void PollEvents()
        {
            while (SDL.PollEvent(out ev) == 1)
            {
                switch (ev.Type)
                {
                    case EventType.Quit:
                        OnQuit?.Invoke();
                        break;
                    case EventType.KeyDown:
                        OnKeyDown?.Invoke(ev.Key.Keysym.Sym, ev.Key.Keysym.ScanCode);
                        break;
                    case EventType.KeyUp:
                        OnKeyUp?.Invoke(ev.Key.Keysym.Sym, ev.Key.Keysym.ScanCode);
                        break;
                    case EventType.MouseMotion:
                        OnMouseMove?.Invoke(ev.Motion.X, ev.Motion.Y);
                        break;
                    case EventType.MouseButtonDown:
                        OnMouseButtonDown?.Invoke(ev.Button.Button);
                        break;
                    case EventType.MouseButtonUp:
                        OnMouseButtonUp?.Invoke(ev.Button.Button);
                        break;
                    case EventType.MouseWheel:
                        OnMouseScroll?.Invoke(ev.Wheel.X, ev.Wheel.Y);
                        break;
                    case EventType.JoyDeviceAdded:
                        OnJoyDeviceAdd?.Invoke(ev.JDevice.Which);
                        break;
                    case EventType.JoyDeviceRemoved:
                        OnJoyDeviceRemove?.Invoke(ev.JDevice.Which);
                        break;
                    case EventType.JoyButtonDown:
                        OnJoyButtonDown?.Invoke(ev.JButton.Which, ev.JButton.Button);
                        break;
                    case EventType.JoyButtonUp:
                        OnJoyButtonDown?.Invoke(ev.JButton.Which, ev.JButton.Button);
                        break;
                    case EventType.JoyAxisMotion:
                        OnJoyAxisMove?.Invoke(ev.JAxis.Which, ev.JAxis.Axis, ev.JAxis.Value / (float)short.MaxValue);
                        break;
                    case EventType.WindowEvent:
                        if (ev.Window.WindowID == windowID)
                        {
                            switch (ev.Window.Event)
                            {
                                case WindowEventID.Close:
                                    OnWinClose?.Invoke();
                                    break;
                                case WindowEventID.Shown:
                                    OnWinShown?.Invoke();
                                    break;
                                case WindowEventID.Hidden:
                                    OnWinHidden?.Invoke();
                                    break;
                                case WindowEventID.Exposed:
                                    OnWinExposed?.Invoke();
                                    break;
                                case WindowEventID.Moved:
                                    OnWinMoved?.Invoke();
                                    break;
                                case WindowEventID.Resized:
                                    OnWinResized?.Invoke();
                                    break;
                                case WindowEventID.Minimized:
                                    OnWinMinimized?.Invoke();
                                    break;
                                case WindowEventID.Maximized:
                                    OnWinMaximized?.Invoke();
                                    break;
                                case WindowEventID.Restored:
                                    OnWinRestored?.Invoke();
                                    break;
                                case WindowEventID.Enter:
                                    OnWinEnter?.Invoke();
                                    break;
                                case WindowEventID.Leave:
                                    OnWinLeave?.Invoke();
                                    break;
                                case WindowEventID.FocusGained:
                                    OnWinFocusGained?.Invoke();
                                    break;
                                case WindowEventID.FocusLost:
                                    OnWinFocusLost?.Invoke();
                                    break;
                                default:
                                    OnWinOtherEvent?.Invoke((int)ev.Window.Event);
                                    break;
                            }
                        }
                        break;
                    default:
                        OnOtherEvent?.Invoke((int)ev.Type);
                        break;
                }
            }
        }

        /*public event Action OnQuit;
        public event KeyEvent OnKeyDown;
        public event KeyEvent OnKeyRepeat;
        public event KeyEvent OnKeyUp;
        public event MouseMoveEvent OnMouseMove;
        public event MouseButtonEvent OnMouseButtonDown;
        public event MouseButtonEvent OnMouseButtonUp;
        public event MouseScrollEvent OnMouseScroll;
        public event JoyDeviceEvent OnJoyDeviceAdd;
        public event JoyDeviceEvent OnJoyDeviceRemove;
        public event JoyButtonEvent OnJoyButtonDown;
        public event JoyButtonEvent OnJoyButtonUp;
        public event JoyAxisEvent OnJoyAxisMove;
        public event Action OnWinClose;
        public event Action OnWinShown;
        public event Action OnWinHidden;
        public event Action OnWinExposed;
        public event Action OnWinMoved;
        public event Action OnWinResized;
        public event Action OnWinMinimized;
        public event Action OnWinMaximized;
        public event Action OnWinRestored;
        public event Action OnWinEnter;
        public event Action OnWinLeave;
        public event Action OnWinFocusGained;
        public event Action OnWinFocusLost;
        public event OtherEvent OnOtherEvent;*/

        public override void SwapBuffers()
        {
            SDL.GLSwapWindow(window);
        }

        public override void GetMousePosition(out int x, out int y)
        {
            SDL.GetMouseState(out x, out y);
        }

        public override void GetPosition(out int x, out int y)
        {
            SDL.GetWindowPosition(window, out x, out y);
        }

        public override void GetSize(out int w, out int h)
        {
            SDL.GetWindowSize(window, out w, out h);
        }

        public override void GetDrawSize(out int w, out int h)
        {
            SDL.GLGetDrawableSize(window, out w, out h);
        }

        public override void GetBorderSize(out int top, out int left, out int bottom, out int right)
        {
            SDL.GetWindowBorderSize(window, out top, out left, out bottom, out right);
        }

        public override void SetSize(int w, int h)
        {
            SDL.SetWindowSize(window, w, h);
        }

        public override void SetFullScreen(bool enabled)
        {
            if (fullscreen != enabled)
            {
                if (SDL.SetWindowFullscreen(window, enabled ? FullscreenMode.Desktop : FullscreenMode.Off) == 0)
                    fullscreen = enabled;
            }
        }

        public override void SetVSync(bool enabled)
        {
            if (vsync != enabled)
            {
                if (SDL.GLSetSwapInterval(enabled ? 1 : 0) == 0)
                    vsync = enabled;
            }
        }

        public override void SetResizable(bool resizable)
        {
            SDL.SetWindowResizable(window, resizable);
        }

        public override void CenterWindow()
        {
            SDL.SetWindowPosition(window, SDL.WindowPosCentered, SDL.WindowPosCentered);
        }

        public override bool BeginIO()
        {
            return true;
        }

        /*public override string ReadTextFile(string file)
        {
            if (!File.Exists(file))
                return null;
            return File.ReadAllText(file);
        }

        public override Color[] ReadImageFile(string file, out int w, out int h)
        {
            if (File.Exists(file))
            {
                
            }
            w = 0;
            h = 0;
            return null;
        }

        public override bool WriteTextFile(string file, ref string text)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                return false;
            File.WriteAllText(file, text);
            return true;
        }

        public override bool WriteImageFile(string file, int w, int h, Color[] bytes)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                return false;



            return true;
        }*/

        public override bool EndIO()
        {
            return true;
        }

        public override string GetClipboard()
        {
            return SDL.GetClipboardText();
        }

        public override void SetClipboard(ref string text)
        {
            SDL.SetClipboardText(text);
        }
    }
}
