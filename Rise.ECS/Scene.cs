using System;
using System.Collections.Generic;
namespace Rise
{
    public class Scene
    {
        List<Entity> rootEntities = new List<Entity>();
        List<Entity> allEntities = new List<Entity>();
        bool cleanup;

        public T AddEntity<T>(T e) where T : Entity
        {
            if (e.scene != null)
                throw new Exception("Entity is already in a scene.");
            if (e.parent != null)
                throw new Exception("Can only add root entities to the scene.");

            e.AssignSearchIndex();
            rootEntities.Add(e);
            e.AddedToScene(this);

            if (e.cleanup)
                TriggerCleanup();

            return e;
        }

        public T RemoveEntity<T>(T e) where T : Entity
        {
            if (e.scene != this)
                throw new Exception("Entity is not in this scene.");
            if (e.parent != null)
                throw new Exception("Can only remove root entities from the scene.");

            TriggerCleanup();
            rootEntities[rootEntities.BinarySearch(e)] = null;
            e.RemovedFromScene();

            return e;
        }

        internal void EntityAdded(Entity e)
        {
            allEntities.Add(e);
        }

        internal void EntityRemoved(Entity e)
        {
            allEntities[allEntities.BinarySearch(e)] = null;
        }

        internal void TriggerCleanup()
        {
            cleanup = true;
        }

        void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;
                for (int i = rootEntities.Count - 1; i >= 0; --i)
                {
                    if (rootEntities[i] != null)
                        rootEntities[i].Cleanup();
                    else
                        rootEntities.RemoveAt(i);
                }
            }
        }
    }
}
