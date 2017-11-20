using System;
namespace Rise
{
    public class Component
    {
        Entity entity;

        public Component()
        {
            
        }

        /*public Entity Entity
        {
            get { return entity; }
            set
            {
                if (entity != value)
                {
                    if (entity != null)
                    {
                        entity.components[entity.components.IndexOf(this)] = null;
                        entity.TriggerCleanup();
                    }

                    if (value != null)
                    {
                        entity = value;
                        entity.components.Add(this);
                    }
                    else
                    {
                        entity = null;
                    }
                }
            }
        }*/
    }
}
