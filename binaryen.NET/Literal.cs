using System;
using System.Runtime.InteropServices;

namespace Binaryen
{
    /// <summary>
    /// Represents a typed literal value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Literal : IEquatable<Literal>
    {
        /// <summary>
        /// The type of the value.
        /// </summary>
        [FieldOffset(0)]
        public ValueType Type;

        /// <summary>
        /// 32-bit integer value.
        /// </summary>
        [FieldOffset(8)]
        public int I32;
        
        /// <summary>
        /// 64-bit integer value.
        /// </summary>
        [FieldOffset(8)]
        public long I64;

        /// <summary>
        /// 32-bit floating point value.
        /// </summary>
        [FieldOffset(8)]
        public float F32;

        /// <summary>
        /// 64-bit floating point value.
        /// </summary>
        [FieldOffset(8)]
        public double F64;

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 32-bit integer.
        /// </summary>
        /// <param name="x">The 32-bit integer value.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified integer.</returns>
        public static Literal Int32(int x)
        {
            return BinaryenLiteralInt32(x);
        }

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 64-bit integer.
        /// </summary>
        /// <param name="x">The 64-bit integer value.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified integer.</returns>
        public static Literal Int64(long x)
        {
            return BinaryenLiteralInt64(x);
        }

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 32-bit floating point number.
        /// </summary>
        /// <param name="x">The 32-bit floating point value.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified number.</returns>
        public static Literal Float32(float x)
        {
            return BinaryenLiteralFloat32(x);
        }

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 64-bit floating pointer number.
        /// </summary>
        /// <param name="x">The 64-bit floating point value.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified number.</returns>
        public static Literal Float64(double x)
        {
            return BinaryenLiteralFloat64(x);
        }

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 32-bit floating point number bits.
        /// </summary>
        /// <param name="x">The 32-bit floating point bits.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified bits.</returns>
        public static Literal Float32Bits(int x)
        {
            return BinaryenLiteralFloat32Bits(x);
        }

        /// <summary>
        /// Creates a new <see cref="Literal"/> value from the specified 64-bit floating pointer number bits.
        /// </summary>
        /// <param name="x">The 64-bit floating point bits.</param>
        /// <returns>A <see cref="Literal"/> instance representing the specified bits.</returns>
        public static Literal Float64Bits(long x)
        {
            return BinaryenLiteralFloat64Bits(x);
        }

        /// <summary>
        /// Returns a string representation of the literal.
        /// </summary>
        /// <returns>A string representation of the literal.</returns>
        public override string ToString()
        {
            switch (Type)
            {
                case ValueType.Int32:
                    return I32.ToString();

                case ValueType.Int64:
                    return I64.ToString();

                case ValueType.Float32:
                    return F32.ToString();

                case ValueType.Float64:
                    return F64.ToString();

                default:
                    throw new InvalidOperationException($"Unexpected literal type {Type}.");
            }
        }

        /// <summary>
        /// Determines whether the specified object is a <see cref="Literal"/> instance and
        /// is equivalent to this <see cref="Literal"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="obj"/> is a <see cref="Literal"/> instance and
        /// equivalent to this <see cref="Literal"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (obj is Literal) && Equals((Literal)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Literal"/> is equivalent to this <see cref="Literal"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="Literal"/> instance to compare to.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is equivalent to this <see cref="Literal"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Literal other)
        {
            if (Type == other.Type)
            {
                switch (Type)
                {
                    case ValueType.Int32:
                        return I32 == other.I32;

                    case ValueType.Int64:
                        return I64 == other.I64;

                    case ValueType.Float32:
                        return F32 == other.F32;

                    case ValueType.Float64:
                        return F64 == other.F64;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Literal"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            switch (Type)
            {
                case ValueType.Int32:
                    return Type.GetHashCode() ^ I32.GetHashCode();

                case ValueType.Int64:
                    return Type.GetHashCode() ^ I64.GetHashCode();

                case ValueType.Float32:
                    return Type.GetHashCode() ^ F32.GetHashCode();

                case ValueType.Float64:
                    return Type.GetHashCode() ^ F64.GetHashCode();

                default:
                    throw new InvalidOperationException($"Unexpected literal type {Type}.");
            }
        }

        #region Imports

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralInt32(int x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralInt64(long x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat32(float x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat64(double x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat32Bits(int x);

        [DllImport("binaryen", CallingConvention = CallingConvention.Cdecl)]
        private static extern Literal BinaryenLiteralFloat64Bits(long x);

        #endregion
    }
}
