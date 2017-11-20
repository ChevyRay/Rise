using System;
namespace Rise
{
    public class Component
    {
        Entity entity;

        internal void AddedToEntity(Entity e)
        {
            entity = e;
        }

        internal void RemovedFromEntity()
        {
            entity = null;
        }

        public Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    entity?.RemoveComponent(this);
                    value?.AddComponent(this);
                }
            }
        }

        public Scene Scene
        {
            get { return entity != null ? entity.Scene : null; }
        }
    }
}
