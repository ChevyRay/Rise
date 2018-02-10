using System;
using System.Collections.Generic;
using Rise.Serialization;
namespace Rise.DataTree
{
    /// 
    /// Some notes:
    /// 
    ///     -   My naming convention for all this is really bad (DataNode, DataTree,
    ///         SerializedData, Data<T> wut!??) We should probably find something
    ///         a bit more clear to understand. Also, we might want to rename the
    ///         library/namespace as Rise.Data or Rise.DataModel or something??
    /// 
    ///     -   One slow thing is like... if you add/remove to a list, the entire
    ///         list has to be serialized in order to undo it. I feel like there's
    ///         a better way to accomplish this, but it's a bit tricky. It seems
    ///         like a weak-point of this sort of serialization-based undo system.
    /// 
    ///     -   DataNode could hold a reference to its most recent SerializedData,
    ///         and then flag when it has been modified (or hash, etc.) This way,
    ///         when large objects change, we'd only have to re-serialize portions.
    /// 
    ///     -   We might want to allow the ability to load/unload undo lists for
    ///         DataTree, so we could have an undo stack that was a lot larger,
    ///         but could potentially not sit around hogging tons of memory. This
    ///         would only really be necessary for super memory-heavy application.
    /// 
    ///     -   Because of how the serializers work, it is entirely possible to
    ///         multi-thread the serialization process if it becomes slow. DataTree
    ///         would need to create a separate ByteWriter/Reader for each thread,
    ///         but other than that it should be completely thread-safe (lol i hope).
    /// 

    public class DataModel
    {
        public DataNode Root { get; private set; }
        public int MaxUndos { get; private set; }
        public int PastCount { get { return past.Count; } }
        public int FutureCount { get { return future.Count; } }

        List<SerializedData> past = new List<SerializedData>();
        List<SerializedData> future = new List<SerializedData>();
        ByteWriter writer = new ByteWriter(true);
        ByteReader reader = new ByteReader(true);

        public DataModel(Type rootType, int maxUndos)
        {
            Root = Activator.CreateInstance(rootType) as DataNode;
            if (Root == null)
                throw new Exception("Root type is not a DataNode: " + rootType);
            Root.Init(this, null, "Root");

            MaxUndos = maxUndos;
        }
        public DataModel(Type rootType) : this(rootType, 50)
        {
            
        }

        public DataNode FindByPath(string path)
        {
            if (path.StartsWith("Root."))
                return Root.FindByPath(path.Substring(5));
            return null;
        }

        internal SerializedData SerializeNode(DataNode node)
        {
            writer.Clear();
            node.WriteBytes(writer);
            var bytes = writer.GetBytes();
            return new SerializedData(node.GetType(), node.PathToNode, bytes);
        }

        internal void DeserializeNode(DataNode node, SerializedData state)
        {
            if (state.Type != node.GetType())
                throw new Exception($"Type mismatch on deserialization: expected {state.Type}, got {node.GetType()}.");

            reader.Init(state.Bytes);
            node.ReadBytes(reader);
        }

        void RecordInto(DataNode node, List<SerializedData> list)
        {
            if (list.Count == MaxUndos)
                list.RemoveAt(0);

            list.Add(SerializeNode(node));
        }

        internal void RecordUndo(DataNode node)
        {
            RecordInto(node, past);
        }

        void LoadState(List<SerializedData> fromList, List<SerializedData> toList)
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
}
