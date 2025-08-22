using System;

namespace TinyNaCl.Internal
{
    internal static class Ed25519Internal
    {
        public static bool crypto_sign_verify(byte[] signature, byte[] message, byte[] key)
        {
            byte[] h;
            byte[] checkr = new byte[32];
            GroupElementP3 A;
            GroupElementP2 R;

            if ((signature[63] & 224) != 0)
                return false;
            if (GroupOperations.ge_frombytes_negate_vartime(out A, key, 0) != 0)
                return false;

            var hasher = new Sha512();
            hasher.Update(signature, 0, 32);
            hasher.Update(key, 0, 32);
            hasher.Update(message, 0, message.Length);
            h = hasher.Finalize();

            ScalarOperations.sc_reduce(h);

            var sm32 = new byte[32];//todo: remove allocation
            Array.Copy(signature, 32, sm32, 0, 32);
            GroupOperations.ge_double_scalarmult_vartime(out R, h, ref A, sm32);
            GroupOperations.ge_tobytes(checkr, 0, ref R);
            var result = CryptoBytes.ConstantTimeEquals(checkr, signature, 32);
            CryptoBytes.Wipe(h);
            CryptoBytes.Wipe(checkr);
            return result;
        }
    }
}