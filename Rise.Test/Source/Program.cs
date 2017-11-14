using System;
using System.IO;
using System.Diagnostics;
using Rise;
using Rise.Imaging;
using Rise.PlatformSDL;
namespace Rise.Test
{
    static class Program
    {
        public static void Main(string[] args)
        {
            App.Init<PlatformSDL2>();
            App.OnInit += Init;
            App.OnUpdate += Update;
            App.OnRender += Render;
            App.Run("Rise.Test", 640, 360, null);
        }

        static Shader shader;

        static Material material;
        static Texture face;
        static Mesh mesh;

        static Texture screenTexture;
        static Material screenMaterial;
        static RenderTarget screenTarget;
        static Mesh screenMesh;

        static void Init()
        {
            const int screenW = 320;
            const int screenH = 180;

            Texture.DefaultMinFilter = TextureFilter.Linear;
            Texture.DefaultMagFilter = TextureFilter.Nearest;
            face = new Texture("Assets/face.png", true);

            mesh = new Mesh();
            mesh.AddRect(new Rectangle(0f, 0f, face.Width, face.Height), Vector2.Zero, Vector2.One);
            mesh.Update();

            shader = Shader.FromFile("Assets/basic_shader.glsl");

            material = new Material(shader);
            material.SetMatrix4x4("g_Matrix", Matrix4x4.Orthographic(screenW, screenH, 0f, 1f));
            material.SetTexture("g_Texture", face);

            screenTexture = new Texture(screenW, screenH, null);
            screenTarget = new RenderTarget(screenTexture);

            screenMaterial = new Material(shader);
            screenMaterial.SetMatrix4x4("g_Matrix", Matrix4x4.Orthographic(screenW, screenH, 0f, 1f));
            screenMaterial.SetTexture("g_Texture", screenTexture);

            screenMesh = new Mesh();
            screenMesh.AddRect(new Rectangle(0f, 0f, screenW, screenH), Vector2.Zero, Vector2.One);
            screenMesh.Update();
        }

        static void Update()
        {
            
        }

        static bool first = true;

        static void Render()
        {
            Graphics.SetTarget(screenTarget);
            //Graphics.SetTarget(null);
            Graphics.Clear(Color.Transparent);
            Graphics.SetBlendMode(BlendMode.Alpha);
            Graphics.Draw(material, mesh);

            if (first)
            {
                first = false;
                var bits = screenTexture.GetPixels();
                for (int y = 0; y < bits.Height; ++y)
                {
                    for (int x = 0; x < bits.Width; ++x)
                    {
                        Console.Write(bits.GetPixel(x, y).A > 0 ? 'R' : ' ');
                    }
                    Console.WriteLine();
                }
            }

            //Graphics.SetTarget(null);
            //Graphics.Clear(Color.Black);
            //Graphics.SetBlendMode(BlendMode.Alpha);
            //Graphics.Draw(screenMaterial, screenMesh);
        }
    }
}
