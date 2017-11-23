using System;
namespace Rise.Entities
{
    public class Component
    {
        public Entity Entity { get; private set; }

        public Component()
        {
            
        }

        internal void AddedToEntity(Entity entity)
        {
            Entity = entity;
        }

        internal void RemovedFromEntity()
        {
            Entity = null;
        }
    }
}
