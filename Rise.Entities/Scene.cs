using System;
using System.Collections.Generic;
namespace Rise.Entities
{
    public class Scene
    {
        List<Manager> managers = new List<Manager>();
        List<Entity> entities = new List<Entity>();
        bool cleanupEntities;
        bool cleanupManagers;
        uint nextAddIndex;

        public Scene()
        {
            
        }

        internal void ComponentAdded(Component component)
        {
            for (int i = 0; i < managers.Count; ++i)
                managers[i]?.ComponentAdded(component);
        }

        internal void ComponentRemoved(Component component)
        {
            for (int i = managers.Count - 1; i > 0; --i)
                managers[i]?.ComponentRemoved(component);
        }

        internal void TriggerCleanupManagers()
        {
            cleanupManagers = true;
        }

        internal void TriggerCleanupEntities()
        {
            cleanupEntities = true;
        }

        public T AddManager<T>(T manager) where T : Manager
        {
            if (manager.Scene != null)
                throw new Exception("Manager is already in a scene.");

            managers.Add(manager);
            manager.Added(this);

            return manager;
        }

        public void RemoveManager(Manager manager)
        {
            if (manager.Scene != this)
                throw new Exception("Manager is not in this scene.");

            cleanupEntities = true;
            managers[managers.IndexOf(manager)] = null;
            manager.Removed();
        }

        public T Add<T>(T entity) where T : Entity
        {
            if (entity.Scene != null)
                throw new Exception("Entity is already in a scene.");

            if (nextAddIndex == uint.MaxValue)
                ResetAddIndices();

            entities.Add(entity);
            entity.Added(this, nextAddIndex++);
            return entity;
        }

        public void Remove(Entity entity)
        {
            if (entity.Scene != this)
                throw new Exception("Entity is not in this scene.");

            cleanupEntities = true;
            entities[entities.BinarySearch(entity)] = null;
            entity.Removed();
        }

        public void RemoveWithChildren(Entity entity)
        {
            Remove(entity);
            RemoveChildren(entity);
        }

        public void RemoveChildren(Entity entity)
        {
            for (int i = 0, n = entities.Count; i < n; ++i)
                if (entities[i] != null && entity.IsAncestorOf(entities[i]))
                    Remove(entities[i]);
        }

        public IEnumerable<T> GetEntities<T>() where T : Entity
        {
            for (int i = 0, n = entities.Count; i < n; ++i)
                if (entities[i] is T)
                    yield return (T)entities[i];
        }

        public void RemoveAll<T>() where T : Entity
        {
            for (int i = 0, n = entities.Count; i < n; ++i)
                if (entities[i] is T)
                    Remove(entities[i]);
        }

        void ResetAddIndices()
        {
            nextAddIndex = 0;
            for (int i = 0; i < entities.Count; ++i)
                if (entities[i] != null)
                    entities[i].sceneAddIndex = nextAddIndex++;
        }

        void Cleanup()
        {
            if (cleanupManagers)
            {
                cleanupManagers = false;
                for (int i = managers.Count - 1; i >= 0; --i)
                {
                    if (managers[i] != null)
                        managers[i].Cleanup();
                    else
                        managers.RemoveAt(i);
                }
            }

            if (cleanupEntities)
            {
                cleanupEntities = false;
                for (int i = entities.Count - 1; i >= 0; --i)
                {
                    if (entities[i] != null)
                        entities[i].Cleanup();
                    else
                        entities.RemoveAt(i);
                }
            }
        }

        public IEnumerable<Entity> Entities
        {
            get
            {
                for (int i = 0, n = entities.Count; i < n; ++i)
                    if (entities[i] != null)
                        yield return entities[i];
            }
        }
    }
}
