using System;
using System.Collections.Generic;
namespace Rise.TriangulationTest
{
    public class BitmapTriangulator
    {
        List<Point2> edge = new List<Point2>();
        List<Point2> hull = new List<Point2>();

        public BitmapTriangulator()
        {
            
        }

        public bool Triangulate(Bitmap bitmap, float angleThreshold, float extend, Polygon result)
        {
            edge.Clear();
            hull.Clear();
            result.Clear();

            var left = new Point2(int.MaxValue, 0);

            //Get all pixels that are next to a transparent pixel
            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    if (bitmap.IsPixelTransparent(x, y) &&
                        (bitmap.IsPixelVisible(x - 1, y) ||
                         bitmap.IsPixelVisible(x + 1, y) ||
                         bitmap.IsPixelVisible(x, y + 1) ||
                         bitmap.IsPixelVisible(x, y - 1)))
                    {
                        edge.Add(new Point2(x, y));
                        if (x < left.X)
                            left = new Point2(x, y);
                    }
                }
            }

            //If somehow we ended up w/ less than a triangle, just bail
            if (edge.Count < 3)
                return false;

            //Find the convex hull (gift-wrapping algorithm... faster algorithms exist if we want to optimize)
            Point2 a, b, c;
            a = left;
            do
            {
                hull.Add(a);
                b = edge[0];
                for (int i = 1; i < edge.Count; i++)
                {
                    c = edge[i];
                    int side = (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
                    if (a == b || side > 0)
                        b = c;
                }
                a = b;
            }
            while (b != hull[0]);

            //Extend the hull
            if (extend != 0f)
            {
                var centroid = Vector2.Zero;
                int signedArea = 0;
                for (int i = 0; i < hull.Count; ++i)
                {
                    a = hull[i];
                    b = hull[(i + 1) % hull.Count];
                    var area = a.X * b.Y - b.X * a.Y;
                    signedArea += area;
                    centroid += (a + b) * area;
                }
                centroid /= signedArea * 3f;

                //Move all points out from the centroid
                for (int i = 0; i < hull.Count; ++i)
                {
                    a = (Point2)(hull[i] + Vector2.Normalize(hull[i] - centroid, extend));

                    //This breaks everything :(
                    //a.X = Calc.Clamp(a.X, 0, bitmap.Width);
                    //a.Y = Calc.Clamp(a.Y, 0, bitmap.Height);

                    hull[i] = a;
                }
            }

            //Remove colinear points in the hull
            int prevCount = 0;
            while (prevCount != hull.Count)
            {
                prevCount = hull.Count;
                for (int i = 0; i < hull.Count; ++i)
                {
                    a = hull[i];
                    b = hull[(i + 1) % hull.Count];
                    c = hull[(i + 2) % hull.Count];
                    Vector2 ab = b - a;
                    Vector2 cb = c - b;
                    if (Math.Abs(ab.Angle - cb.Angle) <= angleThreshold)
                        hull.RemoveAt(i);
                }
            }

            //Build polygon
            for (int i = 0; i < hull.Count; ++i)
                result.AddPoint(hull[i]);

            return true;
        }
    }
}
