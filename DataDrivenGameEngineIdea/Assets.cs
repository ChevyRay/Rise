using System;
namespace GameEngine
{
    public static class Assets
    {
        public static T Find<T>(string name) where T : Asset
        {
            return null;
        }
    }

    public abstract class Asset
    {
        public string Name;
        public int Hash;
        public bool Disposed;
    }

    public struct AssetRef<T> where T : Asset
    {
        string name;
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
