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
            App.Run("Rise.Test", 640, 360, null);
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
            var pinkSquare = new Texture("Assets/pink_square.png", true);
            var cubeMesh = Mesh3D.CreateCube(Vector3.One, Color4.White);

            //View
            var scene = new Scene(screenW, screenH);
            var viewDrag = false;
            var viewDragPos = Vector2.Zero;

            //Draw to screen
            var draw = new DrawCall(new Material(shader3D));

            //Create a model
            var cube = new Model(cubeMesh, pinkSquare);
            cube.Scale = new Vector3(2f, 0.75f, 1f);
            scene.Add(cube);

            //Create a model
            cube = new Model(cubeMesh, pinkSquare);
            cube.Position = new Vector3(0f, 0.5f, -2f);
            scene.Add(cube);

            App.OnUpdate += Update;
            App.OnRender += Render;
            Update();
            void Update()
            {
                //Panning
                if (viewDrag)
                {
                    var move = Mouse.Position - viewDragPos;
                    if (move != Vector2.Zero)
                    {
                        move *= 5f * Time.Delta;
                        scene.ViewRot = Quaternion.Euler(move.Y, move.X, 0f) * scene.ViewRot;
                        viewDragPos = Mouse.Position;
                    }
                    if (!Mouse.LeftDown)
                        viewDrag = false;
                }
                else if (Mouse.LeftPressed)
                {
                    viewDrag = true;
                    viewDragPos = Mouse.Position;
                }

                //Zooming
                if (Mouse.ScrollY != 0)
                {
                    scene.ViewDist -= Mouse.ScrollY * 10f * Time.Delta;
                }
            }
            void Render()
            {
                //draw.Perform(Color3.Black);

                draw.Clear(Color3.Black);
                foreach (var model in scene.Models)
                {
                    draw.Material.SetMatrix4x4("ModelViewProjectionMatrix", model.MvpMatrix);
                    draw.Material.SetTexture("Texture", model.Texture);
                    draw.Mesh = model.Mesh;
                    draw.Perform();
                }
            }
        }
    }

    public class Scene
    {
        Matrix4x4 projMatrix;
        Matrix4x4 viewMatrix;
        Quaternion viewRot = Quaternion.Identity;
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
                viewMatrix = Matrix4x4.CreateRotation(viewRot) * Matrix4x4.CreateTranslation(0f, 0f, -viewDist);
                foreach (var model in Models)
                    model.SetDirty();
            }
        }

        public Quaternion ViewRot
        {
            get { return viewRot; }
            set
            {
                viewRot = value;
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
