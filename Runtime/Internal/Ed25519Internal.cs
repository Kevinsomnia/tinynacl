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

            GroupOperations.ge_double_scalarmult_vartime(out R, h, ref A, signature, 32);
            GroupOperations.ge_tobytes(checkr, 0, ref R);
            var result = CryptoBytes.ConstantTimeEquals(checkr, signature, 32);
            CryptoBytes.Wipe(h);
            CryptoBytes.Wipe(checkr);
            return result;
        }

        // RFC 8032 §5.1.6 — Ed25519 signing.
        // seed is the 32-byte private key. Returns 64-byte signature = R || S.
        public static byte[] crypto_sign(byte[] message, byte[] seed)
        {
            var hasher = new Sha512();
            hasher.Update(seed, 0, 32);
            byte[] expanded = hasher.Finalize();

            // Clamp the lower half to produce the signing scalar.
            expanded[0] &= 248;
            expanded[31] &= 127;
            expanded[31] |= 64;

            // A = a * B
            byte[] publicKey = new byte[32];
            GroupOperations.ge_scalarmult_base(out GroupElementP3 A, expanded, 0);
            GroupOperations.ge_p3_tobytes(publicKey, 0, ref A);

            // r = SHA-512(prefix || message) mod L
            hasher.Init();
            hasher.Update(expanded, 32, 32);
            hasher.Update(message, 0, message.Length);
            byte[] rHash = hasher.Finalize();
            ScalarOperations.sc_reduce(rHash);

            // R = r * B, written directly into signature[0..32].
            byte[] signature = new byte[64];
            GroupOperations.ge_scalarmult_base(out GroupElementP3 R, rHash, 0);
            GroupOperations.ge_p3_tobytes(signature, 0, ref R);

            // k = SHA-512(R || A || message) mod L
            hasher.Init();
            hasher.Update(signature, 0, 32);
            hasher.Update(publicKey, 0, 32);
            hasher.Update(message, 0, message.Length);
            byte[] kHash = hasher.Finalize();
            ScalarOperations.sc_reduce(kHash);

            // S = (k * a + r) mod L, written into signature[32..64].
            byte[] s = new byte[32];
            ScalarOperations.sc_muladd(s, kHash, expanded, rHash);
            Array.Copy(s, 0, signature, 32, 32);

            CryptoBytes.Wipe(expanded);
            CryptoBytes.Wipe(rHash);
            CryptoBytes.Wipe(kHash);
            CryptoBytes.Wipe(s);
            return signature;
        }

        public static byte[] crypto_public_key_from_seed(byte[] seed)
        {
            var hasher = new Sha512();
            hasher.Update(seed, 0, 32);
            byte[] expanded = hasher.Finalize();
            expanded[0] &= 248;
            expanded[31] &= 127;
            expanded[31] |= 64;

            byte[] publicKey = new byte[32];
            GroupOperations.ge_scalarmult_base(out GroupElementP3 A, expanded, 0);
            GroupOperations.ge_p3_tobytes(publicKey, 0, ref A);

            CryptoBytes.Wipe(expanded);
            return publicKey;
        }
    }
}