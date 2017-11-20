using System;
using System.Collections.Generic;
namespace NodeIdea
{
    public class Scene
    {
        public int NodeCount { get; private set; }

        Node root;
        List<Node> nodes = new List<Node>();
        bool cleanup;

        public Node Root
        {
            get { return root; }
            set
            {
                if (root != value)
                {
                    root?.RemovedFromScene();
                    if (value != null)
                    {
                        if (value.Parent != null)
                            throw new Exception("Node must be unparented to be the root of a scene.");
                        root = value;
                        root.AddedToScene(this);
                    }
                }
            }
        }

        internal void AddNode(Node node)
        {
            node.indexInScene = nodes.Count;
            nodes.Add(node);
            ++NodeCount;
        }

        internal void RemoveNode(Node node)
        {
            nodes[node.indexInScene] = null;
            --NodeCount;
            cleanup = true;
        }

        internal void Cleanup()
        {
            Root.Cleanup();
            if (cleanup)
            {
                cleanup = false;
                for (int i = 0; i < nodes.Count; ++i)
                {
                    if (nodes[i] != null)
                        nodes[i].indexInScene = i;
                    else
                        nodes.RemoveAt(i--);
                }
            }
        }

        public Node[] GetAllNodes()
        {
            var arr = new Node[NodeCount];
            int n = 0;
            for (int i = 0; i < nodes.Count; ++i)
                if (arr[i] != null)
                    arr[n++] = arr[i];
            return arr;
        }

        public IEnumerable<Node> Nodes
        {
            get
            {
                for (int i = 0, n = nodes.Count; i < n; ++i)
                    if (nodes[i] != null)
                        yield return nodes[i];
            }
        }
    }
}
