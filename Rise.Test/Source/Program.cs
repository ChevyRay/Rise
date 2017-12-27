using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        static DrawCall draw;

        static void Init()
        {       
            int screenW = Screen.DrawWidth;
            int screenH = Screen.DrawHeight;
            Texture.DefaultMinFilter = TextureFilter.Linear;
            Texture.DefaultMagFilter = TextureFilter.Nearest;

            var shader = Shader.FromFile("Assets/basic_3d.glsl");
            var texture = new Texture("Assets/pink_square.png", true);

            var model = Matrix4x4.CreateRotationX(-90f * Calc.Rad);
            var view = Matrix4x4.CreateLookAt(new Vector3(20f, 20f, 20f), Vector2.Zero, Vector3.Up);
            var projection = Matrix4x4.CreatePerspectiveFOV(45f * Calc.Rad, (float)screenW / screenH, 0.1f, 100000f);
            //var projection = Matrix4x4.CreateOrthographic(screenW, screenH, 0.1f, 100000f);
            var mvp = model * view * projection;

            draw.Material = new Material(shader);
            draw.Material.SetMatrix4x4("g_ModelViewProjection", ref mvp);
            draw.Material.SetMatrix4x4("g_Model", ref model);
            draw.Material.SetTexture("g_Texture", texture);
            draw.Material.SetColor("g_AmbientColor", Color.White);

            //draw.Mesh = Mesh3D.CreateQuad(10f, 10f, Color.White);
            draw.Mesh = Mesh3D.CreateCube(10f, Color.White);
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            draw.Perform(0x242029ff);
        }
    }
}
