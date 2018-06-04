using System;
namespace Rise.Gui
{
    public class GuiRect : GuiElement
    {
        public Color4 Color;

        public GuiRect(int width, int height, Color4 color)
            : base(width, height)
        {
            Color = color;
            FlexX = width == 0;
            FlexY = height == 0;
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            batch.DrawRect(RectX, RectY, RectW, RectH, Color);
        }
    }
}
