using System;
using System.Collections.Generic;
namespace Rise.Gui
{
    public abstract class Element
    {
        public event Action OnAdded;
        public event Action OnRemoved;
        public event Action OnAddedToView;
        public event Action OnRemovedFromView;

        public event Action<DrawBatch2D> OnRender;

        public event Action<MouseButton> OnMouseDown;
        public event Action<MouseButton> OnMouseUp;
        public event Action OnMouseMove;
        public event Action OnMouseEnter;
        public event Action OnMouseExit;
        public event Action OnScroll;
        public event Action OnLayout;

        public View View { get; internal set; }
        public Container Parent { get; private set; }

        public string Name = null;

        bool enabled = true;
        public bool MouseEnabled = true;

        int sizeX;
        int sizeY;
        bool flexX = true;
        bool flexY = true;
        internal RectangleI rect;

        public Element(int sizeX, int sizeY, bool flexX, bool flexY)
        {
            if (sizeX == 0)
                throw new ArgumentException("Must be > 0", nameof(sizeX));
            if (sizeY == 0)
                throw new ArgumentException("Must be > 0", nameof(sizeY));

            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.flexX = flexX;
            this.flexY = flexY;
        }

        public override string ToString()
        {
            if (Name != null)
                return $"{GetType().Name}: {Name}";
            else
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
                    if (this is Container)
                        ((Container)this).WidthOfChildren = false;
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
                    if (this is Container)
                        ((Container)this).HeightOfChildren = false;
                }
            }
        }

        public void SetFlex(bool x, bool y)
        {
            FlexX = x;
            FlexY = y;
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

        internal void DoAdded(Container parent)
        {
            Parent = parent;
            OnAdded?.Invoke();

            if (parent is View)
                DoAddedToView((View)parent);
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

        internal virtual void DoAddedToView(View view)
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
                OnMouseDown?.Invoke(button);
        }

        internal virtual void DoMouseUp(MouseButton button)
        {
            if (MouseEnabled)
                OnMouseUp?.Invoke(button);
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

        public Container Root
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

        public int RectX { get { return rect.X; } }
        public int RectY { get { return rect.Y; } }
        public int RectW { get { return rect.W; } }
        public int RectH { get { return rect.H; } }

        public bool MouseOver
        {
            get { return rect.Contains(Mouse.Position); }
        }

        public bool MouseOverPrev
        {
            get { return rect.Contains(Mouse.LastPosition); }
        }

        public RectangleI Rect
        {
            get { return rect; }
        }
    }
}
