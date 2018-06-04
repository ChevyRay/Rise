using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise;
namespace Rise.Gui
{
    public abstract class GuiScrollbar : GuiElement
    {
        bool dragging;
        Point2 dragOffset;

        public int Thickness
        {
            get { return (Layout == LayoutMode.Horizontal ? SizeY : SizeX); }
            set
            {
                if (Layout == LayoutMode.Horizontal)
                    SizeY = value;
                else
                    SizeX = value;
            }
        }

        public LayoutMode Layout { get; private set; }
        public GuiContainer Container { get; private set; }
        public int Padding = 2;

        RectangleI handle;

        public GuiScrollbar(LayoutMode layout, GuiContainer container, int thickness) 
            : base(thickness, thickness)
        {
            Layout = layout;
            FlexX = layout == LayoutMode.Horizontal;
            FlexY = layout == LayoutMode.Vertical;
            Container = container;
            OnLayout += UpdateHandle;
        }

        void UpdateHandle()
        {
            handle = rect.Inflated(-Padding);
            if (Layout == LayoutMode.Horizontal)
            {
                handle.W = (int)Math.Max(RectW / 4f, (Container.RectW / (float)Container.ContentRect.W) * RectW);
                handle.X = (int)(RectX + Padding + Container.ScrollPercentX * (RectW - handle.W - Padding * 2));
            }
            else
            {
                handle.H = (int)Math.Max(RectH / 4f, (Container.RectH / (float)Container.ContentRect.H) * RectH);
                handle.Y = (int)(RectY + Padding + Container.ScrollPercentY * (RectH - handle.H - Padding * 2));
            }
        }

        public RectangleI Handle
        {
            get { return handle; }
        }

        public bool MouseOverHandle
        {
            get { return handle.Contains(Mouse.Position); }
        }

        internal override void DoDragStart()
        {
            if (MouseOverHandle)
            {
                dragging = true;
                dragOffset = new Point2(Mouse.X - Handle.X, Mouse.Y - Handle.Y);
            }

            base.DoDragStart();
        }

        internal override void DoDragUpdate()
        {
            if (dragging)
            {
                if (Layout == LayoutMode.Horizontal)
                    Container.ScrollPercentX = (Mouse.X - dragOffset.X - RectX - Padding) / (float)(RectW - Padding * 2 - handle.W);
                else
                    Container.ScrollPercentY = (Mouse.Y - dragOffset.Y - RectY - Padding) / (float)(RectH - Padding * 2 - handle.H);

                UpdateHandle();
            }

            base.DoDragUpdate();
        }

        internal override void DoDragEnd()
        {
            dragging = false;
           
            base.DoDragEnd();
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            Render(batch, handle);
        }

        protected abstract void Render(DrawBatch2D batch, RectangleI handle);
    }
}
