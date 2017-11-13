using System;
namespace Rise
{
    public class LinearSpline
    {
        struct LinearPoint
        {
            public Vector2 Value;
            public float Length;
        }

        LinearPoint[] points = new LinearPoint[4];
        int count;
        float length;
        bool updateLengths = true;

        public LinearSpline(int count)
        {
            this.count = count;
            Reserve(count);
        }
        public LinearSpline() : this(4)
        {

        }

        void UpdateLengths()
        {
            updateLengths = false;
            length = 0f;
            if (count > 0)
            {
                for (int i = 0, j = 1; j < count; ++i, ++j)
                {
                    points[i].Length = Vector2.Distance(points[i].Value, points[j].Value);
                    length += points[i].Length;
                }
                points[count - 1].Length = 0f;
            }
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
            if (count > 0)
            {
                count = 0;
                updateLengths = true;
            }
        }

        public void AddPoint(Vector2 value)
        {
            Reserve(count + 1);
            points[count++].Value = value;
            updateLengths = true;
        }

        public void SetPoint(int i, Vector2 value)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            points[i].Value = value;
            updateLengths = true;
        }

        public Vector2 GetPoint(int i)
        {
            if (i < 0 || i >= count)
                throw new IndexOutOfRangeException(nameof(i));
            return points[i].Value;
        }

        public Vector2 GetPosition(float dist)
        {
            if (updateLengths)
                UpdateLengths();

            if (dist >= length)
                return points[count - 1].Value;
            if (dist < 0)
                return points[0].Value;

            int i = 0;
            while (dist > points[i].Length)
                dist -= points[i++].Length;

            return Vector2.Lerp(points[i].Value, points[i + 1].Value, dist / points[i].Length);
        }

        /*public void ExtrapolateWithPointDist(HermiteSpline spline, float pointDist, bool loop)
        {
            
        }

        public void ExtrapolateWithPointCount(HermiteSpline spline, int pointCount, bool loop)
        {

        }

        public void ExtrapolateWithPrecision(HermiteSpline spline, float precision, bool loop)
        {

        }*/

        /*public void GenerateFromHermite(HermiteSpline spline, int count, bool loop)
        {
            Reserve(count);
            this.count = count;
            float n = count;
            if (loop)
            {
                for (int i = 0; i < count; ++i)
                    points[i].Value = spline.GetPosition(i / n);
            }
            else
            {
                for (int i = 0; i < count; ++i)
                    points[i].Value = spline.GetPosition(i / n);
            }
            updateLengths = true;
        }*/

        public int Count
        {
            get { return count; }
        }

        public float Length
        {
            get
            {
                if (updateLengths)
                    UpdateLengths();
                return length;
            }
        }
    }
}
