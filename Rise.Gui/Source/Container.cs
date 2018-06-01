﻿using System;
using System.Collections.Generic;
using Rise;
namespace Rise.Gui
{
    public class Container : Element
    {
        public Color4 BackgroundColor = Color4.Transparent;
        public bool MouseChildren = true;
        public bool ScrollableX = true;
        public bool ScrollableY = true;
        public bool AlwaysClip = false;

        List<Element> children = new List<Element>();

        LayoutMode layout;
        AlignX alignX = AlignX.Left;
        AlignY alignY = AlignY.Top;
        int spacing;
        int padLeft;
        int padRight;
        int padTop;
        int padBottom;
        Point2 scroll;

        bool widthOfChildren = true;
        bool heightOfChildren = true;
        int enabledChildren;
        RectangleI contentRect;

        public RectangleI ContentRect { get { return contentRect; } }

        public Container(int sizeX, int sizeY)
            : base(sizeX, sizeY, false, false)
        {

        }
        public Container() : this(16, 16)
        {

        }

        public int ScrollX
        {
            get { return scroll.X; }
            set
            {
                value = Calc.Clamp(value, rect.MaxX - contentRect.MaxX, rect.X - contentRect.X);
                if (scroll.X != value)
                {
                    scroll.X = value;
                    UpdateLayout();
                }
            }
        }

        public int ScrollY
        {
            get { return scroll.Y; }
            set
            {
                value = Calc.Clamp(value, rect.MaxY - contentRect.MaxY, rect.Y - contentRect.Y);
                if (scroll.Y != value)
                {
                    scroll.Y = value;
                    UpdateLayout();
                }
            }
        }
        
        public float ScrollPercentX
        {
            get { return scroll.X / (float)(rect.W - contentRect.W); }
            set { ScrollX = (int)(value * (rect.W - contentRect.W)); }
        }

        public float ScrollPercentY
        {
            get { return scroll.Y / (float)(rect.H - contentRect.H); }
            set { ScrollY = (int)(value * (rect.H - contentRect.H)); }
        }

        public bool WidthOfChildren
        {
            get { return widthOfChildren; }
            set
            {
                if (widthOfChildren != value)
                {
                    widthOfChildren = value;
                    UpdateLayout();
                    if (value)
                        FlexX = false;
                }
            }
        }

        public bool HeightOfChildren
        {
            get { return heightOfChildren; }
            set
            {
                if (heightOfChildren != value)
                {
                    heightOfChildren = value;
                    UpdateLayout();
                    if (value)
                        FlexY = false;
                }
            }
        }

        internal override void DoAddedToView(View view)
        {
            base.DoAddedToView(view);

            //When added to a view, notify all children of it as well
            foreach (var child in Children)
                child.DoAddedToView(view);
        }

        internal override void DoRemovedFromView()
        {
            base.DoRemovedFromView();

            UpdateLayout();

            //When removed from a view, notify all children of it as well
            foreach (var child in Children)
                child.DoRemovedFromView();
        }

        internal override void DoSize()
        {
            //Update size of children
            enabledChildren = 0;
            foreach (var child in Children)
            {
                if (child.Enabled)
                {
                    child.DoSize();
                    ++enabledChildren;
                }
            }

            //Update width
            if (widthOfChildren)
            {
                rect.W = 0;
                if (enabledChildren > 0)
                {
                    if (Layout == LayoutMode.Horizontal)
                    {
                        //Your width is all your children's height added up (flexible elements are ignored)
                        rect.W = (enabledChildren - 1) * spacing;
                        foreach (var child in Children)
                            if (child.Enabled)
                                rect.W += child.SizeX;
                    }
                    else
                    {
                        //Your width is your widest child's width
                        foreach (var child in Children)
                            if (child.Enabled)
                                rect.W = Math.Max(rect.W, child.SizeX);
                    }
                }
                rect.W += padLeft + padRight;
            }
            else
                rect.W = SizeX;

            //Update height
            if (heightOfChildren)
            {
                rect.H = 0;
                if (enabledChildren > 0)
                {
                    if (Layout == LayoutMode.Vertical)
                    {
                        //Your height is all your children's height added up (flexible elements are ignored)
                        rect.H = (enabledChildren - 1) * spacing;
                        foreach (var child in Children)
                            if (child.Enabled)
                                rect.H += child.SizeY;
                    }
                    else
                    {
                        //Your height is your tallest child's height
                        foreach (var child in Children)
                            if (child.Enabled)
                                rect.H = Math.Max(rect.H, child.SizeY);
                    }
                }
                rect.H += padTop + padBottom;
            }
            else
                rect.H = SizeY;
        }

