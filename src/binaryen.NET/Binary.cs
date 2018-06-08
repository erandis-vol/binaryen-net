namespace Binaryen
{
    /// <summary>
    /// Represents a binary module.
    /// </summary>
    public class Binary
    {
        /// <summary>
        /// Initialies a new instance of the <see cref="Binary"/> class.
        /// </summary>
        /// <param name="bytes">The binary data.</param>
        /// <param name="sourceMap">The source map.</param>
        public Binary(byte[] bytes, string sourceMap = null)
        {
            Bytes = bytes;
            SourceMap = sourceMap;
        }

        /// <summary>
        /// Gets the binary data.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        /// Gets the binary source map.
        /// </summary>
        public string SourceMap { get; }
    }
}
