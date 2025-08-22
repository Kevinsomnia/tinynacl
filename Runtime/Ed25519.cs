using TinyNaCl.Internal;

namespace TinyNaCl
{
    public static class Ed25519
    {
        public static bool Verify(byte[] signature, byte[] message, byte[] publicKey)
        {
            if (publicKey.Length != 32)
                throw new System.ArgumentException("Key length not 32", nameof(publicKey));

            return Ed25519Internal.crypto_sign_verify(signature, message, publicKey);
        }
    }
}