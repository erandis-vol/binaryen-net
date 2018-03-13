using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binaryen
{
    /// <summary>
    /// Represents a segment of memory.
    /// </summary>
    public class MemorySegment
    {
        private byte[] data;
        private Expression offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> class.
        /// </summary>
        /// <param name="data">The memory data.</param>
        /// <param name="offset">The start offset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="offset"/> is <c>null</c>.</exception>
        public MemorySegment(string data, Expression offset) : this(data, null, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> class.
        /// </summary>
        /// <param name="data">The memory data.</param>
        /// <param name="encoding">The data encoding.</param>
        /// <param name="offset">The start offset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="offset"/> is <c>null</c>.</exception>
        public MemorySegment(string data, Encoding encoding, Expression offset)
        {
            if (data == null || offset == null)
                throw new ArgumentNullException(data == null ? nameof(data) : nameof(offset));

            if (encoding == null)
                encoding = Encoding.Default;

            this.data = encoding.GetBytes(data);
            this.offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> class.
        /// </summary>
        /// <param name="data">The memory data.</param>
        /// <param name="offset">The start offset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="offset"/> is <c>null</c>.</exception>
        public MemorySegment(IEnumerable<byte> data, Expression offset)
        {
            if (data == null || offset == null)
                throw new ArgumentNullException(data == null ? nameof(data) : nameof(offset));

            this.data = data.ToArray();
            this.offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemorySegment"/> class.
        /// </summary>
        /// <param name="data">The memory data.</param>
        /// <param name="offset">The start offset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> or <paramref name="offset"/> is <c>null</c>.</exception>
        public MemorySegment(byte[] data, Expression offset)
        {
            if (data == null || offset == null)
                throw new ArgumentNullException(data == null ? nameof(data) : nameof(offset));

            this.data = data;
            this.offset = offset;
        }

        /// <summary>
        /// The data of the memory.
        /// </summary>
        public byte[] Data => data;

        /// <summary>
        /// The offset of the memory.
        /// </summary>
        public Expression Offset => offset;

        /// <summary>
        /// The size of the memory.
        /// </summary>
        public uint Size => (uint)data.Length;
    }
}
