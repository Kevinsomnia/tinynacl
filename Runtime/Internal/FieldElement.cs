using System.Runtime.InteropServices;

namespace TinyNaCl.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct FieldElement
    {
        [FieldOffset(0)]
        internal int x0;
        [FieldOffset(4)]
        internal int x1;
        [FieldOffset(8)]
        internal int x2;
        [FieldOffset(12)]
        internal int x3;
        [FieldOffset(16)]
        internal int x4;
        [FieldOffset(20)]
        internal int x5;
        [FieldOffset(24)]
        internal int x6;
        [FieldOffset(28)]
        internal int x7;
        [FieldOffset(32)]
        internal int x8;
        [FieldOffset(36)]
        internal int x9;

        internal FieldElement(params int[] elements)
        {
            x0 = elements[0];
            x1 = elements[1];
            x2 = elements[2];
            x3 = elements[3];
            x4 = elements[4];
            x5 = elements[5];
            x6 = elements[6];
            x7 = elements[7];
            x8 = elements[8];
            x9 = elements[9];
        }
    }
}
