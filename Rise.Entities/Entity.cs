using System;
using System.Collections.Generic;
namespace Rise.Entities
{
    public class Entity : IComparable<Entity>
    {
        public event Action OnAdded;
        public event Action<Scene> OnRemoved;

        static Predicate<Component> whereNull = e => e == null;

        public Scene Scene { get; private set; }
        internal ulong sceneAddIndex;

        public Transform2D Transform { get; private set; }

        List<Component> components = new List<Component>();
        bool cleanup;

        bool changeLock;

        public Entity()
        {
            Transform = new Transform2D();
        }

        internal void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;
                components.RemoveAll(whereNull);
            }
        }

        public T Add<T>(T component) where T : Component
        {
            if (component.Entity != null)
                throw new Exception("Component is already on an entity.");

            components.Add(component);
            component.Added(this);

            return component;
        }

        public void Remove(Component component)
        {
            if (component.Entity != this)
                throw new Exception("Component is not on this entity.");
            
            components[components.IndexOf(component)] = null;
            component.Removed();

            TriggerCleanup();
        }

        public int CompareTo(Entity other)
        {
            return sceneAddIndex.CompareTo(other.sceneAddIndex);
        }

        internal void Added(Scene scene, uint addIndex)
        {
            if (changeLock)
                throw new Exception("Cannot add entities during add/remove callbacks.");
            changeLock = true;

            Scene = scene;
            sceneAddIndex = addIndex;
            OnAdded?.Invoke();

            for (int i = 0, n = components.Count; i < n; ++i)
                components[i]?.AddedToScene(true);

            if (cleanup)
                Scene.TriggerCleanupEntities();

            changeLock = false;
        }

        internal void Removed()
        {
            if (changeLock)
                throw new Exception("Cannot remove entities during add/remove callbacks.");

            changeLock = true;

            var s = Scene;
            Scene = null;
            OnRemoved?.Invoke(s);

            for (int i = 0, n = components.Count; i < n; ++i)
                components[i]?.RemovedFromScene(s, true);

            changeLock = false;
        }

        protected void TriggerCleanup()
        {
            if (!cleanup)
            {
                cleanup = true;
                Scene?.TriggerCleanupEntities();
            }
        }
    }
}
