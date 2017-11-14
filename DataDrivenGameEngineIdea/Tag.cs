using System;
namespace GameEngine
{
    public struct Tag
    {
        public static readonly Tag None = new Tag(0);
        public static readonly Tag Default = new Tag(1);
        public static readonly Tag All = new Tag(uint.MaxValue);

        public uint Value;

        public Tag(uint value)
        {
            Value = value;
        }

        public static implicit operator uint(Tag tag)
        {
            return tag.Value;
        }

        public static explicit operator Tag(uint value)
        {
            return new Tag(value);
        }

        public static Tag operator !(Tag a)
        {
            return new Tag(~a.Value);
        }

        public static Tag operator ~(Tag a)
        {
            return new Tag(~a.Value);
        }

        public static Tag operator &(Tag a, Tag b)
        {
            return new Tag(a.Value & b.Value);
        }

        public static Tag operator |(Tag a, Tag b)
        {
            return new Tag(a.Value | b.Value);
        }

        public static Tag operator ^(Tag a, Tag b)
        {
            return new Tag(a.Value ^ b.Value);
        }

        public static Tag operator +(Tag a, Tag b)
        {
            return new Tag(a.Value | b.Value);
        }

        public static Tag operator -(Tag a, Tag b)
        {
            return new Tag(a.Value & ~b.Value);
        }
    }
}
