using System;
namespace GameEngine
{
    public struct Vector2
    {
        public static readonly Vector2 Zero;
        public static readonly Vector2 One = new Vector2(1f, 1f);

        public float X;
        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
