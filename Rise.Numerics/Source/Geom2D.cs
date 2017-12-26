using System;
namespace Rise
{
    public static class Geom2D
    {
        static bool IsSeparatingAxis<T, U>(ref T a, ref U b, Vector2 axis) where T : IShape where U : IShape
        {
            float min0, max0, min1, max1;
            a.Project(axis, out min0, out max0);
            b.Project(axis, out min1, out max1);
            return min0 >= max1 || max0 <= min1;
        }

        static bool GetPushOut<T, U>(ref T a, ref U b, Vector2 axis, ref float min, ref Vector2 result) where T : IShape where U : IShape
        {
            float min0, max0, min1, max1;
            a.Project(axis, out min0, out max0);
            b.Project(axis, out min1, out max1);
            if (min0 >= max1 || max0 <= min1)
                return false;
            float dist1 = Math.Abs(min1 - max0);
            float dist2 = Math.Abs(max1 - min0);
            if (dist1 < dist2)
            {
                if (dist1 < min)
                {
                    min = dist1;
                    result = axis * (min1 - max0);
                }
            }
            else if (dist2 < min)
            {
                min = dist2;
                result = axis * (max1 - min0);
            }
            return true;
        }

        static bool RaySegment(ref Ray2D ray, Vector2 a, Vector2 b, out float t)
        {
            var perp = Vector2.TurnLeft(b - a);
            var dot = Vector2.Dot(ray.Normal, perp);
            if (Calc.Approx(dot, 0f))
            {
                t = float.MaxValue;
                return false;
            }
            a -= ray.Point;
            t = Vector2.Dot(perp, a) / dot;
            var s = Vector2.Dot(Vector2.TurnLeft(ray.Normal), a) / dot;
            return t >= 0.0f && s >= 0.0f && s <= 1.0f;
        }

        public static void Project(ref Circle circle, Vector2 axis, out float min, out float max)
        {
            min = Vector2.Dot(circle.Center - axis * circle.Radius, axis);
            max = Vector2.Dot(circle.Center + axis * circle.Radius, axis);
        }
        public static Vector2 Project(ref Circle circle, Vector2 point)
        {
            if (point.Equals(circle.Center))
                return point;
            return circle.Center + Vector2.Normalize(point - circle.Center) * circle.Radius;
        }

