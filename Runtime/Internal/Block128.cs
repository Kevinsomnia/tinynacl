using System.Runtime.InteropServices;

namespace TinyNaCl.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Block128
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
        [FieldOffset(64)]
        public ulong x8;
        [FieldOffset(72)]
        public ulong x9;
        [FieldOffset(80)]
        public ulong x10;
        [FieldOffset(88)]
        public ulong x11;
        [FieldOffset(96)]
        public ulong x12;
        [FieldOffset(104)]
        public ulong x13;
        [FieldOffset(112)]
        public ulong x14;
        [FieldOffset(120)]
        public ulong x15;
    }
}