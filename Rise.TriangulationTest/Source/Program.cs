using System;
using System.Collections.Generic;
using Rise;
namespace Rise.TriangulationTest
{
    static class Program
    {
        static DrawBatch2D batch;
        static Polygon poly;
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

            poly = new Polygon();
            var tri = new BitmapTriangulator();
            tri.Triangulate(bitmap, 0.1f * Calc.Rad, 4f, poly);

            Console.WriteLine("Time: {0} ms", Time.ClockMilliseconds - time);
            Console.WriteLine("Points: {0}", poly.PointCount);

            tris = new List<Triangle>();
            var list = new List<Vector2>(poly.PointCount);
            for (int i = 0; i < poly.PointCount; ++i)
                list.Add(poly.GetPoint(i));
            while (list.Count > 2)
            {
                for (int i = 0; i < list.Count - 2; ++i)
                {
                    var a = list[i];
                    var b = list[(i + 1) % list.Count];
                    var c = list[(i + 2) % list.Count];
                    float side = (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
                    if (side < 0f)
                    {
                        tris.Add(new Triangle(a, b, c));
                        list.RemoveAt(i + 1);
                    }
                }
            }

            Console.WriteLine("Triangles: {0}", tris.Count);

            Console.WriteLine("Bitmap Area: {0}", bitmap.Width * bitmap.Height);
            Console.WriteLine("Convex Area: {0}", poly.GetArea());
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            var m = Matrix4x4.CreateTranslation(new Vector2(200f, 100f)) * Matrix4x4.CreateScale(Screen.PixelSize, Screen.PixelSize);

            batch.Begin(null, null, m, BlendMode.Premultiplied);

            batch.DrawTexture(texture, Vector2.Zero, Color4.White);

            foreach (var tri in tris)
            {
                batch.DrawLine(tri.A, tri.B, 1f, Color4.Green);
                batch.DrawLine(tri.B, tri.C, 1f, Color4.Green);
                batch.DrawLine(tri.C, tri.A, 1f, Color4.Green);
            }

            for (int i = 0; i < poly.PointCount; ++i)
            {
                Vector2 a = poly.GetPoint(i);
                Vector2 b = poly.GetPoint((i + 1) % poly.PointCount);
                batch.DrawRect(Rectangle.Box(a, 4f), Color4.White);
            }

            //batch.DrawRect(Rectangle.Box(poly.Centroid, 8f), Color4.Red);

            batch.End();
        }
    }
}
