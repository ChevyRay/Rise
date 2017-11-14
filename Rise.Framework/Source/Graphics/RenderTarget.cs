using System;
using Rise.OpenGL;
namespace Rise
{
    public class RenderTarget : ResourceHandle
    {
        static RenderTarget binded;
        static RenderTarget target;
        static DrawBuffer[] drawBuffers = new DrawBuffer[16];

        internal uint id;

        Texture[] textures = new Texture[16];

        public RenderTarget()
        {
            id = GL.GenFramebuffer();
        }
        public RenderTarget(Texture texture) : this()
        {
            SetTexture(0, texture);
        }
        public RenderTarget(Texture[] textures) : this()
        {
            for (int i = 0; i < textures.Length; ++i)
                SetTexture(i, textures[i]);
        }

        protected override void Dispose()
        {
            GL.DeleteFramebuffer(id);
        }

        public Texture GetTexture(int n)
        {
            if (n < 0 || n >= textures.Length)
                throw new ArgumentOutOfRangeException(nameof(n));

            return textures[n];
        }

        public void SetTexture(int n, Texture texture)
        {
            if (n < 0 || n >= textures.Length)
                throw new ArgumentOutOfRangeException(nameof(n));

            Bind(this);
            textures[n] = texture;
            var col = (TextureAttachment)((uint)TextureAttachment.Color0 + n);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, col, TextureTarget.Texture2D, texture.id, 0);
            drawBuffers[0] = DrawBuffer.Color0;
            GL.DrawBuffers(1, drawBuffers);
        }

        internal static void Bind(RenderTarget buffer)
        {
            if (binded != buffer)
            {
                binded = buffer;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, buffer != null ? buffer.id : 0);
            }
        }

        internal static void SetTarget(RenderTarget buffer)
        {
            Bind(buffer);
            if (buffer != target)
            {
                target = buffer;
                /*if (buffer != null)
                {
                    //Pipe each color output into its corresponding texture
                    int num = 0;
                    for (uint i = 0; i < drawBuffers.Length; ++i)
                        if (buffer.textures[i] != null)
                            drawBuffers[num++] = (DrawBuffer)((uint)DrawBuffer.Color0 + i);
                    GL.DrawBuffers(num, drawBuffers);
                }
                else
                {
                    //Pipe the first color output onto the back buffer
                    drawBuffers[0] = DrawBuffer.Back;
                    GL.DrawBuffers(1, drawBuffers);
                }*/
            }
        }
    }
}
