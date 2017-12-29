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
        static Matrix4x4 model;
        static Matrix4x4 view;
        static Matrix4x4 proj;
        static float angleX;
        static float angleY;
        static Vector3 cameraPos = new Vector3(0f, 1f, -2f);

        static RenderTarget gBuffer;
        static Texture gDiffuse;
        static Texture gNormal;
        static Texture gPosition;

        static DrawCall lighting;
        static float shininess = 16f;

        static DrawCall toScreen;

        static void Init()
        {       
            int screenW = Screen.DrawWidth;
            int screenH = Screen.DrawHeight;
            const float zMin = 0.1f;
            const float zMax = 10.0f;
            Texture.DefaultMinFilter = TextureFilter.Linear;
            Texture.DefaultMagFilter = TextureFilter.Nearest;

            var shader3D = Shader.FromFile("Assets/basic_3d.glsl");
            var shader2D = Shader.FromFile("Assets/basic_gbuffer.glsl");
            var directionalLightShader = Shader.FromFile("Assets/basic_directional_light.glsl");
            var pinkSquare = new Texture("Assets/pink_square.png", true);

            gDiffuse = new Texture(screenW, screenH, OpenGL.TextureFormat.RGB);
            gNormal = new Texture(screenW, screenH, OpenGL.TextureFormat.RGB16F);
            gPosition = new Texture(screenW, screenH, OpenGL.TextureFormat.RGB32F);
            gBuffer = new RenderTarget(screenW, screenH, gDiffuse, gNormal, gPosition);

            model = Matrix4x4.Identity;
            view = Matrix4x4.CreateLookAt(cameraPos, Vector2.Zero, Vector3.Up);
            proj = Matrix4x4.CreatePerspectiveFOV(60f * Calc.Rad, (float)screenW / screenH, zMin, zMax);
            var mvp = model * view * proj;

            draw.Target = gBuffer;
            draw.Material = new Material(shader3D);
            draw.Material.SetMatrix4x4("g_ModelViewProjection", ref mvp);
            draw.Material.SetMatrix4x4("g_Model", ref model);
            draw.Material.SetTexture("g_Texture", pinkSquare);
            draw.Mesh = Mesh3D.CreateCube(new Vector3(1f, 1f, 1f), Color.White);

            //Directional light
            var lightTexture = new Texture(screenW, screenH, OpenGL.TextureFormat.RGB);
            lighting.Target = new RenderTarget(screenW, screenH, lightTexture);
            lighting.Material = new Material(directionalLightShader);
            lighting.Material.SetMatrix4x4("g_Matrix", Matrix4x4.CreateOrthographic(0f, screenW, 0f, screenH, -1f, 1f));
            lighting.Material.SetTexture("g_Normal", gNormal);
            lighting.Material.SetTexture("g_Position", gPosition);
            lighting.Material.SetVector3("g_CameraPosition", cameraPos);
            lighting.Material.SetColor("g_AmbientColor", Color.Black);
            lighting.Material.SetVector3("g_LightDirection", Vector3.Down);
            lighting.Material.SetColor("g_DiffuseColor", Color.White);
            lighting.Material.SetColor("g_SpecularColor", Color.White);
            lighting.Material.SetFloat("g_Shininess", shininess);
            lighting.Mesh = Mesh2D.CreateRect(new Rectangle(screenW, screenH));

            toScreen.Material = new Material(shader2D);
            toScreen.Material.SetMatrix4x4("g_Matrix", Matrix4x4.CreateOrthographic(0f, Screen.DrawWidth, 0f, Screen.DrawHeight, -1f, 1f));
            toScreen.Material.SetTexture("g_Diffuse", gDiffuse);
            toScreen.Material.SetTexture("g_Lighting", lightTexture);
            toScreen.Mesh = Mesh2D.CreateRect(new Rectangle(Screen.DrawWidth, Screen.DrawHeight));
            toScreen.SetBlendMode(BlendMode.Alpha);
        }

        static void Update()
        {
            if (Keyboard.Down(KeyCode.Right))
                angleY -= Time.Delta;
            if (Keyboard.Down(KeyCode.Left))
                angleY += Time.Delta;
            if (Keyboard.Down(KeyCode.Up))
                angleX += Time.Delta;
            if (Keyboard.Down(KeyCode.Down))
                angleX -= Time.Delta;

            var prev = cameraPos;
            if (Keyboard.Down(KeyCode.Q))
                cameraPos.Z += 2f * Time.Delta;
            if (Keyboard.Down(KeyCode.W))
                cameraPos.Z -= 2f * Time.Delta;

            if (Keyboard.Down(KeyCode.LeftBracket))
                shininess = Math.Max(0f, shininess - 2f * Time.Delta);
            if (Keyboard.Down(KeyCode.RightBracket))
                shininess = Math.Min(100f, shininess + 2f * Time.Delta);

            model = Matrix4x4.CreateRotationX(angleX) * Matrix4x4.CreateRotationY(angleY);
            view = Matrix4x4.CreateLookAt(cameraPos, Vector2.Zero, Vector3.Up);
            var mvp = model * view * proj;
            draw.Material.SetMatrix4x4("g_ModelViewProjection", ref mvp);
            draw.Material.SetMatrix4x4("g_Model", ref model);
            lighting.Material.SetVector3("g_CameraPosition", cameraPos);
            lighting.Material.SetFloat("g_Shininess", shininess);
        }

        static void Render()
        {
            draw.Perform(0x242029ff);
            lighting.Perform(Color.Black);
            toScreen.Perform(Color.Black);
        }
    }
}
