using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace Rise.Serialization
{
    public class SerializedNode
    {
        public Type Type { get; private set; }
        public string Path { get; private set; }
        public byte[] Bytes { get; private set; }

        internal SerializedNode(Type type, string path, byte[] bytes)
        {
            Type = type;
            Path = path;
            Bytes = bytes;
        }
    }

    public class DataTree
    {
        public DataNode Root { get; private set; }
        public int MaxUndos { get; private set; }
        public int PastCount { get { return past.Count; } }
        public int FutureCount { get { return future.Count; } }

        List<SerializedNode> past = new List<SerializedNode>();
        List<SerializedNode> future = new List<SerializedNode>();
        internal ByteWriter writer = new ByteWriter();
        ByteReader reader = new ByteReader();

        public DataTree(Type rootType, int maxUndos)
        {
            Root = Activator.CreateInstance(rootType) as DataNode;
            if (Root == null)
                throw new Exception("Root type is not a DataNode: " + rootType);
            Root.Init(this, null, "Root");

            MaxUndos = maxUndos;
        }
        public DataTree(Type rootType) : this(rootType, 50)
        {
            
        }

        public DataNode FindByPath(string path)
        {
            if (path.StartsWith("Root."))
                return Root.FindByPath(path.Substring(5));
            return null;
        }

        internal SerializedNode SerializeNode(DataNode node)
        {
            writer.Clear();
            node.WriteBytes(writer);
            var bytes = writer.GetBytes();
            return new SerializedNode(node.GetType(), node.PathToNode, bytes);
        }

        internal void DeserializeNode(DataNode node, SerializedNode state)
        {
            if (state.Type != node.GetType())
                throw new Exception($"Type mismatch on deserialization: expected {state.Type}, got {node.GetType()}.");

            reader.Init(state.Bytes);
            node.ReadBytes(reader);
        }

        void RecordInto(DataNode node, List<SerializedNode> list)
        {
            if (list.Count == MaxUndos)
                list.RemoveAt(0);

            list.Add(SerializeNode(node));
        }

        internal void RecordUndo(DataNode node)
        {
            RecordInto(node, past);
        }

        void LoadState(List<SerializedNode> fromList, List<SerializedNode> toList)
        {
            if (fromList.Count > 0)
            {
                //Get the state to undo
                var state = fromList[fromList.Count - 1];
                fromList.RemoveAt(fromList.Count - 1);

                //Find the node that will be undone
                var node = FindByPath(state.Path);

                //Store the node in the second list
                RecordInto(node, toList);

                //Deserialize the node from the recorded bytes
                DeserializeNode(node, state);
            }
        }

        public void Undo()
        {
            LoadState(past, future);
        }

        public void Redo()
        {
            LoadState(future, past);
        }

        public void ClearUndoState()
        {
            past.Clear();
            future.Clear();
        }
    }

    public abstract class DataNode
    {
        public DataTree Tree { get; private set; }
        public DataNode ParentNode { get; private set; }
        public string PathToNode { get; internal set; }

        internal virtual void Init(DataTree tree, DataNode parent, string path)
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
            Tree.RecordUndo(this);
        }

        public SerializedNode Serialize()
        {
            return Tree.SerializeNode(this);
        }

        public void Deserialize(SerializedNode state)
        {
            Tree.DeserializeNode(this, state);
        }
    }

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
            node.Init(Tree, this, $"{PathToNode}.{key}");
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
