using System;
using System.Runtime.CompilerServices;

namespace TinyNaCl
{
    public static class CryptoBytes
    {
        /// <summary>
        /// Comparison of two byte sequences.
        /// 
        /// The runtime of this method does not depend on the contents of the arrays. Using constant time
        /// prevents timing attacks that allow an attacker to learn if the arrays have a common prefix.
        /// 
        /// It is important to use such a constant time comparison when verifying MACs.
        /// </summary>
        /// <param name="length">Length of both byte sequences</param>
        /// <returns>True if sequences are equal</returns>
        public static bool ConstantTimeEquals(byte[] x, byte[] y, int length)
        {
            return InternalConstantTimeEquals(x, y, length) != 0;
        }

        private static uint InternalConstantTimeEquals(byte[] x, byte[] y, int length)
        {
            int differentbits = 0;
            for (int i = 0; i < length; i++)
                differentbits |= x[i] ^ y[i];
            return 1 & (unchecked((uint)differentbits - 1) >> 8);
        }

        /// <summary>
        /// Overwrites the contents of the array, wiping the previous content. 
        /// </summary>
        public static void Wipe(byte[] data)
        {
            InternalWipe(data, 0, data.Length);
        }

        // Secure wiping is hard
        // * the GC can move around and copy memory
        //   Perhaps this can be avoided by using unmanaged memory or by fixing the position of the array in memory
        // * Swap files and error dumps can contain secret information
        //   It seems possible to lock memory in RAM, no idea about error dumps
        // * Compiler could optimize out the wiping if it knows that data won't be read back
        //   I hope this is enough, suppressing inlining
        //   but perhaps `RtlSecureZeroMemory` is needed
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe(byte[] data, int offset, int count)
        {
            Array.Clear(data, offset, count);
        }
    }
}
