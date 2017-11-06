namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// User data web socket client event.
    /// </summary>
    public abstract class UserDataEventArgs : WebSocketClientEventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        protected UserDataEventArgs(long timestamp)
            : base(timestamp)
        { }

        #endregion Constructors
    }
}
