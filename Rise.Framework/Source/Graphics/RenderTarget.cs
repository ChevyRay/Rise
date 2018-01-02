using System;
using Rise.OpenGL;
namespace Rise
{
    public class RenderTarget : ResourceHandle
    {
        struct RenderTexture
        {
            public uint ID;
            public TextureTarget Target;
        }

        static RenderTarget binded;
        static DrawBuffer[] drawBuffers = new DrawBuffer[16];

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture DepthTexture { get; private set; }

        internal uint id;
        RenderTexture[] textures = new RenderTexture[16];

        public RenderTarget(int width, int height)
        {
            id = GL.GenFramebuffer();
            Width = width;
            Height = height;
        }
        public RenderTarget(Texture texture) : this(texture.Width, texture.Height)
        {
            SetTexture(0, texture);
        }
        public RenderTarget(int width, int height, Texture depthTexture, params Texture[] textures) : this(width, height)
        {
            SetDepthTexture(depthTexture);
            for (int i = 0; i < textures.Length; ++i)
                SetTexture(i, textures[i]);
        }

        protected override void Dispose()
        {
            GL.DeleteFramebuffer(id);
        }

        public void BlitTextureTo(RenderTarget target, int textureNum, BlitFilter filter, RectangleI rect)
        {
            if (textures[textureNum].ID == 0)
                throw new Exception("RenderTarget does not have a texture in slot: " + textureNum);

            Bind(null);
            GL.BindFramebuffer(FramebufferTarget.Read, id);
            GL.BindFramebuffer(FramebufferTarget.Draw, target != null ? target.id : 0);
            GL.ReadBuffer((ReadBuffer)((uint)ReadBuffer.Color0 + textureNum));
            GL.BlitFramebuffer(new RectangleI(Width, Height), rect, BufferBit.Color, filter);
        }
        public void BlitTextureTo(RenderTarget target, int textureNum, BlitFilter filter, int x, int y)
        {
            BlitTextureTo(target, textureNum, filter, new RectangleI(x, y, Width, Height));
        }

        public void Clear(Color4 color)
        {
            Bind(this);
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(BufferBit.Color | BufferBit.Depth);
        }

        /*public Texture GetTexture(int n)
        {
            if (n < 0 || n >= textures.Length)
                throw new ArgumentOutOfRangeException(nameof(n));

            return textures[n];
        }*/

        void SetTexture(int n, uint texID, TextureTarget texTarget)
        {
            if (n < 0 || n >= textures.Length)
                throw new ArgumentOutOfRangeException(nameof(n));
            Bind(this);
            textures[n].ID = texID;
            textures[n].Target = texTarget;
            var col = (TextureAttachment)((uint)TextureAttachment.Color0 + n);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, col, texTarget, texID, 0);

            //TODO: probably don't need to do this for every one!?
            UpdateDrawBuffers();
        }

        public void SetTexture(int n, Texture texture)
        {
            if (texture.Width != Width || texture.Height != Height)
                throw new Exception("Texture size must be the same as RenderTarget.");
            if (texture.Format.PixelFormat() == PixelFormat.Depth)
                throw new Exception("Texture attachment cannot be a depth texture.");

            SetTexture(n, texture.ID, texture.BindTarget);
        }

        public void SetDepthTexture(Texture texture)
        {
            if (texture != null)
            {
                if (texture.Width != Width || texture.Height != Height)
                    throw new Exception("Texture size must be the same as RenderTarget.");
                if (texture.Format.PixelFormat() != PixelFormat.Depth)
                    throw new Exception("Texture is not a depth texture.");
                Bind(this);

                //If the target is different than the previous depth texture, clear the old target
                if (DepthTexture != null && DepthTexture.BindTarget != texture.BindTarget)
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, TextureAttachment.Depth, DepthTexture.BindTarget, 0, 0);

                DepthTexture = texture;
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, TextureAttachment.Depth, texture.BindTarget, texture.ID, 0);
            }
            else if (DepthTexture != null)
            {
                Bind(this);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, TextureAttachment.Depth, DepthTexture.BindTarget, 0, 0);
                DepthTexture = null;
            }
        }

        void UpdateDrawBuffers()
        {
            //Pipe each color output into its corresponding texture
            int num = 0;
            for (uint i = 0; i < drawBuffers.Length; ++i)
                if (textures[i].ID != 0)
                    drawBuffers[num++] = (DrawBuffer)((uint)DrawBuffer.Color0 + i);
            GL.DrawBuffers(num, drawBuffers);
        }

        internal static void Bind(RenderTarget buffer)
        {
            if (binded != buffer)
            {
                binded = buffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer != null ? buffer.id : 0);
            }
        }
    }
}
