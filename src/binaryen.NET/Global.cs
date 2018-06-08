using System;

namespace Binaryen
{
    // Global values are unique in that the API does not allow us to query their properties

    /// <summary>
    /// Represents a global value.
    /// </summary>
    public class Global : AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Global"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The reference handle.</param>
        /// <exception cref="NotSupportedException">always throws.</exception>
        public Global(IntPtr handle)
            : base(handle)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Global"/> class.
        /// </summary>
        /// <param name="name">The internal name.</param>
        /// <param name="type">The type.</param>
        /// <param name="mutable">Whether the value is mutable.</param>
        /// <param name="init">The initial value expression.</param>
        public Global(string name, ValueType type, bool mutable, Expression init)
            : base(IntPtr.Zero)
        {
            Name = name;
            Type = type;
            IsMutable = mutable;
            InitialValue = init;
        }

        /// <summary>
        /// Gets the name of the global.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the global.
        /// </summary>
        public ValueType Type { get; }

        /// <summary>
        /// Determines whether the global is mutable.
        /// </summary>
        public bool IsMutable { get; }

        /// <summary>
        /// Gets the initial value expression of the global.
        /// </summary>
        public Expression InitialValue { get; }
    }
}