        internal override void DoLayout()
        {
            contentRect = rect;

            if (Layout == LayoutMode.Horizontal)
            {
                //Calculate horizontal flex space and vertically position children
                int fixedW = padLeft + padRight + (enabledChildren - 1) * spacing;
                int flexW = fixedW;
                int flexCount = 0;
                foreach (var child in Children)
                {
                    if (child.Enabled)
                    {
                        //Calculate our flexible space
                        if (child.FlexX)
                            ++flexCount;
                        else
                            flexW += child.rect.W;
                        fixedW += child.rect.W;

                        //Align children vertically
                        if (child.FlexY)
                        {
                            child.rect.Y = rect.Y + padTop;
                            child.rect.H = rect.H - padTop - padBottom;
                        }
                        else if (alignY == AlignY.Top)
                            child.rect.Y = rect.Y + padTop;
                        else if (alignY == AlignY.Center)
                            child.rect.Y = rect.CenterY - child.rect.H / 2;
                        else
                            child.rect.Y = rect.MaxY - child.rect.H - padBottom;
                    }
                }

                //If we have no extra space to flex with, disable flexing
                flexW = (rect.W - flexW);
                if (flexW <= 0)
                    flexCount = 0;

                //Position and flex elements horizontally
                if (flexCount > 0)
                {
                    //Calculate the width of all the flexed elements
                    int extraW = flexW % flexCount;
                    flexW /= flexCount;

                    //No horizontal alignment if we're flex-fitting
                    int x = rect.X + padLeft;
                    foreach (var child in Children)
                    {
                        if (child.Enabled)
                        {
                            child.rect.X = x;
                            if (child.FlexX)
                            {
                                child.rect.W = flexW;
                                if (extraW > 0)
                                {
                                    ++child.rect.W;
                                    --extraW;
                                }
                            }
                            RectangleI.Conflate(ref contentRect, ref child.rect, out contentRect);
                            //child.DoLayout();
                            x += child.rect.W + spacing;
                        }
                    }
                }
                else
                {
                    //Horizontally align
                    int x;
                    if (alignX == AlignX.Left)
                        x = rect.X + padLeft;
                    else if (alignX == AlignX.Center)
                        x = rect.CenterX - fixedW / 2;
                    else
                        x = rect.MaxX - fixedW - padRight;

                    //Position elements horizontally
                    foreach (var child in Children)
                    {
                        if (child.Enabled)
                        {
                            child.rect.X = x;
                            RectangleI.Conflate(ref contentRect, ref child.rect, out contentRect);
                            //child.DoLayout();
                            x += child.rect.W + spacing;
                        }
                    }
                }
            }
            else
            {
                //Calculate vertical flex space and horizontall position children
                int fixedH = padTop + padBottom + (enabledChildren - 1) * spacing;
                int flexH = fixedH;
                int flexCount = 0;
                foreach (var child in Children)
                {
                    if (child.Enabled)
                    {
                        //Calculate our flexible space
                        if (child.FlexY)
                            ++flexCount;
                        else
                            flexH += child.rect.H;
                        fixedH += child.rect.H;

                        //Align children vertically
                        if (child.FlexX)
                        {
                            child.rect.X = rect.X + padLeft;
                            child.rect.W = rect.W - padLeft - padRight;
                        }
                        else if (alignX == AlignX.Left)
                            child.rect.X = rect.X + padLeft;
                        else if (alignX == AlignX.Center)
                            child.rect.X = rect.CenterX - child.rect.W / 2;
                        else
                            child.rect.X = rect.MaxX - child.rect.W - padRight;
                    }
                }

                //If we have no extra space to flex with, disable flexing
                flexH = (rect.H - flexH);
                if (flexH <= 0)
                    flexCount = 0;

                //Position and flex elements horizontally
                if (flexCount > 0)
                {
                    //Calculate the width of all the flexed elements
                    int extraH = flexH % flexCount;
                    flexH /= flexCount;

                    //No horizontal alignment if we're flex-fitting
                    int y = rect.Y + padTop;
                    foreach (var child in Children)
                    {
                        if (child.Enabled)
                        {
                            child.rect.Y = y;
                            if (child.FlexY)
                            {
                                child.rect.H = flexH;
                                if (extraH > 0)
                                {
                                    ++child.rect.H;
                                    --extraH;
                                }
                            }
                            RectangleI.Conflate(ref contentRect, ref child.rect, out contentRect);
                            //child.DoLayout();
                            y += child.rect.H + spacing;
                        }
                    }
                }
                else
                {
                    //Horizontally align
                    int y = 0;
                    if (alignY == AlignY.Top)
                        y += rect.Y + padTop;
                    else if (alignY == AlignY.Center)
                        y += rect.CenterY - fixedH / 2;
                    else
                        y += rect.MaxY - fixedH - padBottom;

                    //Position elements horizontally
                    foreach (var child in Children)
                    {
                        if (child.Enabled)
                        {
                            child.rect.Y = y;
                            RectangleI.Conflate(ref contentRect, ref child.rect, out contentRect);
                            //child.DoLayout();
                            y += child.rect.H + spacing;
                        }
                    }
                }
            }

            //Make sure our offset is still valid
            ScrollX = scroll.X;
            ScrollY = scroll.Y;

            //Apply scroll offset and update child layouts
            if (enabledChildren > 0)
            {
                foreach (var child in Children)
                {
                    if (child.Enabled)
                    {
                        child.rect.X += scroll.X;
                        child.rect.Y += scroll.Y;
                        child.DoLayout();
                    }
                }
            }

            base.DoLayout();
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            void RenderChildren()
            {
                foreach (var child in Children)
                    if (child.Enabled)
                        child.DoRender(batch);
            }

            if (BackgroundColor.A > 0)
                batch.DrawRect(rect.X, rect.Y, rect.W, rect.H, BackgroundColor);

            if (children.Count > 0)
            {
                var clip = AlwaysClip || !rect.Equals(ref contentRect);
                if (clip)
                {
                    //Make sure to retain the clip state as we enter/exit child containers
                    //batch.PushClipRect(new RectangleI(rect.X, View.RectH - rect.MaxY, rect.W, rect.H));
                    batch.PushClipRect(new RectangleI(rect.X, rect.Y, rect.W, rect.H));
                    RenderChildren();
                    batch.PopClipRect();
                }
                else
                    RenderChildren();
            }

            base.DoRender(batch);
        }

