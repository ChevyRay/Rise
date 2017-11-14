using System;
namespace GameEngine
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class DataAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
        public int ID { get; private set; }

        public FieldAttribute(int id)
        {
            ID = id;
        }
    }
}
