using System;
namespace Rise.Entities
{
    public class Component
    {
        public event Action OnAdded;
        public event Action<Entity> OnRemoved;
        public event Action OnAddedToScene;
        public event Action<Scene> OnRemovedFromScene;

        public Entity Entity { get; private set; }

        bool changeLock;

        public Scene Scene
        {
            get { return Entity != null ? Entity.Scene : null; }
        }

        internal void Added(Entity entity)
        {
            if (changeLock)
                throw new Exception("Cannot add a component in add/remove callbacks.");

            changeLock = true;

            Entity = entity;
            OnAdded?.Invoke();
            if (entity.Scene != null)
                AddedToScene(false);
            
            changeLock = false;
        }

        internal void Removed()
        {
            if (changeLock)
                throw new Exception("Cannot emove a component in add/remove callbacks.");

            changeLock = true;

            var e = Entity;
            Entity = null;
            OnRemoved?.Invoke(e);
            if (e.Scene != null)
                RemovedFromScene(e.Scene, false);
            
            changeLock = false;
        }

        internal void AddedToScene(bool checkLock)
        {
            if (checkLock && changeLock)
                throw new Exception("Cannot change a component's scene in add/remove callbacks.");

            Scene.ComponentAdded(this);
            OnAddedToScene?.Invoke();
        }

        internal void RemovedFromScene(Scene scene, bool checkLock)
        {
            if (checkLock && changeLock)
                throw new Exception("Cannot change a component's scene add/remove callbacks.");

            Scene.ComponentRemoved(this);
            OnRemovedFromScene?.Invoke(scene);
        }
    }
}
