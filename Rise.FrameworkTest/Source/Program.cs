using System;
using Rise.PlatformSDL;
namespace Rise.FrameworkTest
{
    static class Program
    {
        static int prevSecond;
        static Texture2D logo;
        static Texture2D star;
        static Mesh2D mesh;
        static Material material;

        static Rectangle[] positions = new Rectangle[20];

        public static void Main(string[] args)
        {
            App.OnInit += Init;
            App.OnUpdate += Update;
            App.OnRender += Render;
            App.Init<PlatformSDL2>();
            App.Run("Framework Test", 960, 540, ErrorHandler);
        }

        //If you pass an error handler to Run(), it will run a try/catch around the
        //entire process. This is useful for catch-all error reporting.
        static void ErrorHandler(Exception e)
        {
            
        }

        //This is called when the app has set up all the platform/context/etc. code
        //This is also where you'd load any global assets, etc.
        //Basically, don't interact with the Rise API until this is called
        static void Init()
        {
            logo = new Texture2D("Assets/rise_logo.png", true);
            star = new Texture2D("Assets/star.png", true);

            mesh = new Mesh2D();

            var shader = new Shader(Shader.Basic2D);
            material = new Material(shader);

            Screen.ClearColor = 0x1e4e50ff;
        }

        //Called every frame (default is 60 FPS). This is where you can interact with
        //Mouse, Keyboard, Time, etc. They are all updated right before this is called.
        static void Update()
        {
            if (Mouse.Pressed(MouseButton.Left))
            {
                Console.WriteLine("Left click!");
            }

            if (Keyboard.Down(KeyCode.Space))
            {
                Console.WriteLine("Space is held down...");
            }

            if (Time.Total > prevSecond + 1)
            {
                prevSecond = (int)Time.Total;
                Console.WriteLine("Running time: {0}s", prevSecond);
            }

            for (int i = 1; i < positions.Length; ++i)
                positions[i - 1] = positions[i];
            positions[positions.Length - 1] = new Rectangle(Mouse.X - star.Width / 2, Mouse.Y - star.Height / 2, star.Width, star.Height);;
        }

        //Called every frame, after update has completed. It is wrapped in code that
        //managed the render state, so don't call DrawCall.Perform() outside of here.
        static void Render()
        {
            //First we setup our draw call
            var draw = new DrawCall(null, material, mesh, BlendMode.Premultiplied);
            material.SetMatrix4x4("Matrix", Matrix4x4.CreateOrthographic(Screen.DrawWidth, Screen.DrawHeight, -1f, 1f));

            //Then we'll draw the logo
            {
                //The material is how we interact with shaders
                material.SetTexture("Texture", logo);

                //A mesh defines the geometry of what we're drawing
                mesh.Clear();
                var rect = Rectangle.Box(new Vector2(Screen.DrawWidth / 2, Screen.DrawHeight / 2 - 32), logo.Width, logo.Height);
                mesh.AddRect(rect, Vector2.Zero, Vector2.One);
                mesh.Update();

                //To render, we setup a draw call and then perform it

                draw.Perform();
            }

            //After we call Perform(), we can change stuff and then call Perform() again
            //The the rendering system will internally optimize the draw state
            {
                material.SetTexture("Texture", star);

                mesh.Clear();
                for (int i = 0; i < positions.Length; ++i)
                {
                    float a = i / (positions.Length - 1f);
                    mesh.AddRect(positions[i], Vector2.Zero, Vector2.One, Color4.White * a, Color4.Transparent);
                }
                mesh.Update();

                draw.Perform();
            }


            //This might seem tedious, but in reality this will be wrapped in components
            //or some kind of rendering class! For example, the Atlas automatically allows
            //us to pack textures/fonts into atlases, and the DrawBatch2D allows us to
            //use simple draw functions between a Begin() and End()
            //We just want to provide a low-level API so you have lots of options
        }
    }
}
