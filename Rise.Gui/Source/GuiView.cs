using System;
using System.Collections.Generic;
using Rise;
namespace Rise.Gui
{
    public class GuiView : GuiContainer
    {
        public event Action OnUpdate;

        public Texture2D Texture { get; private set; }
        public GuiElement Dragging { get; internal set; }

        bool updateLayout = true;
        DrawBatch2D batch = new DrawBatch2D();
        RenderTarget target;
        Material material;

        internal GuiElement leftClick;
        internal GuiElement middleClick;
        internal GuiElement rightClick;

        public GuiView(Shader shader, int width, int height, LayoutMode layout) : base(width, height, layout)
        {
            rect = new RectangleI(width, height);
            FlexX = FlexY = false;
            View = this;
            Texture = new Texture2D(width, height, TextureFormat.RGBA);
            target = new RenderTarget(Texture);
            material = new Material(shader);
        }

        internal void InvalidateLayout()
        {
            updateLayout = true;
        }

        public void SetSize(int width, int height)
        {
            SizeX = width;
            SizeY = height;
            target.Resize(width, height);
        }

        //TODO: Maybe user should send these events to the view manually,
        //that way they'd be able to pick and choose/control its input.
        //They could also "simulate" interacting with UI, which would be cool.
        public void Update()
        {
            bool hoverPrev = MouseOverPrev;
            bool hoverCurr = MouseOver;

            if (Mouse.Position != Mouse.LastPosition)
            {
                DoMouseMove();
                Dragging?.DoDragUpdate();
            }

            if (hoverCurr)
            {
                if (Mouse.LeftPressed)
                {
                    DoMouseDown(MouseButton.Left);
                    Dragging?.DoDragStart();
                }
                else if (Mouse.MiddlePressed)
                    DoMouseDown(MouseButton.Middle);
                else if (Mouse.RightPressed)
                    DoMouseDown(MouseButton.Right);

                if (Mouse.LeftReleased)
                {
                    DoMouseUp(MouseButton.Left);
                    Dragging?.DoDragEnd();
                    DoDrop(Dragging);
                    Dragging = null;
                    leftClick?.DoClick(MouseButton.Left);
                    leftClick = null;
                }
                else if (Mouse.MiddleReleased)
                {
                    DoMouseUp(MouseButton.Middle);
                    middleClick?.DoClick(MouseButton.Middle);
                    middleClick = null;
                }
                else if (Mouse.RightReleased)
                {
                    DoMouseUp(MouseButton.Right);
                    rightClick?.DoClick(MouseButton.Right);
                    rightClick = null;
                }

                if (Mouse.Scroll != Point2.Zero)
                    DoScroll();
            }

            OnUpdate?.Invoke();
        }

        public void Render()
        {
            //Update layout
            while (updateLayout)
            {
                updateLayout = false;
                DoSize();
                DoLayout();
            }

            target.Clear(Color4.Transparent);
            batch.Begin(target, material, Matrix4x4.Identity, BlendMode.Premultiplied);
            DoRender(batch);
            batch.End();
        }
    }
}
