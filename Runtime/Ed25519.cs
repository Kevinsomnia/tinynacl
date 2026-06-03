using TinyNaCl.Internal;

namespace TinyNaCl
{
    public static class Ed25519
    {
        public const int SignatureSize = 64;
        public const int PublicKeySize = 32;
        public const int PrivateKeySeedSize = 32;

        public static bool Verify(byte[] signature, byte[] message, byte[] publicKey)
        {
            if (signature.Length != SignatureSize)
                throw new System.ArgumentException($"Signature length not {SignatureSize}", nameof(signature));
            if (publicKey.Length != PublicKeySize)
                throw new System.ArgumentException($"Key length not {PublicKeySize}", nameof(publicKey));

            return Ed25519Internal.crypto_sign_verify(signature, message, publicKey);
        }

        /// <summary>
        /// Produces a 64-byte Ed25519 signature for <paramref name="message"/> using
        /// the 32-byte private key seed (RFC 8032 §5.1.6 form).
        /// </summary>
        public static byte[] Sign(byte[] message, byte[] privateKeySeed)
        {
            if (privateKeySeed.Length != PrivateKeySeedSize)
                throw new System.ArgumentException($"Seed length not {PrivateKeySeedSize}", nameof(privateKeySeed));

            return Ed25519Internal.crypto_sign(message, privateKeySeed);
        }

        /// <summary>
        /// Derives the 32-byte Ed25519 public key from a 32-byte private key seed.
        /// </summary>
        public static byte[] PublicKeyFromSeed(byte[] privateKeySeed)
        {
            if (privateKeySeed.Length != PrivateKeySeedSize)
                throw new System.ArgumentException($"Seed length not {PrivateKeySeedSize}", nameof(privateKeySeed));

            return Ed25519Internal.crypto_public_key_from_seed(privateKeySeed);
        }
    }
}