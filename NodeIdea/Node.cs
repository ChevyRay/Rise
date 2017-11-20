using System;
using System.Collections.Generic;
namespace NodeIdea
{
    public class Node
    {
        public Scene Scene { get; private set; }
        public int ChildCount { get; private set; }

        Node parent;
        List<Node> children;
        bool cleanup;
        int indexInParent;
        internal int indexInScene;

        public Node Parent
        {
            get { return parent; }
            set
            {
                //Do we want this kind of convenence setter? Or is this a bit too weird?
                if (parent != value)
                {
                    if (parent != null)
                        parent.RemoveChild(this);
                    else if (Scene != null)
                        throw new Exception("Cannot parent a scene's root node.");
                    value?.AddChild(this);
                }
            }
        }

        public T AddChild<T>() where T : Node, new()
        {
            return AddChild(new T());
        }
        public T AddChild<T>(T node) where T : Node
        {
            if (node.parent != null)
                throw new Exception("Node already has a parent.");
            if (node.Scene != null)
                throw new Exception("Cannot add root node.");

            node.parent = this;
            node.indexInParent = children.Count;
            children.Add(node);
            ++ChildCount;

            if (Scene != null)
                node.AddedToScene(Scene);

            return node;
        }

        public void RemoveChild(Node node)
        {
            if (node.parent != this)
                throw new Exception("Node is not a child.");

            node.parent = null;
            children[node.indexInParent] = null;
            --ChildCount;

            if (Scene != null)
                node.RemovedFromScene();

            TriggerCleanup();
        }

        public void RemoveSelf()
        {
            if (parent != null)
                parent.RemoveChild(this);
            else if (Scene != null)
                Scene.Root = null;
        }

        internal void AddedToScene(Scene s)
        {
            Scene = s;
            s.AddNode(this);
            if (children != null)
                for (int i = 0, n = children.Count; i < n; ++i)
                    children[i]?.AddedToScene(s);
        }

        internal void RemovedFromScene()
        {
            Scene.RemoveNode(this);
            Scene = null;
            if (children != null)
                for (int i = children.Count - 1; i >= 0; --i)
                    children[i]?.RemovedFromScene();
        }

        void TriggerCleanup()
        {
            if (!cleanup)
            {
                cleanup = true;
                parent?.TriggerCleanup();
            }
        }

        internal void Cleanup()
        {
            if (cleanup)
            {
                cleanup = false;
                for (int i = 0; i < children.Count; ++i)
                {
                    if (children[i] != null)
                    {
                        children[i].indexInParent = i;
                        children[i].Cleanup();
                    }
                    else
                        children.RemoveAt(i--);
                }
            }
        }
    }
}
