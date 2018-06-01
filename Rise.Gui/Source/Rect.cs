using System;
namespace Rise.Gui
{
    public class Rect : Element
    {
        public Rect() : base(16, 16, true, true)
        {
            
        }

        internal override void DoRender(DrawBatch2D batch)
        {
            base.DoRender(batch);
        }
    }
}
