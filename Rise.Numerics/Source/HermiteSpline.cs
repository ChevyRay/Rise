using System;
namespace Rise
{
    public class HermiteSpline
    {
        struct HermitePoint
        {
            public float Value;
            public Vector2 Tangents;
        }

        HermitePoint[] points;
        int count;

        public HermiteSpline(int count)
        {
            Reserve(count);
            this.count = count;
        }
        public HermiteSpline() : this(4)
        {

        }

        void Reserve(int n)
        {
            if (points.Length < n)
            {
                int len = points.Length;
                while (len < n)
                    len *= 2;
                Array.Resize(ref points, len);
            }
        }

        public void Clear()
        {
            count = 0;
        }

        public void Add(float value, Vector2 tangents)
        {
            Reserve(count + 1);
            points[count].Value = value;
            points[count++].Tangents = tangents;
        }
        public void Add(float value, float t1, float t2)
        {
            Reserve(count + 1);
            points[count].Value = value;
            points[count].Tangents.X = t1;
            points[count++].Tangents.Y = t2;
        }
        public void Add(float value)
        {
            Reserve(count + 1);
            points[count].Value = value;
            points[count++].Tangents = Vector2.Zero;
        }

        public void AddValue(float value)
        {
            Add(value, Vector2.Zero);
        }

        public void Set(int i, float value, Vector2 tangents)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Value = value;
            points[i].Tangents = tangents;
        }
        public void Set(int i, float value, float t1, float t2)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Value = value;
            points[i].Tangents = new Vector2(t1, t2);
        }

        public void SetValue(int i, float value)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Value = value;
        }

        public void SetTangents(int i, Vector2 tangents)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Tangents = tangents;
        }
        public void SetTangents(int i, float t1, float t2)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Tangents = new Vector2(t1, t2);
        }

        public void Get(int i, out float value, out Vector2 tangents)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            value = points[i].Value;
            tangents = points[i].Tangents;
        }
        public void Get(int i, out float value, out float t1, out float t2)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            value = points[i].Value;
            t1 = points[i].Tangents.X;
            t2 = points[i].Tangents.Y;
        }

        public float GetValue(int i)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            return points[i].Value;
        }

        public Vector2 GetTangents(int i)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            return points[i].Tangents;
        }
        public void GetTangents(int i, out float t1, out float t2)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            t1 = points[i].Tangents.X;
            t2 = points[i].Tangents.Y;
        }

        public void CalculateCatmullRom(bool loop)
        {
            CalculateKochanekBartels(0f, 0f, 0f, loop);
        }

        public void CalculateKochanekBartels(float tension, float bias, float continuity, bool loop)
        {
            //Calculate tangent multipliers
            float t0a = ((1f - tension) * (1f + bias) * (1f + continuity)) * 0.5f;
            float t0b = ((1f - tension) * (1f - bias) * (1f - continuity)) * 0.5f;
            float t1a = ((1f - tension) * (1f + bias) * (1f - continuity)) * 0.5f;
            float t1b = ((1f - tension) * (1f - bias) * (1f + continuity)) * 0.5f;

            //Calculate the tangents
            float p0, p1, p2;
            if (loop)
            {
                for (int i = 0; i < count; ++i)
                {
                    p0 = points[((i - 1) + count) % count].Value;
                    p1 = points[((i + 1) + count) % count].Value;
                    p2 = points[((i + 2) + count) % count].Value;
                    points[i].Tangents.X = t0a * (points[i].Value - p0) + t0b * (p1 - points[i].Value);
                    points[i].Tangents.Y = t1a * (p1 - points[i].Value) + t1b * (p2 - p1);
                }
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    p0 = points[Math.Max(i - 1, 0)].Value;
                    p1 = points[Math.Min(i + 1, count - 1)].Value;
                    p2 = points[Math.Min(i + 2, count - 1)].Value;
                    points[i].Tangents.X = t0a * (points[i].Value - p0) + t0b * (p1 - points[i].Value);
                    points[i].Tangents.Y = t1a * (p1 - points[i].Value) + t1b * (p2 - p1);
                }
            }
        }

        public float GetPosition(int i, float percent)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            return Calc.Hermite(points[i].Value, points[i].Tangents.X, points[i + 1].Value, points[i].Tangents.Y, percent);
        }
        public float GetPosition(float percent)
        {
            var p = percent * (count - 1);
            return GetPosition((int)p, p % 1f);
        }

        public float GetPositionLooped(int i, float percent)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            return Calc.Hermite(points[i].Value, points[i].Tangents.X, points[(i + 1) % count].Value, points[i].Tangents.Y, percent);
        }
        public float GetPositionLooped(float percent)
        {
            var p = percent * (count - 1);
            return GetPositionLooped((int)p, p % 1f);
        }

        public float ApproxLength(float precision)
        {
            if (count == 0)
                return 0f;
            float len = 0f;
            float curr;
            float prev = points[0].Value;
            for (float t = 0f; t < 1f; t += precision)
            {
                curr = GetPosition(t);
                len += Math.Abs(curr - prev);
                prev = curr;
            }
            len += Math.Abs(points[count - 1].Value - prev);
            return len;
        }

        public int Count
        {
            get { return count; }
        }
    }
}
