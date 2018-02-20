using System;

namespace Binance
{
    public class ErrorEventArgs : EventArgs
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
        /// <param name="exception"></param>
        public ErrorEventArgs(Exception exception)
        {
            Throw.IfNull(exception, nameof(exception));

            Exception = exception;
        }

        #endregion Constructors
    }
}
