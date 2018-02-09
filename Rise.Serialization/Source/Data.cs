using System;
namespace Rise.Serialization
{
    public class Data<T> : DataNode
    {
        static CustomSerializer serializer;

        public T Value;

        internal override void Init(DataTree tree, DataNode parent, string path)
        {
            base.Init(tree, parent, path);

            //Hacky, but strings should default to "", not null
            if (this is Data<string>)
            {
                GetType().GetField("Value").SetValue(this, string.Empty);
            }
        }

        public override void WriteBytes(ByteWriter writer)
        {
            if (serializer == null)
                serializer = CustomSerializer.Get<T>();
            serializer.WriteBytes(Value, writer);
        }

        public override void ReadBytes(ByteReader reader)
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
