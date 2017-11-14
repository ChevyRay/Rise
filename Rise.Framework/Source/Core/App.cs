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

        public static ImageLoader ImageLoader { get; private set; }

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

                GL.Init();
                Time.Init(1f / 60f);
                Screen.Init();
                Mouse.Init();
                Keyboard.Init();

                OnInit?.Invoke();

                while (running)
                {
                    platform.PollEvents();

                    if (running)
                    {
                        Mouse.PreUpdate();

                        OnUpdate?.Invoke();

                        Time.PostUpdate(1f / 60f);
                        Mouse.PostUpdate();
                        Keyboard.PostUpdate();

                        Texture.MarkAllForUnbinding();
                        Texture.UnbindMarked();

                        Graphics.Begin();
                        OnRender?.Invoke();
                        Graphics.End();

                        platform.SwapBuffers();
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

        /*public static async Task<T> RunTask<T>(Task<T> task)
        {
            await task;
            if (errorHandler != null)
                errorHandler(task.Exception);
            else
                throw task.Exception;
            return task.Result;
        }
        public static async Task RunTask(Action action)
        {
            var task = Task.Run(action);
            await task;
            if (errorHandler != null)
                errorHandler(task.Exception);
            else
                throw task.Exception;
        }*/
    }
}