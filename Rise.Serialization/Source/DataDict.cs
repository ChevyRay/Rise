using System;
using System.Collections.Generic;
namespace Rise.Serialization
{
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
