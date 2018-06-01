using System;
using Rise;
namespace Rise.Gui
{
    public enum FillMode
    {
        ShrinkOnly,
        MaintainAspectRatio,
        StretchToFill
    }

    public class Image : Element
    {
        SubTexture texture;

        public Color4 Color = Color4.White;
        public FillMode FillMode = FillMode.ShrinkOnly;
        public AlignX AlignX = AlignX.Center;
        public AlignY AlignY = AlignY.Center;

        public Image(SubTexture texture)
            : base(texture.Width, texture.Height, true, true)
        {
            this.texture = texture;
        }

        public SubTexture Texture
        {
            get { return texture; }
            set
            {
                if (texture != value)
                {
                    texture = value;
                    UpdateLayout();
                }
            }
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            if (Color.A > 0)
            {
                //TODO: dumb to be calculating the alignment stuff here, move to layout phase
                var pos = new Rectangle(rect.X, rect.Y, texture.Width, texture.Height);

                if (FillMode == FillMode.ShrinkOnly)
                {
                    float s = Math.Min(1.0f, rect.W / (float)pos.W);
                    s = Math.Min(s, rect.H / (float)pos.H);
                    pos.W = (int)(pos.W * s);
                    pos.H = (int)(pos.H * s);
                }
                else if (FillMode == FillMode.MaintainAspectRatio)
                {
                    float s = rect.W / (float)pos.W;
                    s = Math.Min(s, rect.H / (float)pos.H);
                    pos.W = (int)(pos.W * s);
                    pos.H = (int)(pos.H * s);
                }
                else if (FillMode == FillMode.StretchToFill)
                {
                    pos.W = rect.W;
                    pos.H = rect.H;
                }
                else
                    throw new Exception("Unhandled fill mode.");

                if (AlignX == AlignX.Center)
                    pos.X = (int)(rect.CenterX - (pos.W * 0.5f));
                else if (AlignX == AlignX.Right)
                    pos.X = rect.MaxX - pos.W;

                if (AlignY == AlignY.Center)
                    pos.Y = (int)(rect.CenterY - (pos.H * 0.5f));
                else if (AlignY == AlignY.Bottom)
                    pos.Y = rect.MaxY - pos.H;

                batch.DrawImage(texture, ref pos, Color);
            }

            base.DoRender(batch);
        }
    }
}
