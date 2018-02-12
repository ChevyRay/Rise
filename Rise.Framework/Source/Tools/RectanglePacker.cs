using System;
using System.Runtime.InteropServices;
namespace Rise
{
    public class RectanglePacker
    {
        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr new_packer(int capacity);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void free_packer(IntPtr packer);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void packer_init(IntPtr packer, int width, int height);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void packer_add(IntPtr packer, int id, int w, int h, bool can_rotate);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool packer_pack(IntPtr packer);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void packer_get(IntPtr packer, int index, out int id, out int x, out int y, out int w, out int h);

        [DllImport("risetools.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int packer_get_count(IntPtr packer);

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int PackedCount { get; private set; }

        IntPtr packer;

        public RectanglePacker(int width, int height, int capacity)
        {
            packer = new_packer(capacity);
            Init(width, height);
        }
        public RectanglePacker(int width, int height)
            : this(width, height, 32)
        {
            
        }
        ~RectanglePacker()
        {
            free_packer(packer);
        }

        public void Init(int width, int height)
        {
            packer_init(packer, width, height);
            Width = width;
            Height = height;
            PackedCount = 0;
        }

        public void Add(int id, int width, int height, bool canRotate)
        {
            packer_add(packer, id, width, height, canRotate);
        }

        public bool Pack()
        {
            bool result = packer_pack(packer);
            PackedCount = packer_get_count(packer);
            return result;
        }

        public void GetPacked(int i, out int id, out RectangleI rect)
        {
            if (i < 0 || i >= PackedCount)
                throw new ArgumentOutOfRangeException(nameof(i));
            packer_get(packer, i, out id, out rect.X, out rect.Y, out rect.W, out rect.H);
        }
    }
}
