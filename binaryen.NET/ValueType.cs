namespace Binaryen
{
    /// <summary>
    /// Represents a value type.
    /// </summary>
    public enum ValueType : int
    {
        /// <summary>
        /// No type.
        /// </summary>
        None        = 0,
        
        /// <summary>
        /// 32-bit integer type.
        /// </summary>
        Int32       = 1,
        
        /// <summary>
        /// 64-bit integer type.
        /// </summary>
        Int64       = 2,

        /// <summary>
        /// 32-bit floating point number type.
        /// </summary>
        Float32     = 3,

        /// <summary>
        /// 64-bit floating point number type.
        /// </summary>
        Float64     = 4,

        /// <summary>
        /// Special type. Used to indicate unreachable code.
        /// </summary>
        Unreachable = 5,

        /// <summary>
        /// Special type. Used by blocks to automatically determine their type.
        /// </summary>
        Auto        = -1,
    }
}
