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
            App.Run("Rise.Test", 1024, 576, null);
        }

        static void Init()
        {       
            int screenW = Screen.DrawWidth;
            int screenH = Screen.DrawHeight;
            Texture.DefaultMinFilter = TextureFilter.Nearest;
            Texture.DefaultMagFilter = TextureFilter.Nearest;

            //Load assets
            var shader2D = Shader.FromFile("Assets/basic_2d.glsl");
            var shader3D = Shader.FromFile("Assets/basic_3d_nobuffer.glsl");
            var shaderG = Shader.FromFile("Assets/basic_3d_gbuffer.glsl");
            var shaderDepth = Shader.FromFile("Assets/basic_3d_depth.glsl");
            var shaderPos = Shader.FromFile("Assets/basic_3d_position.glsl");
            var pinkSquare = new Texture("Assets/pink_square.png", true);
            var cubeMesh = Mesh3D.CreateCube(Vector3.One, Color4.White);

            //Create the g-buffer
            var gDepth = new Texture(screenW, screenH, TextureFormat.Depth);
            var gColor = new Texture(screenW, screenH, TextureFormat.RGB);
            var gNormal = new Texture(screenW, screenH, TextureFormat.RGB16F);
            var gZ = new Texture(screenW, screenH, TextureFormat.R);
            var gBuffer = new RenderTarget(screenW, screenH, gDepth, gColor, gNormal, gZ);

            //View
            var scene = new Scene(screenW, screenH);
            var viewRot = false;
            var viewRotPos = Vector2.Zero;

            //Create a model
            var cube = new Model(cubeMesh, pinkSquare);
            cube.Scale = new Vector3(2f, 0.75f, 1f);
            scene.Add(cube);

            //Create a model
            cube = new Model(cubeMesh, pinkSquare);
            cube.Scale = new Vector3(0.75f);
            cube.Position = new Vector3(0f, 0.5f, -2f);
            scene.Add(cube);

            //Draw to buffer
            var draw = new DrawCall(gBuffer, new Material(shaderG));

            //Draw to screen
            int w = screenW / 2;
            int h = screenH / 2;
            var toScreen = new DrawCall(new Material(shaderPos));
            toScreen.Mesh = Mesh2D.CreateRect(new Rectangle(0, 0, w, h));
            toScreen.Material.SetMatrix4x4("Matrix", Matrix4x4.CreateOrthographic(0f, screenW, 0, screenH, -1f, 1f));
            toScreen.Material.SetTexture("ZBuffer", gDepth);

            App.OnUpdate += Update;
            App.OnRender += Render;
            Update();
            void Update()
            {
                //Pivoting
                if (viewRot)
                {
                    var move = Mouse.Position - viewRotPos;
                    if (move != Vector2.Zero)
                    {
                        scene.ViewAngle += move * 6f * Time.Delta;
                        viewRotPos = Mouse.Position;
                    }
                    if (!Mouse.LeftDown)
                        viewRot = false;
                }
                else if (Mouse.LeftPressed)
                {
                    viewRot = true;
                    viewRotPos = Mouse.Position;
                }

                //Zooming
                if (Mouse.ScrollY != 0)
                {
                    scene.ViewDist -= Mouse.ScrollY * 10f * Time.Delta;
                }
            }
            void Render()
            {
                draw.Clear(0x1f171fff);
                foreach (var model in scene.Models)
                {
                    draw.Material.SetMatrix4x4("ModelMatrix", model.Matrix);
                    draw.Material.SetMatrix4x4("ModelViewProjectionMatrix", model.MvpMatrix);
                    draw.Material.SetTexture("Texture", model.Texture);
                    draw.Mesh = model.Mesh;
                    draw.Perform();
                }

                //toScreen.Perform(Color3.Black);
                gBuffer.BlitTextureTo(null, 0, BlitFilter.Linear, new RectangleI(0, h, w, h));
                gBuffer.BlitTextureTo(null, 1, BlitFilter.Linear, new RectangleI(w, h, w, h));
                gBuffer.BlitTextureTo(null, 2, BlitFilter.Linear, new RectangleI(w, 0, w, h));

                toScreen.Material.SetMatrix4x4("ProjMatrix", scene.ProjMatrix);
                toScreen.Material.SetMatrix4x4("ViewMatrix", scene.ViewMatrix);
                toScreen.Perform();
            }
        }
    }

    public class Scene
    {
        Matrix4x4 projMatrix;
        Matrix4x4 viewMatrix;
        Vector2 viewAngle;
        float viewDist = 4f;
        bool dirty = true;
        public List<Model> Models = new List<Model>();

        public Scene(int width, int height)
        {
            projMatrix = Matrix4x4.CreatePerspectiveFOV(70f * Calc.Rad, (float)width / height, 1f, 100f);
        }

        public void Add(Model model)
        {
            model.scene = this;
            model.SetDirty();
            Models.Add(model);
        }

        void SetDirty()
        {
            if (!dirty)
            {
                dirty = true;
                foreach (var model in Models)
                    model.SetDirty();
            }
        }

        void UpdateMatrix()
        {
            if (dirty)
            {
                dirty = false;
                var rot = Quaternion.Euler(viewAngle.Y, 0f, 0f) * Quaternion.Euler(0f, viewAngle.X, 0f);
                viewMatrix = Matrix4x4.CreateRotation(rot) * Matrix4x4.CreateTranslation(0f, 0f, -viewDist);
                foreach (var model in Models)
                    model.SetDirty();
            }
        }

        public Vector2 ViewAngle
        {
            get { return viewAngle; }
            set
            {
                viewAngle = value;
                SetDirty();
            }
        }

        public float ViewDist
        {
            get { return viewDist; }
            set
            {
                viewDist = value;
                SetDirty();
            }
        }

        public Matrix4x4 ProjMatrix
        {
            get
            {
                UpdateMatrix();
                return projMatrix;
            }
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                UpdateMatrix();
                return viewMatrix;
            }
        }
    }

    public class Model
    {
        internal Scene scene;
        public Mesh Mesh { get; private set; }
        public Texture Texture { get; private set; }
        Vector3 position;
        Vector3 scale = Vector3.One;
        Quaternion rotation = Quaternion.Identity;
        Matrix4x4 matrix;
        Matrix4x4 mvpMatrix;
        bool dirty = true;

        public Model(Mesh mesh, Texture texture)
        {
            Mesh = mesh;
            Texture = texture;
        }

        public void SetDirty()
        {
            if (!dirty)
            {
                dirty = true;
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                SetDirty();
            }
        }

        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                SetDirty();
            }
        }

        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                SetDirty();
            }
        }

        void UpdateMatrix()
        {
            if (dirty)
            {
                dirty = false;
                Matrix4x4.CreateTransform(ref position, ref rotation, ref scale, out matrix);
                mvpMatrix = matrix * scene.ViewMatrix * scene.ProjMatrix;
            }
        }

        public Matrix4x4 Matrix
        {
            get
            {
                UpdateMatrix();
                return matrix;
            }
        }

        public Matrix4x4 MvpMatrix
        {
            get
            {
                UpdateMatrix();
                return mvpMatrix;
            }
        }
    }
}
