using System;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    public interface ISymbolStatisticsWebSocketClient : IBinanceWebSocketClient
    {
        #region Events

        /// <summary>
        /// The symbol statistics event.
        /// </summary>
        event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate;

        #endregion Events

        #region Methods

        /// <summary>
        /// Subscribe to all symbols (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="callback">An event callback.</param>
        void Subscribe(Action<SymbolStatisticsEventArgs> callback);

        /// <summary>
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback);

        #endregion Methods
    }
}
