using System;
namespace GameEngine
{
    //This would be a static class that manages all assets
    //You load/unload/etc. assets through this, and can track their progress, etc.
    public static class Assets
    {
        public static T Find<T>(string name) where T : Asset
        {
            return null;
        }
    }

    //Base class for all asset types
    public abstract class Asset
    {
        public string Name;
        public int Hash;
        public bool Disposed;
    }

    //Instead of referencing assets directly, you'd use an AssetRef to that type
    //This protects objects from referencing invalid/unloaded assets
    //If an asset gets replaces, the reference will then point to the new one
    //This also allows asset references to serialize more accurately
    [Data]
    public struct AssetRef<T> where T : Asset
    {
        [Field(0)] string name;

        [NonSerialized]
        T asset;

        public T Asset
        {
            get
            {
                //If asset is replaced or unloaded, we will know
                if (asset == null || asset.Disposed)
                    asset = Assets.Find<T>(name);
                return asset;
            }
        }

        public static implicit operator T(AssetRef<T> r)
        {
            return r.Asset;
        }

        public static implicit operator AssetRef<T>(T asset)
        {
            AssetRef<T> r;
            r.name = asset.Name;
            r.asset = null;
            return r;
        }
    }
}
