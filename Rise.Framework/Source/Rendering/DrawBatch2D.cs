using System;
namespace Rise
{
    //TODO: matrix stack? probably... pretty handy to have
    public class DrawBatch2D
    {
        //Vertices for rendering images
        static Vertex2D v0 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 255, 0, 0);
        static Vertex2D v1 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 255, 0, 0);
        static Vertex2D v2 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 255, 0, 0);
        static Vertex2D v3 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 255, 0, 0);

        //Vertices for rendering images
        static Vertex2D w0 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 0, 255, 0);
        static Vertex2D w1 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 0, 255, 0);
        static Vertex2D w2 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 0, 255, 0);
        static Vertex2D w3 = new Vertex2D(Vector2.Zero, Vector2.One, Color4.White, 0, 255, 0);

        //Vertices for rendering shapes
        static Vertex2D c0 = new Vertex2D(Vector2.Zero, Vector2.Zero, Color4.White, 0, 0, 255);
        static Vertex2D c1 = new Vertex2D(Vector2.Zero, Vector2.Zero, Color4.White, 0, 0, 255);
        static Vertex2D c2 = new Vertex2D(Vector2.Zero, Vector2.Zero, Color4.White, 0, 0, 255);
        static Vertex2D c3 = new Vertex2D(Vector2.Zero, Vector2.Zero, Color4.White, 0, 0, 255);

        //The default material and texture used when nothing is assigned
        static Texture2D defaultTexture;
        static Material defaultMaterial;
        static string matrixName = "Matrix";
        static string textureName = "Texture";

        //Rendering state
        bool rendering;
        Mesh2D mesh;
        int matrixLoc;
        int textureLoc;
        DrawCall draw;
        Texture currTexture;
        Matrix3x2 modelMatrix;
        Matrix4x4 projMatrix;
        Matrix4x4 viewMatrix;
        Matrix4x4 viewProjMatrix;

        RectangleI[] clipRects = new RectangleI[4];
        int clipIndex;

        public DrawBatch2D()
        {
            if (defaultTexture == null)
            {
                defaultTexture = new Texture2D(1, 1, new Color4[] { Color4.White });
                defaultMaterial = new Material(new Shader(Shader.Basic2D));
            }

            mesh = new Mesh2D();
            draw.Mesh = mesh;
        }

        public void Begin(RenderTarget target, Material material, Matrix4x4 matrix, BlendMode blendMode)
        {
            if (rendering)
                throw new Exception("Must call End() before you can call Begin() again.");
            
            rendering = true;
            draw.Target = target;
            modelMatrix = Matrix3x2.Identity;
            viewMatrix = matrix;
            draw.Blend = true;
            draw.BlendMode = blendMode;
            currTexture = defaultTexture;
            draw.Material = null;
            SetMaterial(material ?? defaultMaterial);
            clipIndex = -1;
            draw.Clip = false;
        }
        public void Begin()
        {
            Begin(null, null, Matrix4x4.Identity, BlendMode.Premultiplied);
        }

        public void End()
        {
            if (!rendering)
                throw new Exception("Must call Begin() before you call End().");

            Flush();
            rendering = false;
        }

        public void Flush()
        {
            if (mesh.IndexCount > 0)
            {
                mesh.Update();
                draw.Perform(PrimitiveType.Triangles);
                mesh.Clear();
            }
        }

        void UpdateMatrix()
        {
            //The projection matrix is always a full-size orthographic view of the target
            if (draw.Target != null)
                Matrix4x4.CreateOrthographic(draw.Target.Width, draw.Target.Height, -1f, 1f, out projMatrix);
            else
                Matrix4x4.CreateOrthographic(Screen.Width, Screen.Height, -1f, 1f, out projMatrix);

            //Calculate the final matrix and upload it
            Matrix4x4.Multiply(ref viewMatrix, ref projMatrix, out viewProjMatrix);
            draw.Material.SetMatrix4x4(matrixLoc, ref viewProjMatrix);
        }

        public void PushClipRect(ref RectangleI rect)
        {
            Flush();

            ++clipIndex;
            if (clipIndex == clipRects.Length)
                Array.Resize(ref clipRects, clipIndex * 2);
            clipRects[clipIndex] = rect;
            draw.Clip = true;
            draw.ClipRect = rect;
        }
        public void PushClipRect(RectangleI rect)
        {
            Flush();

            PushClipRect(ref rect);
        }

        public void PopClipRect()
        {
            if (clipIndex >= 0)
            {
                --clipIndex;
                if (clipIndex >= 0)
                    draw.ClipRect = clipRects[clipIndex];
                else
                    draw.Clip = false;
            }
            else
                throw new Exception("No clip rects to pop.");
        }

        public void SetTarget(RenderTarget target)
        {
            if (draw.Target != target)
            {
                Flush();
                draw.Target = target;
                UpdateMatrix();
            }
        }

        public void SetMaterial(Material material)
        {
            if (draw.Material != material)
            {
                Flush();
                draw.Material = material ?? defaultMaterial;

                //The material might have different matrix/texture uniform locations
                matrixLoc = material.GetIndex(ref matrixName);
                textureLoc = material.GetIndex(ref textureName);

                //Update the uniforms
                material.SetTexture(textureLoc, currTexture);
                UpdateMatrix();
            }
        }

        public void SetViewMatrix(ref Matrix4x4 matrix)
        {
            Flush();
            viewMatrix = matrix;

            //When the matrix changes, we have to re-calculate the final matrix
            UpdateMatrix();
            draw.Material.SetMatrix4x4(matrixLoc, ref viewProjMatrix);
        }
        public void SetViewMatrix(Matrix4x4 matrix)
        {
            SetViewMatrix(ref matrix);
        }

        public void SetMatrix(ref Matrix3x2 matrix)
        {
            matrix.CopyTo(out modelMatrix);
        }
        public void SetMatrix(Matrix3x2 matrix)
        {
            SetMatrix(ref matrix);
        }

        public void SetBlendMode(BlendMode blendMode)
        {
            if (!draw.BlendMode.Equals(ref blendMode))
            {
                Flush();
                draw.BlendMode = blendMode;
            }
        }

        void SetTexture(Texture texture)
        {
            if (currTexture != texture)
            {
                Flush();
                currTexture = texture;
                draw.Material.SetTexture(textureLoc, currTexture);
            }
        }

        public void DrawRect(float x, float y, float w, float h, Color4 color)
        {
            c0.Pos = modelMatrix.TransformPoint(x, y);
            c1.Pos = modelMatrix.TransformPoint(x + w, y);
            c2.Pos = modelMatrix.TransformPoint(x + w, y + h);
            c3.Pos = modelMatrix.TransformPoint(x, y + h);
            
            c0.Col = c1.Col = c2.Col = c3.Col = color;

            mesh.AddQuad(ref c0, ref c1, ref c2, ref c3);
        }
        public void DrawRect(ref Rectangle rect, Color4 color)
        {
            c0.Pos = modelMatrix.TransformPoint(rect.TopLeft);
            c1.Pos = modelMatrix.TransformPoint(rect.TopRight);
            c2.Pos = modelMatrix.TransformPoint(rect.BottomRight);
            c3.Pos = modelMatrix.TransformPoint(rect.BottomLeft);

            c0.Col = c1.Col = c2.Col = c3.Col = color;

            mesh.AddQuad(ref c0, ref c1, ref c2, ref c3);
        }
        public void DrawRect(Rectangle rect, Color4 color)
        {
            DrawRect(ref rect, color);
        }

        public void DrawQuad(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color4 color)
        {
            c0.Pos = modelMatrix.TransformPoint(a);
            c1.Pos = modelMatrix.TransformPoint(b);
            c2.Pos = modelMatrix.TransformPoint(c);
            c3.Pos = modelMatrix.TransformPoint(d);

            c0.Col = c1.Col = c2.Col = c3.Col = color;

            mesh.AddQuad(ref c0, ref c1, ref c2, ref c3);
        }
        public void DrawQuad(ref Quad quad, Color4 color)
        {
            c0.Pos = modelMatrix.TransformPoint(quad.A);
            c1.Pos = modelMatrix.TransformPoint(quad.B);
            c2.Pos = modelMatrix.TransformPoint(quad.C);
            c3.Pos = modelMatrix.TransformPoint(quad.D);

            c0.Col = c1.Col = c2.Col = c3.Col = color;

            mesh.AddQuad(ref c0, ref c1, ref c2, ref c3);
        }
        public void DrawQuad(Quad quad, Color4 color)
        {
            DrawQuad(ref quad, color);
        }

        public void DrawLine(Vector2 a, Vector2 b, float width, Color4 color)
        {
            var normal = (b - a).Normalized;
            var perp = new Vector2(normal.Y, -normal.X) * width * 0.5f;

            c0.Pos = modelMatrix.TransformPoint(a + perp);
            c1.Pos = modelMatrix.TransformPoint(b + perp);
            c2.Pos = modelMatrix.TransformPoint(b - perp);
            c3.Pos = modelMatrix.TransformPoint(a - perp);

            c0.Col = c1.Col = c2.Col = c3.Col = color;

            mesh.AddQuad(ref c0, ref c1, ref c2, ref c3);
        }
        
        public void DrawTexture(Texture texture, Vector2 position, Color4 color)
        {
            SetTexture(texture);

            v0.Pos = modelMatrix.TransformPoint(position.X, position.Y);
            v1.Pos = modelMatrix.TransformPoint(position.X + texture.Width, position.Y);
            v2.Pos = modelMatrix.TransformPoint(position.X + texture.Width, position.Y + texture.Height);
            v3.Pos = modelMatrix.TransformPoint(position.X, position.Y + texture.Height);
            
            v0.Tex.X = v0.Tex.Y =  v1.Tex.Y = v3.Tex.X = 0f;
            v1.Tex.X = v2.Tex.X = v2.Tex.Y = v3.Tex.Y = 1f;

            v0.Col = v1.Col = v2.Col = v3.Col = color;

            mesh.AddQuad(ref v0, ref v1, ref v2, ref v3);
        }

        public void DrawTextureWashed(Texture texture, Vector2 position, Color4 color)
        {
            SetTexture(texture);

            w0.Pos = modelMatrix.TransformPoint(position.X, position.Y);
            w1.Pos = modelMatrix.TransformPoint(position.X + texture.Width, position.Y);
            w2.Pos = modelMatrix.TransformPoint(position.X + texture.Width, position.Y + texture.Height);
            w3.Pos = modelMatrix.TransformPoint(position.X, position.Y + texture.Height);

            w0.Tex.X = w0.Tex.Y = w1.Tex.Y = w3.Tex.X = 0f;
            w1.Tex.X = w2.Tex.X = w2.Tex.Y = w3.Tex.Y = 1f;

            w0.Col = w1.Col = w2.Col = w3.Col = color;

            mesh.AddQuad(ref w0, ref w1, ref w2, ref w3);
        }
        
        public void DrawImage(SubTexture image, Vector2 position, Color4 color)
        {
            SetTexture(image.Texture);

            var pos = new Vector2(position.X + image.OffsetX, position.Y + image.OffsetY);
            v0.Pos = modelMatrix.TransformPoint(pos);
            v1.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth, pos.Y);
            v2.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth, pos.Y + image.TrimHeight);
            v3.Pos = modelMatrix.TransformPoint(pos.X, pos.Y + image.TrimHeight);
            
            image.GetUVs(out v0.Tex, out v1.Tex, out v2.Tex, out v3.Tex);
            v0.Col = v1.Col = v2.Col = v3.Col = color;

            mesh.AddQuad(ref v0, ref v1, ref v2, ref v3);
        }
        public void DrawImage(SubTexture image, ref Rectangle position, Color4 color)
        {
            SetTexture(image.Texture);

            float sx = position.W / image.Width;
            float sy = position.H / image.Height;

            var pos = new Vector2(position.X + image.OffsetX * sx, position.Y + image.OffsetY * sy);
            v0.Pos = modelMatrix.TransformPoint(pos);
            v1.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth * sx, pos.Y);
            v2.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth * sx, pos.Y + image.TrimHeight * sy);
            v3.Pos = modelMatrix.TransformPoint(pos.X, pos.Y + image.TrimHeight * sy);

            image.GetUVs(out v0.Tex, out v1.Tex, out v2.Tex, out v3.Tex);
            v0.Col = v1.Col = v2.Col = v3.Col = color;

            mesh.AddQuad(ref v0, ref v1, ref v2, ref v3);
        }
        public void DrawImage(SubTexture image, Rectangle position, Color4 color)
        {
            DrawImage(image, ref position, color);
        }

        public void DrawImageWashed(AtlasImage image, Vector2 position, Color4 color)
        {
            SetTexture(image.Atlas.Texture);

            var pos = new Vector2(position.X + image.OffsetX, position.Y + image.OffsetY);
            w0.Pos = modelMatrix.TransformPoint(pos);
            w1.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth, pos.Y);
            w2.Pos = modelMatrix.TransformPoint(pos.X + image.TrimWidth, pos.Y + image.TrimHeight);
            w3.Pos = modelMatrix.TransformPoint(pos.X, pos.Y + image.TrimHeight);

            image.GetUVs(out w0.Tex, out w1.Tex, out w2.Tex, out w3.Tex);
            w0.Col = w1.Col = w2.Col = w3.Col = color;

            mesh.AddQuad(ref w0, ref w1, ref w2, ref w3);
        }

        public void DrawText(AtlasFont font, string text, Vector2 position, Color4 color)
        {
            var pos = position;

            AtlasChar prev = null;
            AtlasChar chr;
            for (int i = 0; i < text.Length; ++i)
            {
                chr = font.GetChar(text[i]);

                if (prev != null)
                    pos.X += prev.GetKerning(chr.Char);

                if (chr.Image != null)
                    DrawImage(chr.Image, pos, color);
                
                pos.X += chr.Advance;
                prev = chr;
            }
        }
    }
}
