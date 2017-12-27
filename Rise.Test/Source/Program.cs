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

        static DrawCall drawToCanvas;
        static DrawCall drawToScreen;

        static void Init()
        {       
            const int screenW = 320;
            const int screenH = 180;

            Texture.DefaultMinFilter = TextureFilter.Linear;
            Texture.DefaultMagFilter = TextureFilter.Nearest;
            var face = new Texture("Assets/face.png", true);

            var shader = Shader.FromFile("Assets/basic_shader.glsl");
            var canvasTexture = new Texture(screenW, screenH, null);

            drawToCanvas.SetBlendMode(BlendMode.Alpha);
            drawToCanvas.Target = new RenderTarget(canvasTexture);
            drawToCanvas.Material = new Material(shader);
            drawToCanvas.Material.SetMatrix4x4("g_Matrix", Matrix4x4.Orthographic(screenW, screenH, 0f, 1f));
            drawToCanvas.Material.SetTexture("g_Texture", face);
            drawToCanvas.Mesh = Mesh2D.CreateRect(new Rectangle(face.Width, face.Height));

            drawToScreen.SetBlendMode(BlendMode.Alpha);
            drawToScreen.Material = new Material(shader);
            drawToScreen.Material.SetMatrix4x4("g_Matrix", Matrix4x4.Orthographic(screenW, screenH, 0f, 1f));
            drawToScreen.Material.SetTexture("g_Texture", canvasTexture);
            drawToScreen.Mesh = Mesh2D.CreateRect(new Rectangle(screenW, screenH), new Vector2(0f, 1f), new Vector2(1f, 0f));
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            drawToCanvas.Perform(Color.Transparent);
            drawToScreen.Perform(Color.Black);
        }
    }
}
