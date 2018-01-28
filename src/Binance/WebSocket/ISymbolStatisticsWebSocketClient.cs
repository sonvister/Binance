using System;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    public interface ISymbolStatisticsWebSocketClient : IBinanceWebSocketClient
    {
        /// <summary>
        /// The symbol statistics event.
        /// </summary>
        event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate;

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

        /// <summary>
        /// Unsubscribe from all symbol events. If no callback is
        /// specified, then unsubscribe all symbols (all callbacks).
        /// </summary>
        /// <param name="callback"></param>
        void Unsubscribe(Action<SymbolStatisticsEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from symbol events. If no callback is
        /// specified, then unsubscribe symbol (all callbacks).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback);
    }
}
