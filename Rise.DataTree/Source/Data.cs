using System;
using Rise.Serialization;
namespace Rise.DataTree
{
    public class Data<T> : DataNode
    {
        static CustomSerializer serializer;

        public T Value;

        internal override void Init(DataModel tree, DataNode parent, string path)
        {
            base.Init(tree, parent, path);

            //Hacky, but strings should default to "", not null
            if (this is Data<string>)
            {
                GetType().GetField("Value").SetValue(this, string.Empty);
            }
        }

        internal override void WriteBytes(ByteWriter writer)
        {
            if (serializer == null)
                serializer = CustomSerializer.Get<T>();
            serializer.WriteBytes(Value, writer);
        }

        internal override void ReadBytes(ByteReader reader)
        {
            if (serializer == null)
                serializer = CustomSerializer.Get<T>();
            Value = (T)serializer.ReadBytes(reader);
        }

        public override DataNode FindByPath(string path)
        {
            throw new Exception("Cannot search Data<T> by path");
        }
    }
}
