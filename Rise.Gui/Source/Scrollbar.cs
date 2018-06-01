using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rise;
namespace Rise.Gui
{
    public class Scrollbar : Element
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
        public Container Container { get; private set; }

        public int Padding = 2;
        public int HandleBorder;
        public bool RoundedHandle = true;
        public int RoundedHandleCurveSegments;

        public Color4 BackgroundColor = Color4.Transparent;
        public Color4 HandleColor = Color4.Grey;
        public Color4 HandleBorderColor = Color4.Transparent;
        public Color4 HoverBackgroundColor = Color4.Transparent;
        public Color4 HoverHandleColor = Color4.Black;
        public Color4 HoverHandleBorderColor = Color4.Transparent;

        public Scrollbar(LayoutMode layout, Container container, int thickness) 
            : base(thickness, thickness, layout == LayoutMode.Horizontal, layout == LayoutMode.Vertical)
        {
            Layout = layout;
            Container = container;
            RoundedHandleCurveSegments = thickness / 2;
        }

        public RectangleI Handle
        {
            get
            {
                var handle = rect.Inflated(-Padding);
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
                return handle;
            }
        }

        public bool MouseOverHandle
        {
            get { return Handle.Contains(Mouse.Position); }
        }

        internal override void DoMouseDown(MouseButton button)
        {
            if (MouseOverHandle)
            {
                dragging = true;
                dragOffset = new Point2(Mouse.X - Handle.X, Mouse.Y - Handle.Y);
            }

            base.DoMouseDown(button);
        }

        internal override void DoMouseMove()
        {
            if (dragging)
            {
                var handle = Handle;
                if (Layout == LayoutMode.Horizontal)
                    Container.ScrollPercentX = (Mouse.X - dragOffset.X - RectX - Padding) / (float)(RectW - Padding * 2 - handle.W);
                else
                    Container.ScrollPercentY = (Mouse.Y - dragOffset.Y - RectY - Padding) / (float)(RectH - Padding * 2 - handle.H);
            }

            base.DoMouseMove();
        }

        internal override void DoMouseUp(MouseButton button)
        {
            dragging = false;
            base.DoMouseUp(button);
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            Color4 colBg, colHandle, colBorder;
            if (dragging || MouseOverHandle)
            {
                colBg = HoverBackgroundColor;
                colHandle = HoverHandleColor;
                colBorder = HoverHandleBorderColor;
            }
            else
            {
                colBg = BackgroundColor;
                colHandle = HandleColor;
                colBorder = HandleBorderColor;
            }
            
            if (colBg.A > 0)
                batch.DrawRect(rect.X, rect.Y, rect.W, rect.H, colBg);

            var handle = Handle;
            if (HandleBorder > 0 && colBorder.A > 0)
                DrawHandle(batch, handle, colBorder);
            DrawHandle(batch, handle.Inflated(-HandleBorder), colHandle);

            base.DoRender(batch);
        }

        void DrawHandle(DrawBatch2D batch, RectangleI handle, Color4 color)
        {
            /*if (RoundedHandle)
            {
                if (Layout == LayoutMode.Horizontal)
                    Draw2D.HorizontalRoundedRect(handle, color, RoundedHandleCurveSegments);
                else
                    Draw2D.VerticalRoundedRect(handle, color, RoundedHandleCurveSegments);
            }
            else
                Draw2D.Rect(handle, color);*/
            batch.DrawRect(handle.X, handle.Y, handle.W, handle.H, color);
        }

    }
}