        internal override void DoMouseDown(MouseButton button)
        {
            base.DoMouseDown(button);

            if (MouseEnabled && MouseChildren)
                foreach (var child in Children)
                    if (child.Enabled && child.MouseOver)
                        child.DoMouseDown(button);
        }

        internal override void DoMouseUp(MouseButton button)
        {
            base.DoMouseUp(button);

            if (MouseEnabled && MouseChildren)
                foreach (var child in Children)
                    if (child.Enabled && child.MouseOver)
                        child.DoMouseUp(button);
        }

        internal override void DoMouseMove()
        {
            base.DoMouseMove();

            if (MouseEnabled && MouseChildren)
            {
                foreach (var child in Children)
                {
                    if (!child.Enabled)
                        continue;

                    if (child.MouseOver && !child.MouseOverPrev)
                        child.DoMouseEnter();

                    if (child.MouseOver)
                        child.DoMouseMove();

                    if (!child.MouseOver && child.MouseOverPrev)
                        child.DoMouseExit();
                }
            }
        }

        internal override void DoMouseExit()
        {
            base.DoMouseExit();

            foreach (var child in Children)
                if (child.Enabled && !child.MouseOver && child.MouseOverPrev)
                    child.DoMouseExit();
        }

        internal override void DoScroll()
        {
            if (!rect.Equals(ref contentRect))
            {
                if (ScrollableX && Mouse.ScrollX != 0)
                    ScrollX += Mouse.ScrollX;
                if (ScrollableY && Mouse.ScrollY != 0)
                    ScrollY += Mouse.ScrollY;
            }

            base.DoScroll();

            if (MouseEnabled && MouseChildren)
                foreach (var child in Children)
                    if (child.Enabled && child.MouseOver)
                        child.DoScroll();
        }

