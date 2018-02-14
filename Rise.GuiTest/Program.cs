using System;

namespace Rise.GuiTest
{
    class Program
    {
        public static void Main(string[] args)
        {
            App.OnInit += Init;
            App.OnUpdate += Update;
            App.OnRender += Render;
            App.Init<PlatformSDL.PlatformSDL2>();
            App.Run("Gui Test", 960, 540, null);

            void Init()
            {

            }

            void Update()
            {

            }

            void Render()
            {

            }
        }
    }
}
