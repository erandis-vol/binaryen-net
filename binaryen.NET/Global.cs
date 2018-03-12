using System;

namespace Binaryen
{
    // Global values are unique in that the API does not allow us to query their properties

    /// <summary>
    /// Represents a global value.
    /// </summary>
    public class Global
    {
        private string name;
        private ValueType type;
        private bool mutable;
        private Expression init;

        /// <summary>
        /// Initializes a new instance of the <see cref="Global"/> class.
        /// </summary>
        /// <param name="handle">The reference handle.</param>
        /// <exception cref="NotSupportedException">always throws.</exception>
        public Global(IntPtr handle)
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
        {
            this.name = name;
            this.type = type;
            this.mutable = mutable;
            this.init = init;
        }

        /// <summary>
        /// Gets the name of the global.
        /// </summary>
        public string Name => name;

        /// <summary>
        /// Gets the type of the global.
        /// </summary>
        public ValueType Type => type;

        /// <summary>
        /// Gets whether the global is mutable.
        /// </summary>
        public bool IsMutable => mutable;

        /// <summary>
        /// Gets the initial value expression of the global.
        /// </summary>
        public Expression InitialValue => init;
    }
}
