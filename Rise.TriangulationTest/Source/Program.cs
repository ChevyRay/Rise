using System;
using System.Collections.Generic;
using Rise;
namespace Rise.TriangulationTest
{
    static class Program
    {
        static DrawBatch2D batch;
        static List<Point2> points;
        static Texture2D texture;
        static List<Triangle> tris;

        public static void Main(string[] args)
        {
            App.OnInit += Init;
            App.OnUpdate += Update;
            App.OnRender += Render;
            App.Init<PlatformSDL.PlatformSDL2>();
            App.Run("Triangulation Test", 960, 540, null);
        }

        static void Init()
        {
            batch = new DrawBatch2D();

            var bitmap = new Bitmap("Assets/ikenfell.png");
            bitmap.Premultiply();

            texture = new Texture2D(bitmap);

            var time = Time.ClockMilliseconds;

            var tri = new BitmapTriangulator();
            tri.Triangulate(bitmap);

            Console.WriteLine("Time: {0} ms", Time.ClockMilliseconds - time);
            Console.WriteLine("Points: {0}", tri.hull.Count);

            points = tri.hull;

            Console.WriteLine("Bitmap Area: {0}", bitmap.Width * bitmap.Height);
            Console.WriteLine("Convex Area: {0}", GetArea(points));

            tris = new List<Triangle>();
            var list = new List<Point2>(points);
            while (list.Count > 2)
            {
                for (int i = 0; i < list.Count - 2; ++i)
                {
                    var a = list[i];
                    var b = list[(i + 1) % list.Count];
                    var c = list[(i + 2) % list.Count];
                    int side = (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
                    if (side < 0)
                    {
                        tris.Add(new Triangle(a, b, c));
                        list.RemoveAt(i + 1);
                    }
                }
            }

            Console.WriteLine("Triangles: {0}", tris.Count);
        }


        static float GetArea(List<Point2> points)
        {
            int area = 0;
            int j = points.Count - 1;
            for (int i = 0; i < points.Count; ++i)
            {
                area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
                j = i;
            }
            return Math.Abs(area / 2);
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            var m = Matrix4x4.CreateTranslation(new Vector2(10f, 10f));
            batch.Begin(null, null, m, BlendMode.Premultiplied);

            batch.DrawTexture(texture, Vector2.Zero, Color4.White * 0.5f);
            
            /*for (int i = 0; i < points.Count; ++i)
            {
                Vector2 a = points[i];
                Vector2 b = points[(i + 1) % points.Count];
                batch.DrawRect(Rectangle.Box(a + offset, 8f), Color4.White);
                batch.DrawLine(a + offset, b + offset, 2f, Color4.White);
            }*/
            //batch.DrawLine(Vector2.Zero, Mouse.Position, 1f, Color4.White);

            foreach (var tri in tris)
            {
                batch.DrawLine(tri.A, tri.B, 1f, Color4.Green);
                batch.DrawLine(tri.B, tri.C, 1f, Color4.Green);
                batch.DrawLine(tri.C, tri.A, 1f, Color4.Green);
            }

            for (int i = 0; i < points.Count; ++i)
            {
                Vector2 a = points[i];
                Vector2 b = points[(i + 1) % points.Count];
                batch.DrawRect(Rectangle.Box(a, 8f), Color4.White);
            }

            batch.End();
        }
    }
}
