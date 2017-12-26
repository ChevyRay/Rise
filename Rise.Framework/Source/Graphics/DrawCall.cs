﻿using System;
using Rise.OpenGL;
namespace Rise
{
    public struct DrawCall
    {
        static DrawCall state;

        public RenderTarget Target;
        public Material Material;
        public Mesh Mesh;
        public bool Blend;
        public BlendMode BlendMode;
        public bool Clip;
        public RectangleI ClipRect;

        public DrawCall(Material material, Mesh mesh)
        {
            Target = null;
            Material = material;
            Mesh = mesh;
            Blend = false;
            BlendMode = default(BlendMode);
            Clip = false;
            ClipRect = default(RectangleI);
        }
        public DrawCall(RenderTarget target, Material material, Mesh mesh)
        {
            Target = target;
            Material = material;
            Mesh = mesh;
            Blend = false;
            BlendMode = default(BlendMode);
            Clip = false;
            ClipRect = default(RectangleI);
        }
        public DrawCall(RenderTarget target, Material material, Mesh mesh, BlendMode blendMode)
        {
            Target = target;
            Material = material;
            Mesh = mesh;
            Blend = true;
            BlendMode = blendMode;
            Clip = false;
            ClipRect = default(RectangleI);
        }
        public DrawCall(RenderTarget target, Material material, Mesh mesh, BlendMode blendMode, RectangleI clipRect)
        {
            Target = target;
            Material = material;
            Mesh = mesh;
            Blend = true;
            BlendMode = blendMode;
            Clip = true;
            ClipRect = clipRect;
        }

        public void SetBlendMode(BlendMode blendMode)
        {
            Blend = true;
            BlendMode = blendMode;
        }

        public void SetCliprect(RectangleI clipRect)
        {
            Clip = true;
            ClipRect = clipRect;
        }

        public void Clear(Color color)
        {
            RenderTarget.Bind(Target);
            GL.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            GL.Clear(BufferBit.Color | BufferBit.Depth);
        }

        internal static void Begin()
        {
            //Clear render state
            state = new DrawCall();
            RenderTarget.Bind(null);
            GL.Viewport(0, 0, Screen.DrawWidth, Screen.DrawHeight);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            GL.Disable(EnableCap.DepthTest);
            GL.UseProgram(0);
            GL.BindVertexArray(0);

            //Clear the screen
            GL.ClearColor(Screen.ClearColor.R / 255f, Screen.ClearColor.G / 255f, Screen.ClearColor.B / 255f, Screen.ClearColor.A / 255f);
            GL.Clear(BufferBit.Color | BufferBit.Depth);
        }

        internal static void End()
        {
            GL.UseProgram(0);
            GL.BindVertexArray(0);
        }

        public void Perform(Color clearColor)
        {
            RenderTarget.Bind(Target);
            GL.ClearColor(clearColor.R / 255f, clearColor.G / 255f, clearColor.B / 255f, clearColor.A / 255f);
            GL.Clear(BufferBit.Color | BufferBit.Depth);
            Perform();
        }
        public void Perform()
        {
            //Make sure the render target is binded
            RenderTarget.Bind(Target);

            //Set render target
            if (state.Target != Target)
            {
                state.Target = Target;
                if (Target != null)
                    GL.Viewport(0, 0, Target.Width, Target.Height);
                else
                    GL.Viewport(0, 0, Screen.DrawWidth, Screen.DrawHeight);
            }

            //Set blend mode
            if (Blend)
            {
                if (!state.Blend || !BlendMode.Equals(ref state.BlendMode))
                {
                    if (!state.Blend)
                    {
                        state.Blend = true;
                        GL.Enable(EnableCap.Blend);
                    }
                    state.BlendMode = BlendMode;
                    GL.BlendEquation(BlendMode.Eq);
                    GL.BlendFunc(BlendMode.Src, BlendMode.Dst);
                }

            }
            else if (state.Blend)
            {
                state.Blend = false;
                GL.Disable(EnableCap.Blend);
            }

            //Set clipping
            if (Clip)
            {
                if (!state.Clip)
                {
                    state.Clip = true;
                    GL.Enable(EnableCap.ScissorTest);
                }
                GL.Scissor(ClipRect.X, ClipRect.Y, ClipRect.W, ClipRect.H);
            }
            else if (state.Clip)
            {
                state.Clip = false;
                GL.Disable(EnableCap.ScissorTest);
            }

            //We need a material and a valid mesh in order to draw
            if (Mesh == null || Material == null || Mesh.uploadedIndexCount == 0 || Mesh.uploadedVertexCount == 0)
                return;

            //Sync the shader's state with the material
            bool uploadAll = false;
            if (state.Material != Material)
            {
                if (state.Material == null || state.Material.Shader != Material.Shader)
                {
                    GL.UseProgram(Material.Shader.id);
                    uploadAll = true;
                }
                state.Material = Material;
            }
            Material.Upload(uploadAll);

            //Bind the vertex array
            if (state.Mesh != Mesh)
            {
                GL.BindVertexArray(Mesh.vaoID);
                GL.BindBuffer(BufferTarget.Array, Mesh.arrayID);
                GL.BindBuffer(BufferTarget.ElementArray, Mesh.elementID);
                state.Mesh = Mesh;
            }

            //Draw the mesh triangles
            GL.DrawElements(DrawMode.Triangles, Mesh.uploadedIndexCount, IndexType.UnsignedInt, IntPtr.Zero);
        }
    }
}