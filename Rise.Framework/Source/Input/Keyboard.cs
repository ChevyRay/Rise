using System.Collections.Generic;
using System;
namespace Rise
{
    public delegate void KeyEvent(KeyCode key);

    public static class Keyboard
    {
        public const int ScanCodeMask = (1 << 30);

        public static event KeyEvent OnPress;
        public static event KeyEvent OnRelease;
        public static event KeyEvent OnRepeat;

        static HashSet<KeyCode> down = new HashSet<KeyCode>();
        static HashSet<KeyCode> pressed = new HashSet<KeyCode>();
        static HashSet<KeyCode> released = new HashSet<KeyCode>();
        static HashSet<KeyCode> repeated = new HashSet<KeyCode>();

        internal static void Init()
        {
            App.platform.OnKeyDown += (key, scan) =>
            {
                down.Add((KeyCode)key);
                pressed.Add((KeyCode)key);
                OnPress?.Invoke((KeyCode)key);
            };

            App.platform.OnKeyUp += (key, scan) =>
            {
                down.Remove((KeyCode)key);
                released.Add((KeyCode)key);
                OnRelease?.Invoke((KeyCode)key);
            };

            App.platform.OnKeyRepeat += (key, scan) =>
            {
                repeated.Add((KeyCode)key);
                OnRepeat?.Invoke((KeyCode)key);
            };
        }

        internal static void PostUpdate()
        {
            pressed.Clear();
            released.Clear();
            repeated.Clear();
        }

        public static bool Down(KeyCode key)
        {
            return down.Contains(key);
        }

        public static bool Pressed(KeyCode key)
        {
            return pressed.Contains(key);
        }

        public static bool Released(KeyCode key)
        {
            return released.Contains(key);
        }

        public static bool Repeated(KeyCode key)
        {
            return repeated.Contains(key);
        }

        public static bool PressedOrRepeated(KeyCode key)
        {
            return repeated.Contains(key) || pressed.Contains(key);
        }
    }
}
