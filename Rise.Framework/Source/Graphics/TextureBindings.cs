using System;
using System.Collections.Generic;
using Rise.OpenGL;
namespace Rise
{
    static class TextureBindings
    {
        struct Binding
        {
            public uint ID;
            public TextureTarget Target;
        }

        static Binding[] binded = new Binding[GL.MaxTextureUnits];
        static HashSet<uint> unbind = new HashSet<uint>();

        internal static void MakeCurrent(uint id, TextureTarget target)
        {
            if (binded[0].ID != id)
            {
                GL.ActiveTexture(0);

                //If a texture is already binded to slot 0, unbind it
                if (binded[0].Target != target)
                {
                    GL.BindTexture(binded[0].Target, 0);
                    binded[0].Target = target;
                }

                binded[0].ID = id;
                GL.BindTexture(target, id);
            }
        }

        internal static int Bind(uint id, TextureTarget target)
        {
            //If we're marked for unbinding, unmark us
            unbind.Remove(id);

            //If we're already binded, return our slot
            int i = Array.IndexOf(binded, id);
            if (i >= 0)
                return i;

            //If we're not already binded, bind us and return the slot
            for (i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID == 0)
                {
                    binded[i].ID = id;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(target, id);
                    return i;
                }
            }

            throw new Exception("You have exceeded the maximum amount of texture bindings: " + GL.MaxTextureUnits);
        }

        internal static void MarkAllForUnbinding()
        {
            for (int i = 0; i < binded.Length; ++i)
                if (binded[i].ID != 0)
                    unbind.Add(binded[i].ID);
        }

        internal static void UnbindMarked()
        {
            for (uint i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID != 0 && unbind.Contains(binded[i].ID))
                {
                    binded[i].ID = 0;
                    GL.ActiveTexture(i);
                    GL.BindTexture(binded[i].Target, 0);
                }
            }
        }

        internal static void UnbindAll()
        {
            unbind.Clear();
            for (int i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID != 0)
                {
                    binded[i].ID = 0;
                    GL.ActiveTexture((uint)i);
                    GL.BindTexture(binded[i].Target, 0);
                }
            }
        }
    }
}
