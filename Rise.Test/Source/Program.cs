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

        static void Init()
        {
            
        }

        static void Update()
        {
            
        }

        static void Render()
        {
            Graphics.Clear(Color.Black);
        }
    }
}
