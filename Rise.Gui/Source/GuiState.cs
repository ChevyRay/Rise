using System;
namespace Rise.Gui
{
    public partial class GuiState
    {
        static Vertex2D v0;
        static Vertex2D v1;
        static Vertex2D v2;
        static Vertex2D v3;

        public Shader Shader { get; private set; }
        public Texture TargetTexture { get; private set; }

        DrawCall draw;
        Mesh2D mesh;
        int matrixLoc;
        int textureLoc;

        Texture2D whitePixelTexture;

        Atlas currAtlas;
        Texture currTexture;
        Vector2 currWhitePixel;

        public GuiState(Texture targetTexture, Shader shader)
        {
            whitePixelTexture = new Texture2D(1, 1, new Color4[] { Color4.White });

            TargetTexture = targetTexture;
            Shader = shader;

            if (targetTexture != null)
                draw.Target = new RenderTarget(targetTexture);

            draw.Material = new Material(shader);
            matrixLoc = draw.Material.GetIndex("Matrix");
            textureLoc = draw.Material.GetIndex("Texture");
                
            mesh = new Mesh2D();
            draw.Mesh = mesh;

            draw.SetBlendMode(BlendMode.Premultiplied);
        }
        public GuiState(Texture targetTexture) : this(targetTexture, new Shader(Shader.Basic2D))
        {
            
        }
        public GuiState(Shader shader) : this(null, shader)
        {
            
        }
        public GuiState() : this(null, new Shader(Shader.Basic2D))
        {

        }
    }
}
