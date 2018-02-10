using System;
using Rise.Serialization;
using Rise.DataTree;
namespace Rise.Gui
{
    public partial class GuiState
    {
        //This is a raw version
        public bool Checkbox(bool isChecked)
        {
            return isChecked;
        }

        //This version could automatically work with the data/undo stack
        public bool Checkbox(Data<bool> isChecked)
        {
            bool clicked = true;
            if (clicked)
            {
                isChecked.Record();
                isChecked.Value = !isChecked.Value;
            }
            return isChecked.Value;
        }
    }
}