        public static void Project(ref Rectangle rect, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            float dot = Vector2.Dot(rect.TopLeft, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(rect.TopRight, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(rect.BottomLeft, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(rect.BottomRight, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }
        public static Vector2 Project(ref Rectangle rect, Vector2 point)
        {
            var a = Vector2.Project(point, rect.TopLeft, rect.TopRight);
            var b = Vector2.Project(point, rect.TopRight, rect.BottomRight);
            var c = Vector2.Project(point, rect.BottomRight, rect.BottomLeft);
            var d = Vector2.Project(point, rect.BottomLeft, rect.TopLeft);
            var r = a;
            float dmin = Vector2.DistanceSquared(a, point);
            float dist = Vector2.DistanceSquared(b, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = b;
            }
            dist = Vector2.DistanceSquared(c, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = c;
            }
            dist = Vector2.DistanceSquared(d, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = d;
            }
            return r;
        }

        public static void Project(ref Triangle tri, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            float dot = Vector2.Dot(tri.A, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(tri.B, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(tri.C, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }
        public static Vector2 Project(ref Triangle tri, Vector2 point)
        {
            var a = Vector2.Project(point, tri.A, tri.B);
            var b = Vector2.Project(point, tri.B, tri.C);
            var c = Vector2.Project(point, tri.C, tri.A);
            var r = a;
            float dmin = Vector2.DistanceSquared(a, point);
            float dist = Vector2.DistanceSquared(b, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = b;
            }
            dist = Vector2.DistanceSquared(c, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = c;
            }
            return r;
        }

        public static void Project(ref Quad quad, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            float dot = Vector2.Dot(quad.A, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(quad.B, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(quad.C, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
            dot = Vector2.Dot(quad.D, axis);
            min = Math.Min(dot, min);
            max = Math.Max(dot, max);
        }
        public static Vector2 Project(ref Quad quad, Vector2 point)
        {
            var a = Vector2.Project(point, quad.A, quad.B);
            var b = Vector2.Project(point, quad.B, quad.C);
            var c = Vector2.Project(point, quad.C, quad.D);
            var d = Vector2.Project(point, quad.D, quad.A);
            var r = a;
            float dmin = Vector2.DistanceSquared(a, point);
            float dist = Vector2.DistanceSquared(b, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = b;
            }
            dist = Vector2.DistanceSquared(c, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = c;
            }
            dist = Vector2.DistanceSquared(d, point);
            if (dist < dmin)
            {
                dmin = dist;
                r = d;
            }
            return r;
        }

        public static void Project(Polygon poly, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;
            var points = poly.points;
            for (int i = 0; i < points.Count; ++i)
            {
                float dot = Vector2.Dot(points[i], axis);
                min = Math.Min(dot, min);
                max = Math.Max(dot, max);
            }
        }
        public static Vector2 Project(Polygon poly, Vector2 point)
        {
            float dist = float.MaxValue;
            Vector2 result = point;
            var points = poly.points;
            for (int i = 0; i < points.Count; ++i)
            {
                var p2 = Vector2.Project(point, points[i], points[(i + 1) % points.Count]);
                float d = Vector2.DistanceSquared(point, p2);
                if (d < dist)
                {
                    dist = d;
                    result = p2;
                }
            }
            return result;
        }

        public static bool Raycast(ref Circle circle, ref Ray2D ray)
        {
            float dist;
            return Raycast(ref circle, ref ray, out dist);
        }
        public static bool Raycast(ref Circle circle, ref Ray2D ray, out RayHit2D hit)
        {
            if (Raycast(ref circle, ref ray, out hit.Distance))
            {
                hit.Normal = Vector2.Normalize(ray.GetPoint(hit.Distance) - circle.Center);
                return true;
            }
            hit.Normal = Vector2.Zero;
            return false;
        }
        public static bool Raycast(ref Circle circle, ref Ray2D ray, out float dist)
        {
            dist = 0f;

            var diff = circle.Center - ray.Point;
            var sqrDist = diff.LengthSquared;
            var radius = circle.Radius * circle.Radius;

            if (sqrDist < radius)
                return false;

            float distanceAlongRay = Vector2.Dot(ray.Normal, diff);
            if (distanceAlongRay < 0f)
                return false;

            dist = radius + distanceAlongRay * distanceAlongRay - sqrDist;
            if (dist < 0f)
                return false;

            dist = distanceAlongRay - (float)Math.Sqrt(dist);
            return true;
        }

        public static bool Raycast(ref Rectangle rect, ref Ray2D ray)
        {
            float d;
            return RaySegment(ref ray, rect.TopLeft, rect.TopRight, out d)
                || RaySegment(ref ray, rect.TopRight, rect.BottomRight, out d)
                || RaySegment(ref ray, rect.BottomRight, rect.BottomLeft, out d)
                || RaySegment(ref ray, rect.BottomLeft, rect.TopLeft, out d);
        }
        public static bool Raycast(ref Rectangle rect, ref Ray2D ray, out RayHit2D hit)
        {
            hit.Distance = float.MaxValue;
            hit.Normal = Vector2.Zero;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, rect.TopLeft, rect.TopRight, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = rect.TopRight - rect.TopLeft;
                }
            }
            if (RaySegment(ref ray, rect.TopRight, rect.BottomRight, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = rect.BottomRight - rect.TopRight;
                }
            }
            if (RaySegment(ref ray, rect.BottomRight, rect.BottomLeft, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = rect.BottomLeft - rect.BottomRight;
                }
            }
            if (RaySegment(ref ray, rect.BottomLeft, rect.TopLeft, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = rect.TopLeft - rect.BottomLeft;
                }
            }
            if (crossings > 0 && (crossings % 2) == 0)
            {
                hit.Normal = Vector2.TurnLeft(Vector2.Normalize(hit.Normal));
                return true;
            }
            return false;
        }
        public static bool Raycast(ref Rectangle rect, ref Ray2D ray, out float dist)
        {
            dist = float.MaxValue;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, rect.TopLeft, rect.TopRight, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, rect.TopRight, rect.BottomRight, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, rect.BottomRight, rect.BottomLeft, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, rect.BottomLeft, rect.TopLeft, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            return crossings > 0 && (crossings % 2) == 0;
        }

        public static bool Raycast(ref Triangle tri, ref Ray2D ray)
        {
            float d;
            return RaySegment(ref ray, tri.A, tri.B, out d)
                || RaySegment(ref ray, tri.B, tri.C, out d)
                || RaySegment(ref ray, tri.C, tri.A, out d);
        }
        public static bool Raycast(ref Triangle tri, ref Ray2D ray, out RayHit2D hit)
        {
            hit.Distance = float.MaxValue;
            hit.Normal = Vector2.Zero;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, tri.A, tri.B, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = tri.B - tri.A;
                }
            }
            if (RaySegment(ref ray, tri.B, tri.C, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = tri.C - tri.B;
                }
            }
            if (RaySegment(ref ray, tri.C, tri.A, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = tri.A - tri.C;
                }
            }
            if (crossings > 0 && (crossings % 2) == 0)
            {
                hit.Normal = Vector2.TurnLeft(Vector2.Normalize(hit.Normal));
                return true;
            }
            return false;
        }
        public static bool Raycast(ref Triangle tri, ref Ray2D ray, out float dist)
        {
            dist = float.MaxValue;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, tri.A, tri.B, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, tri.B, tri.C, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, tri.C, tri.A, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            return crossings > 0 && (crossings % 2) == 0;
        }

        public static bool Raycast(ref Quad quad, ref Ray2D ray)
        {
            float d;
            return RaySegment(ref ray, quad.A, quad.B, out d)
                || RaySegment(ref ray, quad.B, quad.C, out d)
                || RaySegment(ref ray, quad.C, quad.D, out d)
                || RaySegment(ref ray, quad.D, quad.A, out d);
        }
        public static bool Raycast(ref Quad quad, ref Ray2D ray, out RayHit2D hit)
        {
            hit.Distance = float.MaxValue;
            hit.Normal = Vector2.Zero;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, quad.A, quad.B, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = quad.B - quad.A;
                }
            }
            if (RaySegment(ref ray, quad.B, quad.C, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = quad.C - quad.B;
                }
            }
            if (RaySegment(ref ray, quad.C, quad.D, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = quad.D - quad.C;
                }
            }
            if (RaySegment(ref ray, quad.D, quad.A, out d))
            {
                ++crossings;
                if (d < hit.Distance)
                {
                    hit.Distance = d;
                    hit.Normal = quad.A - quad.D;
                }
            }
            if (crossings > 0 && (crossings % 2) == 0)
            {
                hit.Normal = Vector2.TurnLeft(Vector2.Normalize(hit.Normal));
                return true;
            }
            return false;
        }
        public static bool Raycast(ref Quad quad, ref Ray2D ray, out float dist)
        {
            dist = float.MaxValue;
            float d;
            int crossings = 0;
            if (RaySegment(ref ray, quad.A, quad.B, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, quad.B, quad.C, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, quad.C, quad.D, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            if (RaySegment(ref ray, quad.D, quad.A, out d))
            {
                ++crossings;
                if (d < dist)
                    dist = d;
            }
            return crossings > 0 && (crossings % 2) == 0;
        }

        public static bool Raycast(Polygon poly, ref Ray2D ray)
        {
            float d;
            var points = poly.points;
            for (int i = 0; i < poly.points.Count; ++i)
                if (RaySegment(ref ray, points[i], points[(i + 1) % points.Count], out d))
                    return true;
            return false;
        }
        public static bool Raycast(Polygon poly, ref Ray2D ray, out RayHit2D hit)
        {
            hit.Distance = float.MaxValue;
            hit.Normal = Vector2.Zero;
            float d;
            int crossings = 0;
            Vector2 a, b;
            var points = poly.points;
            for (int i = 0; i < points.Count; ++i)
            {
                a = points[i];
                b = points[(i + 1) % points.Count];
                if (RaySegment(ref ray, a, b, out d))
                {
                    ++crossings;
                    if (d < hit.Distance)
                    {
                        hit.Distance = d;
                        hit.Normal = b - a;
                    }
                }
            }
            if (crossings > 0 && (crossings % 2) == 0)
            {
                hit.Normal = Vector2.TurnLeft(Vector2.Normalize(hit.Normal));
                return true;
            }
            return false;
        }
        public static bool Raycast(Polygon poly, ref Ray2D ray, out float dist)
        {
            dist = float.MaxValue;
            float d;
            int crossings = 0;
            var points = poly.points;
            for (int i = 0; i < points.Count; ++i)
            {
                var a = points[i];
                var b = points[(i + 1) % points.Count];
                if (RaySegment(ref ray, a, b, out d))
                {
                    ++crossings;
                    if (d < dist)
                        dist = d;
                }
            }
            return crossings > 0 && (crossings % 2) == 0;
        }

        public static bool Contains(ref Circle a, ref Circle b)
        {
            if (b.Radius > a.Radius)
                return false;
            return Vector2.DistanceSquared(a.Center, b.Center) < (a.Radius - b.Radius) * (a.Radius - b.Radius);
        }
        public static bool Contains(ref Circle circle, ref Rectangle rect)
        {
            return circle.Contains(rect.TopLeft)
                && circle.Contains(rect.TopRight)
                && circle.Contains(rect.BottomLeft)
                && circle.Contains(rect.BottomRight);
        }
        public static bool Contains(ref Circle circle, ref Triangle tri)
        {
            return circle.Contains(tri.A)
                && circle.Contains(tri.B)
                && circle.Contains(tri.C);
        }
        public static bool Contains(ref Circle circle, ref Quad quad)
        {
            return circle.Contains(quad.A)
                && circle.Contains(quad.B)
                && circle.Contains(quad.C)
                && circle.Contains(quad.D);
        }
        public static bool Contains(ref Circle circle, Polygon poly)
        {
            for (int i = 0; i < poly.points.Count; ++i)
                if (!circle.Contains(poly.points[i]))
                    return false;
            return true;
        }

        public static bool Contains(ref Rectangle rect, ref Circle circle)
        {
            return circle.Left >= rect.X
                && circle.Right <= rect.Right
                && circle.Top >= rect.Y
                && circle.Bottom <= rect.Bottom;
        }
        public static bool Contains(ref Rectangle a, ref Rectangle b)
        {
            return b.X >= a.X && b.Y >= a.Y && b.Right <= a.Right && b.Bottom <= a.Bottom;
        }
        public static bool Contains(ref Rectangle rect, ref Triangle tri)
        {
            return rect.Contains(tri.A)
                && rect.Contains(tri.B)
                && rect.Contains(tri.C);
        }
        public static bool Contains(ref Rectangle rect, ref Quad quad)
        {
            return rect.Contains(quad.A)
                && rect.Contains(quad.B)
                && rect.Contains(quad.C)
                && rect.Contains(quad.D);
        }
        public static bool Contains(ref Rectangle rect, Polygon poly)
        {
            for (int i = 0; i < poly.points.Count; ++i)
                if (!rect.Contains(poly.points[i]))
                    return false;
            return true;
        }

        public static bool Contains(ref Triangle tri, ref Circle circle)
        {
            if (!tri.Contains(circle.Center))
                return false;
            var p = tri.Project(circle.Center);
            return Vector2.DistanceSquared(circle.Center, p) >= circle.Radius * circle.Radius;
        }
        public static bool Contains(ref Triangle tri, ref Rectangle rect)
        {
            return tri.Contains(rect.TopLeft)
                && tri.Contains(rect.TopRight)
                && tri.Contains(rect.BottomRight)
                && tri.Contains(rect.BottomLeft);
        }
        public static bool Contains(ref Triangle a, ref Triangle b)
        {
            return a.Contains(b.A)
                && a.Contains(b.B)
                && a.Contains(b.C);
        }
        public static bool Contains(ref Triangle tri, ref Quad quad)
        {
            return tri.Contains(quad.A)
                && tri.Contains(quad.B)
                && tri.Contains(quad.C)
                && tri.Contains(quad.D);
        }
        public static bool Contains(ref Triangle tri, Polygon poly)
        {
            for (int i = 0; i < poly.points.Count; ++i)
                if (!tri.Contains(poly.points[i]))
                    return false;
            return true;
        }

        public static bool Contains(ref Quad quad, ref Circle circle)
        {
            if (!quad.Contains(circle.Center))
                return false;
            var p = quad.Project(circle.Center);
            return Vector2.DistanceSquared(circle.Center, p) >= circle.Radius * circle.Radius;
        }
        public static bool Contains(ref Quad quad, ref Rectangle rect)
        {
            return quad.Contains(rect.TopLeft)
                && quad.Contains(rect.TopRight)
                && quad.Contains(rect.BottomRight)
                && quad.Contains(rect.BottomLeft);
        }
        public static bool Contains(ref Quad quad, ref Triangle tri)
        {
            return quad.Contains(tri.A)
                && quad.Contains(tri.B)
                && quad.Contains(tri.C);
        }
        public static bool Contains(ref Quad a, ref Quad b)
        {
            return a.Contains(b.A)
                && a.Contains(b.B)
                && a.Contains(b.C)
                && a.Contains(b.D);
        }
        public static bool Contains(ref Quad quad, Polygon poly)
        {
            for (int i = 0; i < poly.points.Count; ++i)
                if (!quad.Contains(poly.points[i]))
                    return false;
            return true;
        }

        public static bool Contains(Polygon poly, ref Circle circle)
        {
            if (!poly.Contains(circle.Center))
                return false;
            return !circle.Contains(poly.Project(circle.Center));
        }
        public static bool Contains(Polygon poly, ref Rectangle rect)
        {
            if (!poly.Contains(rect.TopLeft))
                return false;
            if (!poly.Contains(rect.TopRight))
                return false;
            if (!poly.Contains(rect.BottomRight))
                return false;
            if (!poly.Contains(rect.BottomLeft))
                return false;
            return true;
        }
        public static bool Contains(Polygon poly, ref Triangle tri)
        {
            if (!poly.Contains(tri.A))
                return false;
            if (!poly.Contains(tri.B))
                return false;
            if (!poly.Contains(tri.C))
                return false;
            return true;
        }
        public static bool Contains(Polygon poly, ref Quad quad)
        {
            if (!poly.Contains(quad.A))
                return false;
            if (!poly.Contains(quad.B))
                return false;
            if (!poly.Contains(quad.C))
                return false;
            if (!poly.Contains(quad.D))
                return false;
            return true;
        }
        public static bool Contains(Polygon a, Polygon b)
        {
            for (int i = 0; i < b.points.Count; ++i)
                if (!a.Contains(b.points[i]))
                    return false;
            return true;
        }

        public static bool Intersects(ref Circle a, ref Circle b)
        {
            return Vector2.DistanceSquared(a.Center, b.Center) < (a.Radius + b.Radius) * (a.Radius + b.Radius);
        }
        public static bool Intersects(ref Circle circle, ref Rectangle rect)
        {
            if (rect.Contains(circle.Center))
                return true;
            if (circle.Center.X + circle.Radius > rect.X)
                return true;
            if (circle.Center.X - circle.Radius < rect.Right)
                return true;
            if (circle.Center.Y + circle.Radius > rect.Y)
                return true;
            if (circle.Center.Y - circle.Radius < rect.Bottom)
                return true;
            return false;
        }
        public static bool Intersects(ref Circle circle, ref Triangle tri)
        {
            return Intersects(ref tri, ref circle);
        }
        public static bool Intersects(ref Circle circle, ref Quad quad)
        {
            return Intersects(ref quad, ref circle);
        }
        public static bool Intersects(ref Circle circle, Polygon poly)
        {
            return Intersects(poly, ref circle);
        }

        public static bool Intersects(ref Rectangle rect, ref Circle circle)
        {
            return Intersects(ref circle, ref rect);
        }
        public static bool Intersects(ref Rectangle a, ref Rectangle b)
        {
            return a.X < b.Right && a.Y < b.Bottom && a.Right > b.X && a.Bottom > b.Y;
        }
        public static bool Intersects(ref Rectangle rect, ref Triangle tri)
        {
            return Intersects(ref tri, ref rect);
        }
        public static bool Intersects(ref Rectangle rect, ref Quad quad)
        {
            return Intersects(ref quad, ref rect);
        }
        public static bool Intersects(ref Rectangle rect, Polygon poly)
        {
            return Intersects(poly, ref rect);
        }

        public static bool Intersects(ref Triangle tri, ref Circle circle)
        {
            if (IsSeparatingAxis(ref tri, ref circle, tri.NormalAB))
                return false;
            if (IsSeparatingAxis(ref tri, ref circle, tri.NormalBC))
                return false;
            if (IsSeparatingAxis(ref tri, ref circle, tri.NormalCA))
                return false;
            if (IsSeparatingAxis(ref tri, ref circle, Vector2.Normalize(circle.Center - tri.A)))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle tri, ref Rectangle rect)
        {
            if (IsSeparatingAxis(ref tri, ref rect, Vector2.Right))
                return false;
            if (IsSeparatingAxis(ref tri, ref rect, Vector2.Down))
                return false;
            if (IsSeparatingAxis(ref tri, ref rect, tri.NormalAB))
                return false;
            if (IsSeparatingAxis(ref tri, ref rect, tri.NormalBC))
                return false;
            if (IsSeparatingAxis(ref tri, ref rect, tri.NormalCA))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle a, ref Triangle b)
        {
            if (IsSeparatingAxis(ref a, ref b, a.NormalAB))
                return false;
            if (IsSeparatingAxis(ref a, ref b, a.NormalBC))
                return false;
            if (IsSeparatingAxis(ref a, ref b, a.NormalCA))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalAB))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalBC))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalCA))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle tri, ref Quad quad)
        {
            return Intersects(ref quad, ref tri);
        }
        public static bool Intersects(ref Triangle tri, Polygon poly)
        {
            return Intersects(poly, ref tri);
        }

