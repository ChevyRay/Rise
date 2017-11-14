using System;
namespace GameEngine
{
    public class SpriteAsset : Asset
    {
        
    }

    public class SpriteRenderer : Component<SpriteRenderer.Data>, IRenderer
    {
        //By putting all data in a struct, we can easily copy/store/serialize it
        public struct Data
        {
            public bool Visible;
            public Tag Tags;
            public AssetRef<SpriteAsset> Sprite;
            public Vector2 Position;
            public Vector2 Scale;
            public float Angle;
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
