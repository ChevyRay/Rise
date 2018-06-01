using System;
using System.Collections.Generic;
namespace Rise
{
    public abstract class ResourceHandle
    {
        protected ResourceHandle()
        {
            
        }
        ~ResourceHandle()
        {
            if (App.Running)
                Dispose();
        }

        protected abstract void Dispose();
    }
}
