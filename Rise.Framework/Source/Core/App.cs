﻿using System;
using Rise.OpenGL;
using Rise.Imaging;
namespace Rise
{
    public delegate void ErrorHandler(Exception e);

    public static class App
    {
        public static event Action OnInit;
        public static event Action OnUpdate;
        public static event Action OnRender;
        public static event Action OnQuit;

        internal static ErrorHandler errorHandler;
        internal static Platform platform;
        static bool running;
        static bool focused;

        public static bool UpdateWhileFocused;
        public static bool FixedFramerate = true;
        static float framerate = 60f;
        static double frameDuration = 1.0 / 60.0;

        public static ImageLoader ImageLoader { get; private set; }

        public static int FPS { get; private set; }

        public static float Framerate
        {
            get { return framerate; }
            set
            {
                if (framerate != value)
                {
                    if (value <= 0f)
                        throw new Exception("Framerate must be greater than 0.");
                    framerate = value;
                    frameDuration = 1.0 / value;
                }
            }
        }

        public static void Init<PlatformType>() where PlatformType : Platform, new()
        {
            if (platform != null)
                throw new Exception("App has already been initialized.");
            platform = new PlatformType();
            ImageLoader = new ImageLoader();
        }

        public static void Run(string title, int width, int height, ErrorHandler errorHandler)
        {
            App.errorHandler = errorHandler;

            if (errorHandler != null)
            {
                try { Run(); }
                catch(Exception e) { errorHandler(e); }
            }
            else
                Run();

            void Run()
            {
                if (platform == null)
                    throw new Exception("App has not been initialized.");
                if (running)
                    throw new Exception("App is already running.");

                running = true;
                platform.Init(title, width, height);
                platform.CenterWindow();

                platform.OnQuit += Quit;
                platform.OnWinClose += Quit;
                platform.OnWinFocusGained += () => focused = true;
                platform.OnWinFocusLost += () => focused = false;

                GL.Init();
                Time.Init(1f / 60f);
                Screen.Init();
                Mouse.Init();
                Keyboard.Init();

                OnInit?.Invoke();

                double prevTime = platform.GetTime();
                double frameTimer = 0.0;

                while (running)
                {
                    platform.PollEvents();

                    if (!focused && !UpdateWhileFocused)
                    {
                        prevTime = (float)platform.GetTime();
                        continue;
                    }

                    if (running)
                    {
                        double currTime = platform.GetTime();
                        double deltaTime = currTime - prevTime;
                        if (deltaTime > 0.0)
                            FPS = (int)(1.0 / (currTime - prevTime));
                        prevTime = currTime;

                        if (FixedFramerate)
                        {
                            if (!Screen.VSync)
                            {
                                frameTimer += deltaTime;
                                while (frameTimer >= frameDuration)
                                {
                                    frameTimer -= frameDuration;
                                    Update(frameDuration);
                                }
                            }
                            else
                                Update(frameDuration);
                        }
                        else
                            Update(deltaTime);

                        void Update(double dt)
                        {
                            Time.PreUpdate((float)dt);
                            Mouse.PreUpdate();

                            OnUpdate?.Invoke();

                            Mouse.PostUpdate();
                            Keyboard.PostUpdate();

                            TextureBindings.UnbindAll();

                            DrawCall.Begin();
                            OnRender?.Invoke();
                            DrawCall.End();

                            platform.SwapBuffers();
                        }
                    }
                }

                OnQuit?.Invoke();

                ResourceHandle.DisposeAll();

                platform.Quit();
            }
        }

        public static void Quit()
        {
            //if (!running)
            //    throw new Exception("App is not running.");

            running = false;
        }
    }
}