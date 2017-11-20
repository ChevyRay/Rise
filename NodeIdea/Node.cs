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

        public T GetParent<T>() where T : Node
        {
            if (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                return parent.GetParent<T>();
            }
            return null;
        }

        //Do a scan for nodes at a particular depth. We recursively search
        //down the tree until we reach the depth, and then check if that depth
        //has what we're looking for. If not, we check if we can scan deeper.
        T DepthSearch<T>(int depth, ref bool keepSearching) where T : Node
        {
            if (depth > 0)
            {
                --depth;
                if (children != null)
                {
                    T result;
                    for (int i = 0; i < children.Count; ++i)
                    {
                        if (children[i] != null)
                        {
                            result = children[i].DepthSearch<T>(depth, ref keepSearching);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }
            else if (this is T)
                return (T)this;
            else if (children != null && children.Count > 0)
                keepSearching = true;
            return null;
        }
        void DepthSearch<T>(int depth, ref bool keepSearching, List<T> found) where T : Node
        {
            if (depth > 0)
            {
                --depth;
                if (children != null)
                    for (int i = 0; i < children.Count; ++i)
                        children[i]?.DepthSearch(depth, ref keepSearching, found);
            }
            else
            {
                if (this is T)
                    found.Add((T)this);
                if (children != null && children.Count > 0)
                    keepSearching = true;
            }
        }

        //Search for nodes breadth-first, so shallower nodes will be returned
        //before deeper ones. This is slower because it has to do depth scans.
        T FindBreadthFirst<T>() where T : Node
        {
            if (children != null && children.Count > 0)
            {
                T result;
                bool keepSearching = true;
                for (int depth = 0; keepSearching; ++depth)
                {
                    keepSearching = false;
                    for (int i = 0; i < children.Count; ++i)
                    {
                        if (children[i] != null)
                        {
                            result = children[i].DepthSearch<T>(depth, ref keepSearching);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }
            return null;
        }
        int FindBreadthFirst<T>(List<T> found) where T : Node
        {
            int count = found.Count;
            if (children != null && children.Count > 0)
            {
                bool keepSearching = true;
                for (int depth = 0; keepSearching; ++depth)
                {
                    keepSearching = false;
                    for (int i = 0; i < children.Count; ++i)
                        children[i]?.DepthSearch(depth, ref keepSearching, found);
                }
            }
            return found.Count - count;
        }

        //Search for nodes depth-first, so we travel down each branch until we find
        //a node. This is faster, but the result is not guaranteed to be shallowest.
        T FindDepthFirst<T>() where T : Node
        {
            if (children != null && children.Count > 0)
            {
                T result;
                for (int i = 0; i < children.Count; ++i)
                {
                    if (children[i] != null)
                    {
                        if (children[i] is T)
                            return (T)children[i];
                        result = children[i].FindDepthFirst<T>();
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }
        int FindDepthFirst<T>(List<T> found) where T : Node
        {
            int count = found.Count;
            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    if (children[i] != null)
                    {
                        if (children[i] is T)
                            found.Add((T)children[i]);
                        children[i].FindDepthFirst(found);
                    }
                }
            }
            return found.Count - count;
        }

        public T Find<T>(bool depthFirst) where T : Node
        {
            if (depthFirst)
                return FindDepthFirst<T>();
            return FindBreadthFirst<T>();
        }
        public int Find<T>(bool depthFirst, List<T> found) where T : Node
        {
            if (depthFirst)
                return FindDepthFirst(found);
            return FindBreadthFirst(found);
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

        public void Bubble(Predicate<Node> callback)
        {
            if (parent != null && callback(parent))
                parent.Bubble(callback);
        }

        public void Propagate(Predicate<Node> callback)
        {
            if (children != null)
                for (int i = 0, n = children.Count; i < n; ++i)
                    if (children[i] != null && callback(children[i]))
                        children[i].Propagate(callback);
        }

        void TriggerCleanup()
        {
            if (!cleanup)
            {
                cleanup = true;
                if (parent != null)
                    parent.TriggerCleanup();
                else if (Scene != null)
                    Scene.TriggerCleanup();
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
