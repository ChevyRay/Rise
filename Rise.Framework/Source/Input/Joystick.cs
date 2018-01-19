using System;
using System.Collections.Generic;
namespace Rise
{
    public static class Joystick
    {
        static List<JoystickState> joysticks = new List<JoystickState>();
        static Dictionary<int, JoystickState> lookup = new Dictionary<int, JoystickState>();

        internal static void Init()
        {
            App.platform.OnJoyDeviceAdd += DoAdded;
            App.platform.OnJoyDeviceRemove += DoRemoved;
            App.platform.OnJoyButtonDown += DoButtonDown;
            App.platform.OnJoyButtonUp += DoButtonUp;
        }

        internal static void PostUpdate()
        {
            for (int i = 0; i < joysticks.Count; ++i)
                joysticks[i].PostUpdate();
        }

        internal static void DoAdded(int id)
        {
            //DoRemoved(id);
            var joy = new JoystickState(id);
            joysticks.Add(joy);
            lookup[id] = joy;
        }

        internal static void DoRemoved(int id)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
            {
                joysticks.RemoveAt(joysticks.IndexOf(joy));
                lookup.Remove(id);
            }
        }

        internal static void DoButtonDown(int id, int button)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
            {
                UInt64 bit = (UInt64)(1 << button);
                joy.Down |= bit;
                joy.Pressed |= bit;
            }
        }

        internal static void DoButtonUp(int id, int button)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
            {
                UInt64 bit = (UInt64)(1 << button);
                joy.Down &= ~bit;
                joy.Released |= bit;
            }
        }

        internal static void DoAxisMotion(int id, int axis, float value)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                SetValue(ref joy.Axes, axis, value);
        }

        internal static void DoBallMotion(int id, int ball, int x, int y)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                SetValue(ref joy.Balls, ball, new Point2(x, y));
        }

        internal static void DoHatMotion(int id, int hat, HatPosition position)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                SetValue(ref joy.Hats, hat, position);
        }

        static void SetValue<T>(ref List<T> list, int index, T value)
        {
            if (list == null)
                list = new List<T>();
            while (list.Count <= index)
                list.Add(default(T));
            list[index] = value;
        }

        static T GetValue<T>(List<T> list, int index)
        {
            if (list == null || index < 0 || index >= list.Count)
                return default(T);
            return list[index];
        }

        public static bool Connected(int id)
        {
            return lookup.ContainsKey(id);
        }

        public static bool ButtonDown(int id, int button)
        {
            JoystickState joy;
            return lookup.TryGetValue(id, out joy) && (joy.Down & (UInt64)(1 << button)) == 1;
        }

        public static bool ButtonPressed(int id, int button)
        {
            JoystickState joy;
            return lookup.TryGetValue(id, out joy) && (joy.Pressed & (UInt64)(1 << button)) == 1;
        }

        public static bool ButtonReleased(int id, int button)
        {
            JoystickState joy;
            return lookup.TryGetValue(id, out joy) && (joy.Released & (UInt64)(1 << button)) == 1;
        }

        public static float GetAxis(int id, int axis)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                return GetValue(joy.Axes, axis);
            return 0f;
        }

        public static Point2 GetBall(int id, int ball)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                return GetValue(joy.Balls, ball);
            return Point2.Zero;
        }

        public static HatPosition GetHat(int id, int hat)
        {
            JoystickState joy;
            if (lookup.TryGetValue(id, out joy))
                return GetValue(joy.Hats, hat);
            return HatPosition.Center;
        }
    }

    public class JoystickState
    {
        public int ID;
        public UInt64 Down;
        public UInt64 Pressed;
        public UInt64 Released;
        public List<float> Axes;
        public List<HatPosition> Hats;
        public List<Point2> Balls;

        public JoystickState(int id)
        {
            ID = id;
        }

        public void PostUpdate()
        {
            Pressed = 0;
            Released = 0;
            if (Balls != null)
                for (int i = 0; i < Balls.Count; ++i)
                    Balls[i] = Point2.Zero;
        }
    }

    public enum HatPosition : byte
    {
        Center = 0,
        Up = 1,
        Right = 2,
        Down = 4,
        Left = 8,
        RightUp = 2 | 1,
        RightDown = 2 | 4,
        LeftUp = 8 | 1,
        LeftDown = 8 | 4
    }
}
