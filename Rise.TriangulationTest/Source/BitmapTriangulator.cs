using System;
using System.Collections.Generic;
namespace Rise.TriangulationTest
{
    public class BitmapTriangulator
    {
        public List<Point2> edge = new List<Point2>();
        public List<Point2> hull = new List<Point2>();

        public BitmapTriangulator()
        {
            
        }

        public bool Triangulate(Bitmap bitmap)
        {
            edge.Clear();
            hull.Clear();

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

            //Remove colinear points in the hull
            for (int i = 0; i < hull.Count; ++i)
            {
                b = hull[(i + 1) % hull.Count];
                if (b - hull[i] == hull[(i + 2) % hull.Count] - b)
                    hull.RemoveAt(i--);
            }

            return true;
        }
    }
}
