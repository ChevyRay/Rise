using System;
namespace Rise
{
    public delegate float Easer(float t);

    public enum EaseType
    {
        Linear = 0,
        QuadIn, QuadOut, QuadInOut,
        CubeIn, CubeOut, CubeInOut,
        BackIn, BackOut, BackInOut,
        ExpoIn, ExpoOut, ExpoInOut,
        SineIn, SineOut, SineInOut,
        ElasticIn, ElasticOut, ElasticInOut
    }

    public static class Ease
    {
        public static readonly Easer Linear = (t) => { return t; };
        public static readonly Easer QuadIn = (t) => { return t * t; };
        public static readonly Easer QuadOut = (t) => { return 1f - QuadIn(1f - t); };
        public static readonly Easer QuadInOut = (t) => { return (t <= 0.5f) ? QuadIn(t * 2f) * 0.5f : QuadOut(t * 2f - 1f) * 0.5f + 0.5f; };
        public static readonly Easer CubeIn = (t) => { return t * t * t; };
        public static readonly Easer CubeOut = (t) => { return 1f - CubeIn(1f - t); };
        public static readonly Easer CubeInOut = (t) => { return (t <= 0.5f) ? CubeIn(t * 2f) * 0.5f : CubeOut(t * 2f - 1f) * 0.5f + 0.5f; };
        public static readonly Easer BackIn = (t) => { return t * t * (2.70158f * t - 1.70158f); };
        public static readonly Easer BackOut = (t) => { return 1f - BackIn(1f - t); };
        public static readonly Easer BackInOut = (t) => { return (t <= 0.5f) ? BackIn(t * 2f) * 0.5f : BackOut(t * 2f - 1f) * 0.5f + 0.5f; };
        public static readonly Easer ElasticIn = (t) => { return 1f - ElasticOut(1f - t); };
        public static readonly Easer ElasticOut = (t) => { return Calc.Pow(2f, -10f * t) * Calc.Sin((t - 0.075f) * (2f * Calc.PI) / 0.3f) + 1f; };
        public static readonly Easer ElasticInOut = (t) => { return (t <= 0.5f) ? ElasticIn(t * 2f) * 0.5f : ElasticOut(t * 2f - 1f) * 0.5f + 0.5f; };

        public static string ToString(Easer ease)
        {
            if (ease == Linear)
                return "Linear";
            else if (ease == QuadIn)
                return "QuadIn";
            else if (ease == QuadOut)
                return "QuadOut";
            else if (ease == QuadInOut)
                return "QuadInOut";
            else if (ease == CubeIn)
                return "CubeIn";
            else if (ease == CubeOut)
                return "CubeOut";
            else if (ease == CubeInOut)
                return "CubeInOut";
            else if (ease == BackIn)
                return "BackIn";
            else if (ease == BackOut)
                return "BackOut";
            else if (ease == BackInOut)
                return "BackInOut";
            else if (ease == ElasticIn)
                return "ElasticIn";
            else if (ease == ElasticOut)
                return "ElasticOut";
            else if (ease == ElasticInOut)
                return "ElasticInOut";
            return null;
        }

        public static Easer FromString(string name)
        {
            EaseType type;
            if (Enum.TryParse(name, out type))
                return FromType(type);
            return null;
        }

        public static Easer FromType(EaseType type)
        {
            switch (type)
            {
                case EaseType.Linear: return Linear;
                case EaseType.QuadIn: return QuadIn;
                case EaseType.QuadOut: return QuadOut;
                case EaseType.QuadInOut: return QuadInOut;
                case EaseType.CubeIn: return CubeIn;
                case EaseType.CubeOut: return CubeOut;
                case EaseType.CubeInOut: return CubeInOut;
                case EaseType.BackIn: return BackIn;
                case EaseType.BackOut: return BackOut;
                case EaseType.BackInOut: return BackInOut;
                case EaseType.ElasticIn: return ElasticIn;
                case EaseType.ElasticOut: return ElasticOut;
                case EaseType.ElasticInOut: return ElasticInOut;
                default: return Linear; 
            }
        }
    }
}

