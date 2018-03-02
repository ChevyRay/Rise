using System;
using System.Collections;
using System.Collections.Generic;
namespace Rise
{
    public class Polygon : IShape
    {
        internal List<Vector2> points;
        internal List<Vector2> axes;
        internal Vector2 centroid;
        bool isConvex;
        bool dirty = true;

        public Polygon(int capacity)
        {
            points = new List<Vector2>(capacity);
            axes = new List<Vector2>(capacity);
        }
        public Polygon()
        {
            points = new List<Vector2>();
            axes = new List<Vector2>();
        }

        public int PointCount
        {
            get { return points.Count; }
        }

        public Vector2 Centroid
        {
            get
            {
                Update();
                return centroid;
            }
        }

        public bool IsConvex
        {
            get
            {
                Update();
                return isConvex;
            }
        }

        public void Clear()
        {
            if (points.Count > 0)
            {
                points.Clear();
                dirty = true;
            }
        }

        public void AddPoint(Vector2 p)
        {
            points.Add(p);
            dirty = true;
        }

        public bool Contains(Vector2 point)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                var a = points[i];
                var b = points[(i + 1) % points.Count];
                var c = points[(i + 2) % points.Count];
                if (Vector2.Cross(b - a, c - b) < 0f)
                    return false;
            }
            return true;
        }
        public bool Contains(ref Circle circle)
        {
            return Geom2D.Contains(this, ref circle);
        }
        public bool Contains(ref Rectangle rect)
        {
            return Geom2D.Contains(this, ref rect);
        }
        public bool Contains(ref Triangle tri)
        {
            return Geom2D.Contains(this, ref tri);
        }
        public bool Contains(ref Quad quad)
        {
            return Geom2D.Contains(this, ref quad);
        }
        public bool Contains(Polygon poly)
        {
            return Geom2D.Contains(this, poly);
        }

        public float DistanceTo(Vector2 point)
        {
            if (Contains(point))
                return 0f;
            return Vector2.Distance(point, Project(point));
        }
        public float DistanceTo(ref Circle circle)
        {
            return Geom2D.Distance(this, ref circle);
        }
        public float DistanceTo(ref Rectangle rect)
        {
            return Geom2D.Distance(this, ref rect);
        }
        public float DistanceTo(ref Triangle tri)
        {
            return Geom2D.Distance(this, ref tri);
        }
        public float DistanceTo(ref Quad quad)
        {
            return Geom2D.Distance(this, ref quad);
        }
        public float DistanceTo(Polygon poly)
        {
            return Geom2D.Distance(this, poly);
        }

        public void GetBounds(out Rectangle result)
        {
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            Vector2 p;
            for (int i = 0; i < points.Count; ++i)
            {
                p = points[i];
                min = Vector2.Min(p, min);
                max = Vector2.Max(p, max);
            }
            result.X = min.X;
            result.Y = min.Y;
            result.W = max.X - min.X;
            result.H = max.Y - min.Y;
        }

        public Vector2 GetPoint(int i)
        {
            return points[i];
        }

        public bool Intersects(ref Circle circle)
        {
            return Geom2D.Intersects(this, ref circle);
        }
        public bool Intersects(ref Rectangle rect)
        {
            return Geom2D.Intersects(this, ref rect);
        }
        public bool Intersects(ref Triangle tri)
        {
            return Geom2D.Intersects(this, ref tri);
        }
        public bool Intersects(ref Quad quad)
        {
            return Geom2D.Intersects(this, ref quad);
        }
        public bool Intersects(Polygon poly)
        {
            return Geom2D.Intersects(this, poly);
        }

        public bool Intersects(Vector2 point, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, point, out pushOut);
        }
        public bool Intersects(ref Circle circle, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, ref circle, out pushOut);
        }
        public bool Intersects(ref Rectangle rect, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, ref rect, out pushOut);
        }
        public bool Intersects(ref Triangle tri, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, ref tri, out pushOut);
        }
        public bool Intersects(ref Quad quad, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, ref quad, out pushOut);
        }
        public bool Intersects(Polygon poly, out Vector2 pushOut)
        {
            return Geom2D.Intersects(this, poly, out pushOut);
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            Geom2D.Project(this, axis, out min, out max);
        }
        public Vector2 Project(Vector2 point)
        {
            return Geom2D.Project(this, point);
        }

        public bool Raycast(ref Ray2D ray)
        {
            return Geom2D.Raycast(this, ref ray);
        }
        public bool Raycast(ref Ray2D ray, out RayHit2D hit)
        {
            return Geom2D.Raycast(this, ref ray, out hit);
        }
        public bool Raycast(ref Ray2D ray, out float dist)
        {
            return Geom2D.Raycast(this, ref ray, out dist);
        }

        public void RemovePoint(int i)
        {
            points.RemoveAt(i);
            dirty = true;
        }

        public void SetPoint(int i, Vector2 p)
        {
            if (!points[i].Equals(p))
            {
                points[i] = p;
                dirty = true;
            }
        }

        public void SetPoints(IEnumerable<Vector2> values)
        {
            points.Clear();
            points.AddRange(values);
            dirty = true;
        }

        public void Update()
        {
            if (dirty)
            {
                dirty = false;
                isConvex = true;
                centroid = Vector2.Zero;
                float signedArea = 0f;
                axes.Clear();
                for (int i = 0; i < points.Count; ++i)
                {
                    var a = points[i];
                    var b = points[(i + 1) % points.Count];
                    var area = a.X * b.Y - b.X * a.Y;
                    signedArea += area;
                    centroid += (a + b) * area;
                    axes.Add(Vector2.TurnLeft(Vector2.Normalize(b - a)));
                    if (Vector2.Cross(a, b) <= 0f)
                        isConvex = false;
                }
                centroid /= (signedArea * 3f);
            }
        }

        public void ExtendFromCentroid(float distance)
        {
            if (distance != 0f)
            {
                Update();
                for (int i = 0; i < points.Count; ++i)
                    points[i] += Vector2.Normalize(points[i] - centroid, distance);
                dirty = true;
            }
        }

        public float GetArea()
        {
            float area = 0;
            int j = points.Count - 1;
            for (int i = 0; i < points.Count; ++i)
            {
                area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
                j = i;
            }
            return Math.Abs(area * 0.5f);
        }
    }
}
