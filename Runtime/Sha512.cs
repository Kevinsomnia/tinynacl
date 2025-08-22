using System;
using TinyNaCl.Internal;

namespace TinyNaCl
{
    public class Sha512
    {
        private Block64 _state;
        private readonly byte[] _buffer;
        private ulong _totalBytes;
        public const int BlockSize = 128;
        private static readonly byte[] _padding = new byte[] { 0x80 };

        public Sha512()
        {
            _buffer = new byte[BlockSize];//todo: remove allocation
            Init();
        }

        public void Init()
        {
            Sha512Internal.Sha512Init(out _state);
            _totalBytes = 0;
        }

        /// <summary>
        /// Updates internal state with data from the provided array.
        /// </summary>
        /// <param name="data">Array of bytes</param>
        /// <param name="index">Offset of byte sequence</param>
        /// <param name="length">Sequence length</param>
        public void Update(byte[] data, int index, int length)
        {
            Block128 block;
            int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            _totalBytes += (uint)length;

            if (_totalBytes >= ulong.MaxValue / 8)
                throw new InvalidOperationException("Too much data");
            // Fill existing buffer
            if (bytesInBuffer != 0)
            {
                var toCopy = Math.Min(BlockSize - bytesInBuffer, length);
                Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, toCopy);
                index += toCopy;
                length -= toCopy;
                bytesInBuffer += toCopy;
                if (bytesInBuffer == BlockSize)
                {
                    ByteIntegerConverter.LoadBlock128(out block, _buffer, 0);
                    Sha512Internal.Core(out _state, ref _state, ref block);
                    CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
                    bytesInBuffer = 0;
                }
            }
            // Hash complete blocks without copying
            while (length >= BlockSize)
            {
                ByteIntegerConverter.LoadBlock128(out block, data, index);
                Sha512Internal.Core(out _state, ref _state, ref block);
                index += BlockSize;
                length -= BlockSize;
            }
            // Copy remainder into buffer
            if (length > 0)
            {
                Buffer.BlockCopy(data, index, _buffer, bytesInBuffer, length);
            }
        }

        public void Finalize(ArraySegment<byte> output)
        {
            Update(_padding, 0, _padding.Length);
            Block128 block;
            ByteIntegerConverter.LoadBlock128(out block, _buffer, 0);
            CryptoBytes.InternalWipe(_buffer, 0, _buffer.Length);
            int bytesInBuffer = (int)_totalBytes & (BlockSize - 1);
            if (bytesInBuffer > BlockSize - 16)
            {
                Sha512Internal.Core(out _state, ref _state, ref block);
                block = default;
            }
            block.x15 = (_totalBytes - 1) * 8;
            Sha512Internal.Core(out _state, ref _state, ref block);

            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 0, _state.x0);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 8, _state.x1);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 16, _state.x2);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 24, _state.x3);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 32, _state.x4);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 40, _state.x5);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 48, _state.x6);
            ByteIntegerConverter.StoreBigEndian64(output.Array, output.Offset + 56, _state.x7);
            _state = default;
        }

        public byte[] Finalize()
        {
            var result = new byte[64];
            Finalize(new ArraySegment<byte>(result));
            return result;
        }
    }
}
