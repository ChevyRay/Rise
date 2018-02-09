using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Rise.Serialization
{
    public class DataTree
    {
        public DataNode Root { get; private set; }

        public DataTree(Type rootType)
        {
            Root = Activator.CreateInstance(rootType) as DataNode;
            if (Root == null)
                throw new Exception("Root type is not a DataNode: " + rootType);

            Root.Init(this, null, "Root");
        }
    }

    public abstract class DataNode
    {
        public DataTree Tree { get; private set; }
        public DataNode ParentNode { get; private set; }
        public string PathToNode { get; internal set; }

        internal void Init(DataTree tree, DataNode parent, string path)
        {
            Tree = tree;
            ParentNode = parent;
            PathToNode = path;

            foreach (var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var type = field.FieldType;
                if (typeof(DataNode).IsAssignableFrom(type))
                {
                    var node = (DataNode)Activator.CreateInstance(type);
                    node.Init(tree, this, $"{path}.{field.Name}");
                    field.SetValue(this, node);
                }
            }
        }

        public virtual void WriteBytes(ByteWriter writer)
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

        public virtual void ReadBytes(ByteReader reader)
        {
            
        }
    }

    public class Data<T> : DataNode
    {
        static CustomSerializer serializer;

        public T Value;

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
    }

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
            node.Init(Tree, this, $"{PathToNode}[{nodes.Count}]");
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
                nodes[i].PathToNode = $"{PathToNode}[{i}]";
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
                nodes[i].PathToNode = $"{PathToNode}[{i}]";
        }

        public override void WriteBytes(ByteWriter writer)
        {
            writer.Write(nodes.Count);
            for (int i = 0; i < nodes.Count; ++i)
                nodes[i].WriteBytes(writer);
        }

        public override void ReadBytes(ByteReader reader)
        {
            Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                var node = Add();
                node.ReadBytes(reader);
            }
        }
    }

    public class DataDict<K,T> : DataNode where T : DataNode, new()
    {
        static readonly Type[] allowedTypes = {
            typeof(bool),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(char),
            typeof(string)
        };

        static CustomSerializer keySerializer;

        static DataDict()
        {
            if (Array.IndexOf(allowedTypes, typeof(K)) < 0)
                throw new Exception("Illegal dictionary key type: " + typeof(K));
        }

        Dictionary<K, T> nodes = new Dictionary<K, T>();

        public int Count
        {
            get { return nodes.Count; }
        }

        public IEnumerable<KeyValuePair<K, T>> Pairs
        {
            get { return nodes; }
        }

        public IEnumerable<K> Keys
        {
            get { return nodes.Keys; }
        }

        public IEnumerable<T> Values
        {
            get { return nodes.Values; }
        }

        public T this[K key]
        {
            get { return nodes[key]; }
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public T Add(K key, bool overwrite)
        {
            if (!overwrite && nodes.ContainsKey(key))
                throw new Exception("Already contains item with key: " + key);

            var node = new T();
            node.Init(Tree, this, $"{PathToNode}[{key}]");
            nodes[key] = node;

            return node;
        }

        public bool Contains(K key)
        {
            return nodes.ContainsKey(key);
        }

        public bool TryGet(K key, out T value)
        {
            return nodes.TryGetValue(key, out value);
        }

        public T Get(K key)
        {
            return nodes[key];
        }

        public void Remove(K key)
        {
            nodes.Remove(key);
        }

        public override void WriteBytes(ByteWriter writer)
        {
            if (keySerializer == null)
                keySerializer = CustomSerializer.Get<K>();

            writer.Write(Count);
            foreach (var pair in nodes)
            {
                keySerializer.WriteBytes(pair.Key, writer);
                pair.Value.WriteBytes(writer);
            }
        }

        public override void ReadBytes(ByteReader reader)
        {
            if (keySerializer == null)
                keySerializer = CustomSerializer.Get<K>();

            Clear();
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                var key = (K)keySerializer.ReadBytes(reader);
                var value = Add(key, false);
                value.ReadBytes(reader);
            }
        }
    }
}
