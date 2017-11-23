using System;
using System.Collections.Generic;
namespace Rise.Entities
{
    public class Scene
    {
        List<Entity> entities = new List<Entity>();
        bool cleanup;

        public Scene()
        {
            
        }

        internal void TriggerCleanup()
        {
            cleanup = true;
        }

        public T Add<T>(T entity) where T : Entity
        {
            if (entity.Scene != null)
                throw new Exception("Entity is already in a scene.");

            entities.Add(entity);
            entity.AddedToScene(this);
            return entity;
        }

        public void Remove(Entity entity)
        {
            if (entity.Scene != this)
                throw new Exception("Entity is not in this scene.");

            cleanup = true;
            entities[entities.BinarySearch(entity)] = null;
            entity.RemovedFromScene();
        }

        public void RemoveWithAllChildren(Entity entity)
        {
            
        }

        void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;

                for (int i = entities.Count - 1; i >= 0; --i)
                {
                    if (entities[i] != null)
                        entities[i].Cleanup();
                    else
                        entities.RemoveAt(i);
                }
            }
        }
    }
}
