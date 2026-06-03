namespace TinyNaCl.Tests
{
    internal static class Hex
    {
        public static byte[] Decode(string hex)
        {
            if ((hex.Length & 1) != 0)
                throw new System.ArgumentException("Odd-length hex string", nameof(hex));

            var result = new byte[hex.Length >> 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)((Nibble(hex[i << 1]) << 4) | Nibble(hex[(i << 1) + 1]));
            return result;
        }

        public static string Encode(byte[] data)
        {
            const string lut = "0123456789abcdef";
            var chars = new char[data.Length << 1];
            for (int i = 0; i < data.Length; i++)
            {
                chars[i << 1] = lut[data[i] >> 4];
                chars[(i << 1) + 1] = lut[data[i] & 0xF];
            }
            return new string(chars);
        }

        private static int Nibble(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            throw new System.ArgumentException($"Invalid hex char: {c}");
        }
    }
}
