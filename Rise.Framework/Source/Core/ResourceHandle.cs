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
            //resources.Remove(weak);
            //Dispose();
        }

        protected abstract void Dispose();

        internal static void DisposeAll()
        {
            ResourceHandle res;
            foreach (var weak in resources)
            {
                if (weak.TryGetTarget(out res) && res.weak != null)
                {
                    res.Dispose();
                    GC.SuppressFinalize(res);
                }
            }
            resources.Clear();
        }
    }
}
