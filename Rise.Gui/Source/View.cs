using System;
using System.Collections.Generic;
using Rise;
namespace Rise.Gui
{
    public class View : Container
    {
        public event Action OnUpdate;

        bool updateLayout = true;
        DrawBatch2D batch = new DrawBatch2D();
        Texture2D texture;
        RenderTarget target;
        Material material;

        public View(int width, int height) : base(width, height)
        {
            rect = new RectangleI(width, height);
            WidthOfChildren = false;
            HeightOfChildren = false;
            FlexX = false;
            FlexY = false;
            View = this;
            texture = new Texture2D(width, height, TextureFormat.RGBA);
            target = new RenderTarget(texture);
            material = new Material(new Shader(Shader.Basic2D));
        }

        internal void InvalidateLayout()
        {
            updateLayout = true;
        }

        public void SetSize(int width, int height)
        {
            SizeX = width;
            SizeY = height;
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
            }

            if (hoverCurr)
            {
                if (Mouse.LeftPressed)
                    DoMouseDown(MouseButton.Left);
                else if (Mouse.MiddlePressed)
                    DoMouseDown(MouseButton.Middle);
                else if (Mouse.RightPressed)
                    DoMouseDown(MouseButton.Right);

                if (Mouse.LeftReleased)
                    DoMouseUp(MouseButton.Left);
                else if (Mouse.MiddleReleased)
                    DoMouseUp(MouseButton.Middle);
                else if (Mouse.RightReleased)
                    DoMouseUp(MouseButton.Right);

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

            batch.Begin(target, material, Matrix4x4.Identity, BlendMode.Premultiplied);
            DoRender(batch);
            batch.End();
        }
    }
}
