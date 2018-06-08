using System;
using System.Diagnostics;

namespace Binaryen
{
    /// <summary>
    /// Represents an unmanaged object whose memory is managed manually.
    /// </summary>
    public abstract class ManualBaseObject : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManualBaseObject"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        protected ManualBaseObject(IntPtr handle)
        {
            Handle = handle;
            IsDisposed = false;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ManualBaseObject"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        /// <summary>
        /// Asserts whether the object has not been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">the object has been disposed.</exception>
        [DebuggerNonUserCode]
        protected void AssertNotDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(ManualBaseObject));
            }
        }

        /// <summary>
        /// Gets the handle for the unmanaged object.
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// Gets or sets whether the object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }
    }

    /// <summary>
    /// Represents an unmanaged object whose memory is managed automatically.
    /// </summary>
    public abstract class AutomaticBaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticBaseObject"/> class for the specified handle.
        /// </summary>
        /// <param name="handle">The handle to be managed.</param>
        protected AutomaticBaseObject(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// Gets the handle for the unmanaged object.
        /// </summary>
        public IntPtr Handle { get; }
    }
}
