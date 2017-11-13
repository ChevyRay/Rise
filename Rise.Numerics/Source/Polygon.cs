﻿using System;
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
            return Geom.Contains(this, ref circle);
        }
        public bool Contains(ref Rectangle rect)
        {
            return Geom.Contains(this, ref rect);
        }
        public bool Contains(ref Triangle tri)
        {
            return Geom.Contains(this, ref tri);
        }
        public bool Contains(ref Quad quad)
        {
            return Geom.Contains(this, ref quad);
        }
        public bool Contains(Polygon poly)
        {
            return Geom.Contains(this, poly);
        }

        public float DistanceTo(Vector2 point)
        {
            if (Contains(point))
                return 0f;
            return Vector2.Distance(point, Project(point));
        }
        public float DistanceTo(ref Circle circle)
        {
            return Geom.Distance(this, ref circle);
        }
        public float DistanceTo(ref Rectangle rect)
        {
            return Geom.Distance(this, ref rect);
        }
        public float DistanceTo(ref Triangle tri)
        {
            return Geom.Distance(this, ref tri);
        }
        public float DistanceTo(ref Quad quad)
        {
            return Geom.Distance(this, ref quad);
        }
        public float DistanceTo(Polygon poly)
        {
            return Geom.Distance(this, poly);
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
            return Geom.Intersects(this, ref circle);
        }
        public bool Intersects(ref Rectangle rect)
        {
            return Geom.Intersects(this, ref rect);
        }
        public bool Intersects(ref Triangle tri)
        {
            return Geom.Intersects(this, ref tri);
        }
        public bool Intersects(ref Quad quad)
        {
            return Geom.Intersects(this, ref quad);
        }
        public bool Intersects(Polygon poly)
        {
            return Geom.Intersects(this, poly);
        }

        public bool Intersects(Vector2 point, out Vector2 pushOut)
        {
            return Geom.Intersects(this, point, out pushOut);
        }
        public bool Intersects(ref Circle circle, out Vector2 pushOut)
        {
            return Geom.Intersects(this, ref circle, out pushOut);
        }
        public bool Intersects(ref Rectangle rect, out Vector2 pushOut)
        {
            return Geom.Intersects(this, ref rect, out pushOut);
        }
        public bool Intersects(ref Triangle tri, out Vector2 pushOut)
        {
            return Geom.Intersects(this, ref tri, out pushOut);
        }
        public bool Intersects(ref Quad quad, out Vector2 pushOut)
        {
            return Geom.Intersects(this, ref quad, out pushOut);
        }
        public bool Intersects(Polygon poly, out Vector2 pushOut)
        {
            return Geom.Intersects(this, poly, out pushOut);
        }

        public void Project(Vector2 axis, out float min, out float max)
        {
            Geom.Project(this, axis, out min, out max);
        }
        public Vector2 Project(Vector2 point)
        {
            return Geom.Project(this, point);
        }

        public bool Raycast(ref Ray ray)
        {
            return Geom.Raycast(this, ref ray);
        }
        public bool Raycast(ref Ray ray, out RayHit hit)
        {
            return Geom.Raycast(this, ref ray, out hit);
        }
        public bool Raycast(ref Ray ray, out float dist)
        {
            return Geom.Raycast(this, ref ray, out dist);
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
    }
}
