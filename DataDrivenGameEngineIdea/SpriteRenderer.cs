using System;
using System.Runtime.InteropServices;
namespace GameEngine
{
    public class SpriteAsset : Asset
    {
        
    }

    public class SpriteRenderer : Component<SpriteRenderer.Data>, IRenderer
    {
        //By putting all data in a struct, we can easily copy/store/serialize it
        [Data]
        public struct Data
        {
            //By assigning unique IDs to field, we can ensure they're loaded properly
            //Because in the binary format, fields will be preceded by their ID
            [Field(0)] public bool Visible;
            [Field(1)] public Tag Tags;
            [Field(2)] public AssetRef<SpriteAsset> Sprite;
            [Field(3)] public Vector2 Position;
            [Field(4)] public Vector2 Scale;
            [Field(5)] public float Angle;
        }

        public bool Visible { get { return data.Visible; } set { data.Visible = value; } }
        public Tag Tags { get { return data.Tags; } set { data.Tags = value; } }
        public AssetRef<SpriteAsset> Sprite { get { return data.Sprite; } set { data.Sprite = value; } }
        public Vector2 Position { get { return data.Position; } set { data.Position = value; } }
        public Vector2 Scale { get { return data.Scale; } set { data.Scale = value; } }
        public float Angle { get { return data.Angle; } set { data.Angle = value; } }

        public SpriteRenderer()
        {
            data.Visible = true;
            data.Tags = Tag.Default;
            data.Scale = Vector2.One;
        }

        public void Render()
        {
            
        }
    }
}
