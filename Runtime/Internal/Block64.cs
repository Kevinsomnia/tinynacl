using System.Runtime.InteropServices;

namespace TinyNaCl.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Block64
    {
        [FieldOffset(0)]
        public ulong x0;
        [FieldOffset(8)]
        public ulong x1;
        [FieldOffset(16)]
        public ulong x2;
        [FieldOffset(24)]
        public ulong x3;
        [FieldOffset(32)]
        public ulong x4;
        [FieldOffset(40)]
        public ulong x5;
        [FieldOffset(48)]
        public ulong x6;
        [FieldOffset(56)]
        public ulong x7;
    }
}