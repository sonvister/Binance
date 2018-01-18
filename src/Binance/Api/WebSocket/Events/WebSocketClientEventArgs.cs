using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Low-level <see cref="IWebSocketClient"/> message event arguments.
    /// </summary>
    public class WebSocketClientEventArgs : EventArgs
    {
        #region Public Properties

        public string Message { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        public WebSocketClientEventArgs(string message)
        {
            Throw.IfNullOrWhiteSpace(message, nameof(message));

            Message = message;
        }

        #endregion Constructors
    }
}
