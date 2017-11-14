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

        static void Init()
        {
            shader = Shader.FromFile("Assets/basic_shader.glsl");
            material = new Material(shader);

            Texture.DefaultFilter = TextureFilter.Nearest;
            face = new Texture("Assets/face.png", true);

            mesh = new Mesh();
            mesh.AddRect(new Rectangle(0f, 0f, face.Width, face.Height), Vector2.Zero, Vector2.One, Color.White, Color.Transparent);
            mesh.Update();

            material.SetMatrix4x4("g_Matrix", Matrix4x4.Orthographic(Screen.Width, Screen.Height, 0f, 1f));
            material.SetTexture("g_Texture", face);
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            Graphics.Clear(0x1e4e50ff);
            Graphics.SetBlendMode(BlendMode.Premultiplied);
            Graphics.Draw(material, mesh);
        }
    }
}
