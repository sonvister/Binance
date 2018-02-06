using System;

namespace Binance.WebSocket.Manager
{
    public class BinanceWebSocketClientManagerErrorEventArgs : EventArgs
    {
        #region Public Properties

        public BinanceWebSocketClientManagerException Exception { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception"></param>
        public BinanceWebSocketClientManagerErrorEventArgs(BinanceWebSocketClientManagerException exception)
        {
            Throw.IfNull(exception, nameof(exception));

            Exception = exception;
        }

        #endregion Constructors
    }
}
