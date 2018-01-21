using System;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    public interface IAggregateTradeWebSocketClient : IBinanceWebSocketClient
    {
        #region Public Events

        /// <summary>
        /// The aggregate trade event.
        /// </summary>
        event EventHandler<AggregateTradeEventArgs> AggregateTrade;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, Action<AggregateTradeEventArgs> callback);

        #endregion Public Methods
    }
}
