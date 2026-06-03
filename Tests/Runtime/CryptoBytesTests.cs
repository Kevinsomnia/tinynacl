using NUnit.Framework;

namespace TinyNaCl.Tests
{
    public class CryptoBytesTests
    {
        [Test]
        public void ConstantTimeEquals_IdenticalBuffers_ReturnsTrue()
        {
            var a = new byte[] { 1, 2, 3, 4, 5 };
            var b = new byte[] { 1, 2, 3, 4, 5 };
            Assert.IsTrue(CryptoBytes.ConstantTimeEquals(a, b, a.Length));
        }

        [Test]
        public void ConstantTimeEquals_DifferAtStart_ReturnsFalse()
        {
            var a = new byte[] { 9, 2, 3, 4, 5 };
            var b = new byte[] { 1, 2, 3, 4, 5 };
            Assert.IsFalse(CryptoBytes.ConstantTimeEquals(a, b, a.Length));
        }

        [Test]
        public void ConstantTimeEquals_DifferAtEnd_ReturnsFalse()
        {
            var a = new byte[] { 1, 2, 3, 4, 5 };
            var b = new byte[] { 1, 2, 3, 4, 9 };
            Assert.IsFalse(CryptoBytes.ConstantTimeEquals(a, b, a.Length));
        }

        [Test]
        public void ConstantTimeEquals_ZeroLength_ReturnsTrue()
        {
            Assert.IsTrue(CryptoBytes.ConstantTimeEquals(new byte[0], new byte[0], 0));
        }

        [Test]
        public void ConstantTimeEquals_ComparesOnlyFirstN()
        {
            var a = new byte[] { 1, 2, 3, 9 };
            var b = new byte[] { 1, 2, 3, 8 };
            Assert.IsTrue(CryptoBytes.ConstantTimeEquals(a, b, 3));
        }

        [Test]
        public void Wipe_ClearsAllBytes()
        {
            var data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x42 };
            CryptoBytes.Wipe(data);
            foreach (var b in data)
                Assert.AreEqual(0, b);
        }
    }
}
