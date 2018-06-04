using System;
using System.Collections.Generic;
namespace Rise.Gui
{
    public class GuiElement
    {
        public event Action OnAdded;
        public event Action OnRemoved;
        public event Action OnAddedToView;
        public event Action OnRemovedFromView;

        public event Action<DrawBatch2D> OnRender;

        public event Action<MouseButton> OnMouseDown;
        public event Action<MouseButton> OnMouseUp;
        public event Action<MouseButton> OnClick;

        public event Action OnMouseMove;
        public event Action OnMouseEnter;
        public event Action OnMouseExit;
        public event Action OnScroll;

        public event Action OnLayout;

        public event Action OnDragStart;
        public event Action OnDragUpdate;
        public event Action OnDragEnd;
        public event Action<GuiElement> OnDrop;

        public GuiView View { get; internal set; }
        public GuiContainer Parent { get; private set; }

        public string Name = null;

        bool enabled = true;
        public bool MouseEnabled = true;

        //The "size" of an element is its dimensions when unaffected by flexing/etc.
        int sizeX;
        int sizeY;

        //Flexed elements will stretch to fill their parent container
        bool flexX = true;
        bool flexY = true;

        //This is the actual element dimensions after flexing/etc. is applied
        internal RectangleI rect;

        public GuiElement(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
        }

        public override string ToString()
        {
            if (Name != null)
                return $"{GetType().Name}: {Name}";
            return GetType().Name;
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    UpdateLayout();
                }
            }
        }

        public int SizeX
        {
            get { return sizeX; }
            set
            {
                if (sizeX != value)
                {
                    sizeX = value;
                    UpdateLayout();
                }
            }
        }

        public int SizeY
        {
            get { return sizeY; }
            set
            {
                if (sizeY != value)
                {
                    sizeY = value;
                    UpdateLayout();
                }
            }
        }

        public bool FlexX
        {
            get { return flexX; }
            set
            {
                if (flexX != value)
                {
                    flexX = value;
                    UpdateLayout();
                }
            }
        }

        public bool FlexY
        {
            get { return flexY; }
            set
            {
                if (flexY != value)
                {
                    flexY = value;
                    UpdateLayout();
                }
            }
        }

        public bool IsDragging
        {
            get { return View != null && View.Dragging == this; }
        }

        public void UpdateLayout()
        {
            View?.InvalidateLayout();
        }

        internal virtual void DoSize()
        {
            rect.W = sizeX;
            rect.H = sizeY;
        }

        internal virtual void DoLayout()
        {
            OnLayout?.Invoke();
        }

        internal void DoAdded(GuiContainer parent)
        {
            Parent = parent;
            OnAdded?.Invoke();

            if (parent is GuiView)
                DoAddedToView((GuiView)parent);
            else if (parent.View != null)
                DoAddedToView(parent.View);
        }

        internal void DoRemoved()
        {
            Parent = null;
            if (View != null)
            {
                View = null;
                DoRemovedFromView();
            }
            OnRemoved?.Invoke();
        }

        internal virtual void DoAddedToView(GuiView view)
        {
            View = view;
            OnAddedToView?.Invoke();
        }

        internal virtual void DoRemovedFromView()
        {
            View = null;
            OnRemovedFromView?.Invoke();
        }
        
        internal virtual void DoRender(DrawBatch2D batch)
        {
            OnRender?.Invoke(batch);
        }

        internal virtual void DoMouseDown(MouseButton button)
        {
            if (MouseEnabled)
            {
                OnMouseDown?.Invoke(button);
                View.Dragging = this;
                switch (button)
                {
                    case MouseButton.Left: View.leftClick = this; break;
                    case MouseButton.Middle: View.middleClick = this; break;
                    case MouseButton.Right: View.rightClick = this; break;
                }
            }
        }

        internal virtual void DoMouseUp(MouseButton button)
        {
            if (MouseEnabled)
                OnMouseUp?.Invoke(button);
        }

        internal virtual void DoClick(MouseButton button)
        {
            Parent?.DoClick(button);
            if (MouseOver)
                OnClick?.Invoke(button);
        }

        internal virtual void DoMouseMove()
        {
            if (MouseEnabled)
                OnMouseMove?.Invoke();
        }

        internal virtual void DoMouseEnter()
        {
            if (MouseEnabled)
                OnMouseEnter?.Invoke();
        }

        internal virtual void DoMouseExit()
        {
            if (MouseEnabled)
                OnMouseExit?.Invoke();
        }

        internal virtual void DoScroll()
        {
            if (MouseEnabled)
                OnScroll?.Invoke();
        }

        internal virtual void DoDragStart()
        {
            OnDragStart?.Invoke();
        }

        internal virtual void DoDragUpdate()
        {
            OnDragUpdate?.Invoke();
        }

        internal virtual void DoDragEnd()
        {
            OnDragEnd?.Invoke();
        }

        internal virtual void DoDrop(GuiElement elem)
        {
            OnDrop?.Invoke(elem);
        }

        public GuiContainer Root
        {
            get
            {
                if (View != null)
                    return View;
                var p = Parent;
                while (p.Parent != null)
                    p = p.Parent;
                return p;
            }
        }

        public bool MouseOver { get { return rect.Contains(Mouse.Position); } }
        public bool MouseOverPrev { get { return rect.Contains(Mouse.LastPosition); } }
        public RectangleI Rect { get { return rect; } }
        public int RectX { get { return rect.X; } }
        public int RectY { get { return rect.Y; } }
        public int RectW { get { return rect.W; } }
        public int RectH { get { return rect.H; } }
    }
}
