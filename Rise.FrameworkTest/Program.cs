using System;
using Rise.PlatformSDL;
namespace Rise.FrameworkTest
{
    static class Program
    {
        static int prevSecond;

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

            if ((int)Time.Total > prevSecond)
            {
                Console.WriteLine("Running time: {0}s", (int)Time.Total);
                prevSecond = (int)Time.Total;
            }
        }

        //Called every frame, after update has completed. It is wrapped in code that
        //managed the render state, so don't call DrawCall.Perform() outside of here.
        static void Render()
        {
            
        }
    }
}