        public T AddChild<T>(T child) where T : Element
        {
            if (child.Parent != null)
                throw new ArgumentException("Element is already in a container.", nameof(child));

            children.Add(child);
            child.DoAdded(this);
            UpdateLayout();

            return child;
        }

        public T RemoveChild<T>(T child) where T : Element
        {
            if (child.Parent != this)
                throw new ArgumentException("Element is not a child of this container.", nameof(child));

            if (View != null)
                children[children.IndexOf(child)] = null;
            else
                children.Remove(child);

            child.DoRemoved();
            UpdateLayout();

            return child;
        }

        public void SetPadding(int left, int right, int top, int bottom)
        {
            PadLeft = left;
            PadRight = right;
            PadTop = top;
            PadBottom = bottom;
        }

        public void SetPadding(int horizontal, int vertical)
        {
            SetPadding(horizontal, horizontal, vertical, vertical);
        }

        public void SetPadding(int padding)
        {
            SetPadding(padding, padding, padding, padding);
        }

        public IEnumerable<Element> Children
        {
            get
            {
                for (int i = 0; i < children.Count; ++i)
                {
                    if (children[i] != null)
                        yield return children[i];
                    else
                        children.RemoveAt(i--);
                }
            }
        }

        public LayoutMode Layout
        {
            get { return layout; }
            set
            {
                if (layout != value)
                {
                    layout = value;
                    UpdateLayout();
                }
            }
        }

        public AlignX AlignX
        {
            get { return alignX; }
            set
            {
                if (alignX != value)
                {
                    alignX = value;
                    UpdateLayout();
                }
            }
        }

        public AlignY AlignY
        {
            get { return alignY; }
            set
            {
                if (alignY != value)
                {
                    alignY = value;
                    UpdateLayout();
                }
            }
        }

        public int Spacing
        {
            get { return spacing; }
            set
            {
                if (spacing != value)
                {
                    spacing = value;
                    UpdateLayout();
                }
            }
        }

        public int PadLeft
        {
            get { return padLeft; }
            set
            {
                if (padLeft != value)
                {
                    padLeft = value;
                    UpdateLayout();
                }
            }
        }

        public int PadRight
        {
            get { return padRight; }
            set
            {
                if (padRight != value)
                {
                    padRight = value;
                    UpdateLayout();
                }
            }
        }

        public int PadTop
        {
            get { return padTop; }
            set
            {
                if (padTop != value)
                {
                    padTop = value;
                    UpdateLayout();
                }
            }
        }

        public int PadBottom
        {
            get { return padBottom; }
            set
            {
                if (padBottom != value)
                {
                    padBottom = value;
                    UpdateLayout();
                }
            }
        }
    }
}
