using System;
namespace Rise.Gui
{
    public class GuiLabel : GuiElement
    {
        public Color4 Color = Color4.White;

        AtlasFont font;
        string text;
        AlignX alignX = AlignX.Center;
        AlignY alignY = AlignY.Center;
        Vector2 pos;

        public GuiLabel(AtlasFont font, string text, Color4 color)
            : base(font.GetWidth(ref text), font.Height)
        {
            this.font = font;
            this.text = text;
            Color = color;
        }

        public AtlasFont Font
        {
            get { return font; }
            set
            {
                if (font != value)
                {
                    font = value;
                    SizeX = font.GetWidth(ref text);
                    SizeY = font.Height;
                }
            }
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    SizeX = font.GetWidth(ref text);
                }
            }
        }

        internal override void DoLayout()
        {
            if (alignX == AlignX.Left)
                pos.X = rect.X;
            else if (alignX == AlignX.Right)
                pos.X = rect.X - SizeX;
            else
                pos.X = rect.CenterX - SizeX / 2;

            if (alignY == AlignY.Top)
                pos.Y = rect.Y;
            else if (alignY == AlignY.Bottom)
                pos.Y = rect.Y - SizeY;
            else
                pos.Y = rect.CenterY - SizeY / 2;

            base.DoLayout();
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            batch.DrawText(font, ref text, pos, Color);

            base.DoRender(batch);
        }
    }
}
