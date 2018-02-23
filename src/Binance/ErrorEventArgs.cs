using System;

namespace Binance
{
    /// <summary>
    /// Error event arguments.
    /// </summary>
    public sealed class ErrorEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the exception.
        /// </summary>
        public Exception Exception { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">The exception (required).</param>
        public ErrorEventArgs(Exception exception)
        {
            Throw.IfNull(exception, nameof(exception));

            Exception = exception;
        }

        #endregion Constructors
    }
}
