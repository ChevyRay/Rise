using System;
using System.Collections.Generic;
using Rise.OpenGL;
namespace Rise
{
    public abstract class Texture : ResourceHandle
    {
        struct Binding
        {
            public uint ID;
            public TextureTarget Target;
        }

        static Binding[] binded = new Binding[GL.MaxTextureUnits];
        static HashSet<uint> unbind = new HashSet<uint>();

        public static TextureFilter DefaultMinFilter = TextureFilter.Linear;
        public static TextureFilter DefaultMagFilter = TextureFilter.Linear;
        public static TextureWrap DefaultWrapX = TextureWrap.ClampToEdge;
        public static TextureWrap DefaultWrapY = TextureWrap.ClampToEdge;

        internal uint ID { get; private set; }
        public TextureFormat Format { get; private set; }
        internal TextureTarget BindTarget { get; private set; }
        internal TextureTarget DataTarget { get; private set; }
        public int Width { get; internal set; }
        public int Height { get; internal set; }

        internal Texture(uint id, TextureFormat format, TextureTarget bindTarget, TextureTarget dataTarget)
        {
            ID = id;
            Format = format;
            BindTarget = bindTarget;
            DataTarget = dataTarget;
        }

        protected override void Dispose()
        {
            
        }

        internal void MakeCurrent()
        {
            MakeCurrent(ID, BindTarget);
        }
        internal static void MakeCurrent(uint id, TextureTarget target)
        {
            if (binded[0].ID != id)
            {
                GL.ActiveTexture(0);

                //If a texture is already binded to slot 0, unbind it
                if (binded[0].ID != 0 && binded[0].Target != target)
                    GL.BindTexture(binded[0].Target, 0);

                binded[0].ID = id;
                binded[0].Target = target;
                GL.BindTexture(target, id);
            }
        }

        internal static int Bind(uint id, TextureTarget target)
        {
            //If we're marked for unbinding, unmark us
            unbind.Remove(id);

            //If we're already binded, return our slot
            for (int i = 0; i < binded.Length; ++i)
                if (binded[i].ID == id)
                    return i;

            //If we're not already binded, bind us and return the slot
            for (int i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID == 0)
                {
                    binded[i].ID = id;
                    binded[i].Target = target;
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
                    GL.ActiveTexture(i);
                    GL.BindTexture(binded[i].Target, 0);
                    binded[i].ID = 0;
                }
            }
        }

        internal static void UnbindAll()
        {
            unbind.Clear();
            for (uint i = 0; i < binded.Length; ++i)
            {
                if (binded[i].ID != 0)
                {
                    GL.ActiveTexture(i);
                    GL.BindTexture(binded[i].Target, 0);
                    binded[i].ID = 0;
                }
            }
        }
    }
}
