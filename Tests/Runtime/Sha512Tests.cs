using System.Text;
using NUnit.Framework;

namespace TinyNaCl.Tests
{
    public class Sha512Tests
    {
        [Test]
        public void Hash_EmptyInput_MatchesNistVector()
        {
            const string expected =
                "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce" +
                "47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e";
            Assert.AreEqual(expected, Hex.Encode(Hash(new byte[0])));
        }

        [Test]
        public void Hash_Abc_MatchesNistVector()
        {
            const string expected =
                "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a" +
                "2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f";
            Assert.AreEqual(expected, Hex.Encode(Hash(Encoding.ASCII.GetBytes("abc"))));
        }

        [Test]
        public void Hash_LongerInput_MatchesNistVector()
        {
            // FIPS 180-4 sample: 896-bit message
            const string msg = "abcdefghbcdefghicdefghijdefghijkefghijklfghijklmghijklmnhijklmnoijklmnopjklmnopqklmnopqrlmnopqrsmnopqrstnopqrstu";
            const string expected =
                "8e959b75dae313da8cf4f72814fc143f8f7779c6eb9f7fa17299aeadb6889018" +
                "501d289e4900f7e4331b99dec4b5433ac7d329eeb6dd26545e96e55b874be909";
            Assert.AreEqual(expected, Hex.Encode(Hash(Encoding.ASCII.GetBytes(msg))));
        }

        [Test]
        public void Hash_MultiBlockInput_MatchesSingleShot()
        {
            // ~3 blocks worth of pseudo-random data, hashed in chunks vs in one shot.
            var data = new byte[400];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)((i * 31 + 7) & 0xFF);

            var oneShot = new Sha512();
            oneShot.Update(data, 0, data.Length);
            var expected = oneShot.Finalize();

            var chunked = new Sha512();
            chunked.Update(data, 0, 50);
            chunked.Update(data, 50, 130);
            chunked.Update(data, 180, 220);
            var actual = chunked.Finalize();

            Assert.AreEqual(Hex.Encode(expected), Hex.Encode(actual));
        }

        private static byte[] Hash(byte[] data)
        {
            var h = new Sha512();
            h.Update(data, 0, data.Length);
            return h.Finalize();
        }
    }
}
