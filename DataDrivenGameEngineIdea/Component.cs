using System;
using System.IO;
namespace GameEngine
{
    public interface IDataObject<T> where T : struct
    {
        void SetData(ref T value);
        void GetData(out T results);
    }

    public abstract class Component
    {
        
    }

    public abstract class Component<T> : Component, IDataObject<T> where T : struct
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
