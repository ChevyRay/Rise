using System;
using Rise.OpenGL;
namespace Rise
{
    public struct GraphicsState
    {
        public RenderTarget Target;
        public BlendMode? BlendMode;
        public Material Material;
        public Mesh Mesh;
    }

    public static class Graphics
    {
        static GraphicsState state;

        public static GraphicsState State
        {
            get { return state; }
        }

        internal static void Begin()
        {
            SetTarget(null);
            SetBlendMode(null);
            state.Material = null;
            state.Mesh = null;

            //Temp
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
        }

        internal static void End()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        public static void Clear(Color color)
        {
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(BufferBit.Color | BufferBit.Depth);
        }

        public static void SetTarget(RenderTarget target)
        {
            if (state.Target != target)
            {
                state.Target = target;
                RenderTarget.Bind(target);
                if (target != null)
                    GL.Viewport(0, 0, target.Width, target.Height);
                else
                    GL.Viewport(0, 0, Screen.DrawWidth, Screen.DrawHeight);
            }
        }

        public static void SetBlendMode(BlendMode? blendMode)
        {
            if (state.BlendMode != blendMode)
            {
                state.BlendMode = blendMode;
                if (blendMode.HasValue)
                {
                    if (!state.BlendMode.HasValue)
                        GL.Enable(EnableCap.Blend);

                    var blend = blendMode.Value;
                    GL.BlendEquation(blend.Eq);
                    GL.BlendFunc(blend.Src, blend.Dst);
                }
                else
                    GL.Disable(EnableCap.Blend);
            }
        }

        public static void Draw(ref GraphicsState state)
        {
            SetTarget(state.Target);
            SetBlendMode(state.BlendMode);
            Draw(state.Material, state.Mesh);
        }
        public static void Draw(GraphicsState state)
        {
            Draw(ref state);
        }

        //public static void Draw<V>(Material material, Mesh<V> mesh) where V : struct, ICopyable<V>
        public static void Draw(Material material, Mesh mesh)
        {
            //Just make sure the correct render target is always binded
            RenderTarget.Bind(state.Target);

            if (material == null)
                throw new ArgumentNullException(nameof(material));
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));

            //If the mesh is empty, don't draw anything
            if (mesh.uploadedIndexCount == 0 || mesh.uploadedVertexCount == 0)
                return;

            //Sync the shader's state with the material
            var uploadAll = false;
            if (state.Material != material)
            {
                if (state.Material == null || state.Material.Shader != material.Shader)
                {
                    GL.UseProgram(material.Shader.id);
                    uploadAll = true;
                }

                state.Material = material;
            }
            material.Upload(uploadAll);

            //Bind the vertex array
            if (state.Mesh != mesh)// || mesh.dirty)
            {
                GL.BindVertexArray(mesh.vaoID);
                GL.BindBuffer(BufferTarget.Array, mesh.arrayID);
                GL.BindBuffer(BufferTarget.ElementArray, mesh.elementID);

                state.Mesh = mesh;
                //mesh.dirty = false;
            }

            //Draw the mesh triangles
            GL.DrawElements(DrawMode.Triangles, mesh.uploadedIndexCount, IndexType.UnsignedInt, IntPtr.Zero);
        }

        public static void BeginScissor(ref RectangleI rect)
        {
            GL.Enable(EnableCap.ScissorTest);
            GL.Scissor(rect.X, rect.Y, rect.W, rect.H);
        }
        public static void BeginScissor(RectangleI rect)
        {
            BeginScissor(ref rect);
        }

        public static void EndScissor()
        {
            GL.Disable(EnableCap.ScissorTest);
        }
    }
}
