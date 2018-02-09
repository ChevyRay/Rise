using System;
using System.Collections.Generic;
namespace Rise.Serialization
{
    public class DataList<T> : DataNode where T : DataNode, new()
    {
        List<T> nodes = new List<T>();

        public int Count
        {
            get { return nodes.Count; }
        }

        public T this[int i]
        {
            get { return nodes[i]; }
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public T Add()
        {
            var node = new T();
            node.Init(Tree, this, $"{PathToNode}.{nodes.Count}");
            nodes.Add(node);
            return node;
        }

        public bool Contains(T node)
        {
            return nodes.Contains(node);
        }

        public void Remove(T node)
        {
            RemoveAt(nodes.IndexOf(node));
        }

        public void RemoveAt(int index)
        {
            nodes.RemoveAt(index);

            //Update the paths of the nodes after the removed item
            for (int i = index; i < nodes.Count; ++i)
                nodes[i].PathToNode = $"{PathToNode}.{i}";
        }

        public void RemoveWhere(Predicate<T> pred)
        {
            int count = nodes.Count;
            for (int i = 0; i < nodes.Count; ++i)
                if (pred(nodes[i]))
                    nodes.RemoveAt(i--);
            if (nodes.Count < count)
                UpdateAllPaths();
        }

        public T Find(Predicate<T> pred)
        {
            for (int i = 0; i < nodes.Count; ++i)
                if (pred(nodes[i]))
                    return nodes[i];
            return null;
        }

        void UpdateAllPaths()
        {
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].PathToNode = $"{PathToNode}.{i}";
        }

        internal override void WriteBytes(ByteWriter writer)
        {
            writer.WriteInt(nodes.Count);
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].WriteBytes(writer);
        }

        internal override void ReadBytes(ByteReader reader)
        {
            Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                var node = Add();
                node.ReadBytes(reader);
            }
        }

        public override DataNode FindByPath(string path)
        {
            int i = path.IndexOf('.');
            if (i >= 0)
            {
                int index = int.Parse(path.Substring(0, i));
                return nodes[index].FindByPath(path.Substring(i + 1));
            }
            else
            {
                int index = int.Parse(path);
                return nodes[index];
            }
        }
    }
}
