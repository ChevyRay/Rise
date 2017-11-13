using System;
using System.Collections.Generic;
namespace Rise
{
    public abstract class ResourceHandle
    {
        static HashSet<WeakReference<ResourceHandle>> resources = new HashSet<WeakReference<ResourceHandle>>();

        WeakReference<ResourceHandle> weak;

        protected ResourceHandle()
        {
            weak = new WeakReference<ResourceHandle>(this);
            resources.Add(weak);
        }
        ~ResourceHandle()
        {
            if (weak != null)
            {
                resources.Remove(weak);
                weak = null;
                Dispose();
            }
        }

        protected abstract void Dispose();

        internal static void DisposeAll()
        {
            ResourceHandle res;
            foreach (var weak in resources)
            {
                if (weak.TryGetTarget(out res) && res.weak != null)
                {
                    res.weak = null;
                    res.Dispose();
                }
            }
            resources.Clear();
        }
    }
}