        public static bool Intersects(ref Quad quad, ref Circle circle)
        {
            if (IsSeparatingAxis(ref quad, ref circle, quad.NormalAB))
                return false;
            if (IsSeparatingAxis(ref quad, ref circle, quad.NormalBC))
                return false;
            if (IsSeparatingAxis(ref quad, ref circle, quad.NormalCD))
                return false;
            if (IsSeparatingAxis(ref quad, ref circle, quad.NormalDA))
                return false;
            if (IsSeparatingAxis(ref quad, ref circle, circle.Center - quad.A))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, ref Rectangle rect)
        {
            if (IsSeparatingAxis(ref quad, ref rect, Vector2.Right))
                return false;
            if (IsSeparatingAxis(ref quad, ref rect, Vector2.Down))
                return false;
            if (IsSeparatingAxis(ref quad, ref rect, quad.NormalAB))
                return false;
            if (IsSeparatingAxis(ref quad, ref rect, quad.NormalBC))
                return false;
            if (IsSeparatingAxis(ref quad, ref rect, quad.NormalCD))
                return false;
            if (IsSeparatingAxis(ref quad, ref rect, quad.NormalDA))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, ref Triangle tri)
        {
            if (IsSeparatingAxis(ref tri, ref quad, tri.NormalAB))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, tri.NormalBC))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, tri.NormalCA))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, quad.NormalAB))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, quad.NormalBC))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, quad.NormalCD))
                return false;
            if (IsSeparatingAxis(ref tri, ref quad, quad.NormalDA))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad a, ref Quad b)
        {
            if (IsSeparatingAxis(ref a, ref b, a.NormalAB))
                return false;
            if (IsSeparatingAxis(ref a, ref b, a.NormalBC))
                return false;
            if (IsSeparatingAxis(ref a, ref b, a.NormalCD))
                return false;
            if (IsSeparatingAxis(ref a, ref b, a.NormalDA))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalAB))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalBC))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalCD))
                return false;
            if (IsSeparatingAxis(ref a, ref b, b.NormalDA))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, Polygon poly)
        {
            return Intersects(poly, ref quad);
        }

        public static bool Intersects(Polygon poly, ref Circle circle)
        {
            poly.Update();
            for (int i = 0; i < poly.axes.Count; ++i)
                if (IsSeparatingAxis(ref poly, ref circle, poly.axes[i]))
                    return false;
            if (IsSeparatingAxis(ref poly, ref circle, Vector2.Normalize(poly.points[0] - circle.Center)))
                return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Rectangle rect)
        {
            poly.Update();
            if (IsSeparatingAxis(ref poly, ref rect, Vector2.Right))
                return false;
            if (IsSeparatingAxis(ref poly, ref rect, Vector2.Down))
                return false;
            for (int i = 0; i < poly.axes.Count; ++i)
                if (IsSeparatingAxis(ref poly, ref rect, poly.axes[i]))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Triangle tri)
        {
            poly.Update();
            if (IsSeparatingAxis(ref poly, ref tri, tri.NormalAB))
                return false;
            if (IsSeparatingAxis(ref poly, ref tri, tri.NormalBC))
                return false;
            if (IsSeparatingAxis(ref poly, ref tri, tri.NormalCA))
                return false;
            for (int i = 0; i < poly.axes.Count; ++i)
                if (IsSeparatingAxis(ref poly, ref tri, poly.axes[i]))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Quad quad)
        {
            poly.Update();
            if (IsSeparatingAxis(ref poly, ref quad, quad.NormalAB))
                return false;
            if (IsSeparatingAxis(ref poly, ref quad, quad.NormalBC))
                return false;
            if (IsSeparatingAxis(ref poly, ref quad, quad.NormalCD))
                return false;
            if (IsSeparatingAxis(ref poly, ref quad, quad.NormalDA))
                return false;
            for (int i = 0; i < poly.axes.Count; ++i)
                if (IsSeparatingAxis(ref poly, ref quad, poly.axes[i]))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon a, Polygon b)
        {
            a.Update();
            b.Update();
            for (int i = 0; i < a.axes.Count; ++i)
                if (IsSeparatingAxis(ref a, ref b, a.axes[i]))
                    return false;
            for (int i = 0; i < b.axes.Count; ++i)
                if (IsSeparatingAxis(ref a, ref b, b.axes[i]))
                    return false;
            return true;
        }

        public static bool Intersects<T>(Vector2 point, ref T shape, out Vector2 pushOut) where T : IShape
        {
            if (!shape.Contains(point))
            {
                pushOut = Vector2.Zero;
                return false;
            }
            pushOut = shape.Project(point) - point;
            return true;
        }

        public static bool Intersects(ref Circle circle, Vector2 point, out Vector2 result)
        {
            if (!Intersects(point, ref circle, out result))
                return false;
            result = -result;
            return true;
        }
        public static bool Intersects(ref Circle a, ref Circle b, out Vector2 pushOut)
        {
            var offset = a.Center - b.Center;
            var dist = offset.LengthSquared;
            var rad = (a.Radius + b.Radius) * (a.Radius + b.Radius);
            if (dist < rad)
            {
                pushOut = offset.Normalized * (float)Math.Sqrt(dist - rad);
                return true;
            }
            pushOut = Vector2.Zero;
            return false;
        }
        public static bool Intersects(ref Circle circle, ref Rectangle rect, out Vector2 pushOut)
        {
            if (!Intersects(ref rect, ref circle, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Circle circle, ref Triangle tri, out Vector2 pushOut)
        {
            if (!Intersects(ref tri, ref circle, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Circle circle, ref Quad quad, out Vector2 pushOut)
        {
            if (!Intersects(ref quad, ref circle, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Circle circle, Polygon poly, out Vector2 pushOut)
        {
            if (!Intersects(poly, ref circle, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }

        public static bool Intersects(ref Rectangle rect, Vector2 point, out Vector2 pushOut)
        {
            if (!Intersects(point, ref rect, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Rectangle rect, ref Circle circle, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref rect, ref circle, Vector2.Right, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref rect, ref circle, Vector2.Down, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref rect, ref circle, Vector2.Normalize(rect.TopLeft - circle.Center), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref rect, ref circle, Vector2.Normalize(rect.TopRight - circle.Center), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref rect, ref circle, Vector2.Normalize(rect.BottomRight - circle.Center), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref rect, ref circle, Vector2.Normalize(rect.BottomLeft - circle.Center), ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Rectangle a, ref Rectangle b, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref a, ref b, Vector2.Right, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, Vector2.Down, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Rectangle rect, ref Triangle tri, out Vector2 pushOut)
        {
            if (!Intersects(ref tri, ref rect, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Rectangle rect, ref Quad quad, out Vector2 pushOut)
        {
            if (!Intersects(ref quad, ref rect, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Rectangle rect, Polygon poly, out Vector2 pushOut)
        {
            if (!Intersects(poly, ref rect, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }

        public static bool Intersects(ref Triangle tri, Vector2 point, out Vector2 pushOut)
        {
            if (!Intersects(point, ref tri, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Triangle tri, ref Circle circle, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref tri, ref circle, tri.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref circle, tri.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref circle, tri.NormalCA, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref circle, Vector2.Normalize(circle.Center - tri.A), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref circle, Vector2.Normalize(circle.Center - tri.B), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref circle, Vector2.Normalize(circle.Center - tri.C), ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle tri, ref Rectangle rect, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref tri, ref rect, Vector2.Right, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref rect, Vector2.Down, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref rect, tri.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref rect, tri.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref tri, ref rect, tri.NormalCA, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle a, ref Triangle b, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref a, ref b, a.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, a.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, a.NormalCA, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalCA, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Triangle tri, ref Quad quad, out Vector2 pushOut)
        {
            if (!Intersects(ref quad, ref tri, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Triangle tri, Polygon poly, out Vector2 pushOut)
        {
            if (!Intersects(poly, ref tri, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }

        public static bool Intersects(ref Quad quad, Vector2 point, out Vector2 pushOut)
        {
            if (!Intersects(point, ref quad, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(ref Quad quad, ref Circle circle, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref quad, ref circle, quad.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, quad.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, quad.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, quad.NormalDA, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, Vector2.Normalize(circle.Center - quad.A), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, Vector2.Normalize(circle.Center - quad.B), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, Vector2.Normalize(circle.Center - quad.C), ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref circle, Vector2.Normalize(circle.Center - quad.D), ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, ref Rectangle rect, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref quad, ref rect, Vector2.Right, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref rect, Vector2.Down, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref rect, quad.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref rect, quad.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref rect, quad.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref rect, quad.NormalDA, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, ref Triangle tri, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref quad, ref tri, quad.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, quad.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, quad.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, quad.NormalDA, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, tri.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, tri.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref quad, ref tri, tri.NormalCA, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad a, ref Quad b, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref a, ref b, a.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, a.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, a.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, a.NormalDA, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref a, ref b, b.NormalDA, ref min, ref pushOut))
                return false;
            return true;
        }
        public static bool Intersects(ref Quad quad, Polygon poly, out Vector2 pushOut)
        {
            if (!Intersects(poly, ref quad, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }

        public static bool Intersects(Polygon poly, Vector2 point, out Vector2 pushOut)
        {
            if (!Intersects(point, ref poly, out pushOut))
                return false;
            pushOut = -pushOut;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Circle circle, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            poly.Update();
            for (int i = 0; i < poly.axes.Count; ++i)
                if (!GetPushOut(ref poly, ref circle, poly.axes[i], ref min, ref pushOut))
                    return false;
            for (int i = 0; i < poly.points.Count; ++i)
                if (!GetPushOut(ref poly, ref circle, Vector2.Normalize(poly.points[i] - circle.Center), ref min, ref pushOut))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Rectangle rect, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref poly, ref rect, Vector2.Right, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref rect, Vector2.Down, ref min, ref pushOut))
                return false;
            poly.Update();
            for (int i = 0; i < poly.axes.Count; ++i)
                if (!GetPushOut(ref poly, ref rect, poly.axes[i], ref min, ref pushOut))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Triangle tri, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref poly, ref tri, tri.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref tri, tri.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref tri, tri.NormalCA, ref min, ref pushOut))
                return false;
            poly.Update();
            for (int i = 0; i < poly.axes.Count; ++i)
                if (!GetPushOut(ref poly, ref tri, poly.axes[i], ref min, ref pushOut))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon poly, ref Quad quad, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            if (!GetPushOut(ref poly, ref quad, quad.NormalAB, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref quad, quad.NormalBC, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref quad, quad.NormalCD, ref min, ref pushOut))
                return false;
            if (!GetPushOut(ref poly, ref quad, quad.NormalDA, ref min, ref pushOut))
                return false;
            poly.Update();
            for (int i = 0; i < poly.axes.Count; ++i)
                if (!GetPushOut(ref poly, ref quad, poly.axes[i], ref min, ref pushOut))
                    return false;
            return true;
        }
        public static bool Intersects(Polygon a, Polygon b, out Vector2 pushOut)
        {
            pushOut = Vector2.Zero;
            float min = float.MaxValue;
            a.Update();
            for (int i = 0; i < a.axes.Count; ++i)
                if (!GetPushOut(ref a, ref b, a.axes[i], ref min, ref pushOut))
                    return false;
            b.Update();
            for (int i = 0; i < b.axes.Count; ++i)
                if (!GetPushOut(ref a, ref b, b.axes[i], ref min, ref pushOut))
                    return false;
            return true;
        }

        public static float Distance<T>(Vector2 point, ref T shape) where T : IShape
        {
            if (shape.Contains(point))
                return 0f;
            return Vector2.Distance(point, shape.Project(point));
        }

        public static float Distance(ref Circle circle, Vector2 point)
        {
            return Distance(point, ref circle);
        }
        public static float Distance(ref Circle a, ref Circle b)
        {
            return Math.Max(0f, Vector2.Distance(a.Center, b.Center) - (a.Radius + b.Radius));
        }
        public static float Distance(ref Circle circle, ref Rectangle rect)
        {
            var quad = new Quad(ref rect);
            return Distance(ref circle, ref quad);
        }
        public static float Distance(ref Circle circle, ref Triangle tri)
        {
            if (Intersects(ref tri, ref circle))
                return 0f;
            var p = tri.Project(circle.Center);
            return Math.Max(0f, Vector2.Distance(circle.Center, p) - circle.Radius);
        }
        public static float Distance(ref Circle circle, ref Quad quad)
        {
            if (Intersects(ref quad, ref circle))
                return 0f;
            var p = quad.Project(circle.Center);
            return Math.Max(0f, Vector2.Distance(circle.Center, p) - circle.Radius);
        }
        public static float Distance(ref Circle circle, Polygon poly)
        {
            if (Intersects(poly, ref circle))
                return 0f;
            var p = poly.Project(circle.Center);
            return Math.Max(0f, Vector2.Distance(circle.Center, p) - circle.Radius);
        }

        public static float Distance(ref Rectangle rect, Vector2 point)
        {
            return Distance(point, ref rect);
        }
        public static float Distance(ref Rectangle rect, ref Circle circle)
        {
            var quad = new Quad(ref rect);
            return Distance(ref circle, ref quad);
        }
        public static float Distance(ref Rectangle a, ref Rectangle b)
        {
            float dist = float.MaxValue;
            dist = Math.Min(dist, Vector2.DistanceSquared(a.TopLeft, b.Project(a.TopLeft)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.TopRight, b.Project(a.TopRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.BottomRight, b.Project(a.BottomRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.BottomLeft, b.Project(a.BottomLeft)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.TopLeft, a.Project(b.TopLeft)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.TopRight, a.Project(b.TopRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.BottomRight, a.Project(b.BottomRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.BottomLeft, a.Project(b.BottomLeft)));
            return (float)Math.Sqrt(dist);
        }
        public static float Distance(ref Rectangle rect, ref Triangle tri)
        {
            float dist = float.MaxValue;
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.A, rect.Project(tri.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.B, rect.Project(tri.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.C, rect.Project(tri.C)));
            dist = Math.Min(dist, Vector2.DistanceSquared(rect.TopLeft, tri.Project(rect.TopLeft)));
            dist = Math.Min(dist, Vector2.DistanceSquared(rect.TopRight, tri.Project(rect.TopRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(rect.BottomRight, tri.Project(rect.BottomRight)));
            dist = Math.Min(dist, Vector2.DistanceSquared(rect.BottomLeft, tri.Project(rect.BottomLeft)));
            return (float)Math.Sqrt(dist);
        }
        public static float Distance(ref Rectangle rect, ref Quad quad)
        {
            var quad2 = new Quad(ref rect);
            return Distance(ref quad, ref quad2);
        }
        public static float Distance(ref Rectangle rect, Polygon poly)
        {
            return Distance(poly, ref rect);
        }

        public static float Distance(ref Triangle tri, Vector2 point)
        {
            return Distance(point, ref tri);
        }
        public static float Distance(ref Triangle tri, ref Circle circle)
        {
            return Distance(ref circle, ref tri);
        }
        public static float Distance(ref Triangle tri, ref Rectangle rect)
        {
            var quad2 = new Quad(ref rect);
            return Distance(ref tri, ref quad2);
        }
        public static float Distance(ref Triangle a, ref Triangle b)
        {
            float dist = float.MaxValue;
            dist = Math.Min(dist, Vector2.DistanceSquared(a.A, b.Project(a.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.B, b.Project(a.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.C, b.Project(a.C)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.A, a.Project(b.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.B, a.Project(b.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.C, a.Project(b.C)));
            return (float)Math.Sqrt(dist);
        }
        public static float Distance(ref Triangle tri, ref Quad quad)
        {
            return Distance(ref quad, ref tri);
        }
        public static float Distance(ref Triangle quad, Polygon poly)
        {
            return Distance(poly, ref quad);
        }

        public static float Distance(ref Quad quad, Vector2 point)
        {
            return Distance(point, ref quad);
        }
        public static float Distance(ref Quad quad, ref Circle circle)
        {
            return Distance(ref circle, ref quad);
        }
        public static float Distance(ref Quad quad, ref Rectangle rect)
        {
            var quad2 = new Quad(ref rect);
            return Distance(ref quad, ref quad2);
        }
        public static float Distance(ref Quad quad, ref Triangle tri)
        {
            float dist = float.MaxValue;
            dist = Math.Min(dist, Vector2.DistanceSquared(quad.A, tri.Project(quad.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(quad.B, tri.Project(quad.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(quad.C, tri.Project(quad.C)));
            dist = Math.Min(dist, Vector2.DistanceSquared(quad.D, tri.Project(quad.D)));
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.A, quad.Project(tri.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.B, quad.Project(tri.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(tri.C, quad.Project(tri.C)));
            return (float)Math.Sqrt(dist);
        }
        public static float Distance(ref Quad a, ref Quad b)
        {
            float dist = float.MaxValue;
            dist = Math.Min(dist, Vector2.DistanceSquared(a.A, b.Project(a.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.B, b.Project(a.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.C, b.Project(a.C)));
            dist = Math.Min(dist, Vector2.DistanceSquared(a.D, b.Project(a.D)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.A, a.Project(b.A)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.B, a.Project(b.B)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.C, a.Project(b.C)));
            dist = Math.Min(dist, Vector2.DistanceSquared(b.D, a.Project(b.D)));
            return (float)Math.Sqrt(dist);
        }
        public static float Distance(ref Quad quad, Polygon poly)
        {
            return Distance(poly, ref quad);
        }

        public static float Distance(Polygon poly, Vector2 point)
        {
            return Distance(point, ref poly);
        }
        public static float Distance(Polygon poly, ref Circle circle)
        {
            var dist = Distance(circle.Center, ref poly);
            return Math.Max(0f, dist - circle.Radius);
        }
        public static float Distance(Polygon poly, ref Rectangle rect)
        {
            float min = float.MaxValue;
            for (int i = 0; i < poly.points.Count; ++i)
                min = Math.Min(min, Vector2.DistanceSquared(poly.points[i], rect.Project(poly.points[i])));
            min = Math.Min(min, Vector2.DistanceSquared(rect.TopLeft, poly.Project(rect.TopLeft)));
            min = Math.Min(min, Vector2.DistanceSquared(rect.TopRight, poly.Project(rect.TopRight)));
            min = Math.Min(min, Vector2.DistanceSquared(rect.BottomRight, poly.Project(rect.BottomRight)));
            min = Math.Min(min, Vector2.DistanceSquared(rect.BottomLeft, poly.Project(rect.BottomLeft)));
            return (float)Math.Sqrt(min);
        }
        public static float Distance(Polygon poly, ref Triangle tri)
        {
            float min = float.MaxValue;
            for (int i = 0; i < poly.points.Count; ++i)
                min = Math.Min(min, Vector2.DistanceSquared(poly.points[i], tri.Project(poly.points[i])));
            min = Math.Min(min, Vector2.DistanceSquared(tri.A, poly.Project(tri.A)));
            min = Math.Min(min, Vector2.DistanceSquared(tri.B, poly.Project(tri.B)));
            min = Math.Min(min, Vector2.DistanceSquared(tri.C, poly.Project(tri.C)));
            return (float)Math.Sqrt(min);
        }
        public static float Distance(Polygon poly, ref Quad quad)
        {
            float min = float.MaxValue;
            for (int i = 0; i < poly.points.Count; ++i)
                min = Math.Min(min, Vector2.DistanceSquared(poly.points[i], quad.Project(poly.points[i])));
            min = Math.Min(min, Vector2.DistanceSquared(quad.A, poly.Project(quad.A)));
            min = Math.Min(min, Vector2.DistanceSquared(quad.B, poly.Project(quad.B)));
            min = Math.Min(min, Vector2.DistanceSquared(quad.C, poly.Project(quad.C)));
            min = Math.Min(min, Vector2.DistanceSquared(quad.D, poly.Project(quad.D)));
            return (float)Math.Sqrt(min);
        }
        public static float Distance(Polygon a, Polygon b)
        {
            float min = float.MaxValue;
            for (int i = 0; i < a.points.Count; ++i)
                min = Math.Min(min, Vector2.DistanceSquared(a.points[i], b.Project(a.points[i])));
            for (int i = 0; i < b.points.Count; ++i)
                min = Math.Min(min, Vector2.DistanceSquared(b.points[i], a.Project(b.points[i])));
            return (float)Math.Sqrt(min);
        }
    }
}