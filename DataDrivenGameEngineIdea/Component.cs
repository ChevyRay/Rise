using System;
namespace GameEngine
{
    public interface IComponent<T> where T : struct
    {
        void SetData(ref T value);
        void GetData(out T results);
    }

    public abstract class Component<T> : IComponent<T> where T : struct
    {
        protected T data;

        public virtual void SetData(ref T value)
        {
            data = value;
        }

        public void GetData(out T results)
        {
            results = data;
        }
    }

    public interface IUpdater
    {
        bool Active { get; set; }
        Tag Tags { get; set; }
        void Update();
    }

    public interface IRenderer
    {
        bool Visible { get; set; }
        Tag Tags { get; set; }
        void Render();
    }
}
