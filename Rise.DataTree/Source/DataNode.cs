using System;
using System.Reflection;
using Rise.Serialization;
namespace Rise.DataTree
{
    public abstract class DataNode
    {
        public DataModel Model { get; private set; }
        public DataNode ParentNode { get; private set; }
        public string PathToNode { get; internal set; }

        internal virtual void Init(DataModel model, DataNode parent, string path)
        {
            Model = model;
            ParentNode = parent;
            PathToNode = path;

            foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = field.FieldType;
                if (typeof(DataNode).IsAssignableFrom(type))
                {
                    var node = (DataNode)Activator.CreateInstance(type);
                    node.Init(model, this, $"{path}.{field.Name}");
                    field.SetValue(this, node);
                }
            }
        }

        internal virtual void WriteBytes(ByteWriter writer)
        {
            foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = field.FieldType;
                var value = field.GetValue(this);
                if (value == null)
                    throw new Exception($"Field is null: {type}.{field.Name}");
                if (value.GetType() != type)
                    throw new Exception($"Value type [{value.GetType()}] differs from field type [{type}] in field: {type}.{field.Name}.");

                if (value is DataNode)
                {
                    (value as DataNode).WriteBytes(writer);
                }
                else
                {
                    var serializer = CustomSerializer.Get(type);
                    serializer.WriteBytes(value, writer);
                }
            }
        }

        internal virtual void ReadBytes(ByteReader reader)
        {
            foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = field.FieldType;
                if (typeof(DataNode).IsAssignableFrom(type))
                {
                    var node = (DataNode)field.GetValue(this);
                    node.ReadBytes(reader);
                }
            }
        }

        public virtual DataNode FindByPath(string path)
        {
            int i = path.IndexOf('.');
            if (i >= 0)
            {
                var field = GetType().GetField(path.Substring(0, i));
                var node = (DataNode)field.GetValue(this);
                return node.FindByPath(path.Substring(i + 1));
            }
            else
            {
                var field = GetType().GetField(path);
                return (DataNode)field.GetValue(this);
            }
        }

        public void Record()
        {
            Model.RecordUndo(this);
        }

        public SerializedData Serialize()
        {
            return Model.SerializeNode(this);
        }

        public void Deserialize(SerializedData state)
        {
            Model.DeserializeNode(this, state);
        }
    }
}
