using System;
using Binance.Client.Events;

namespace Binance.Client
{
    public interface ISymbolStatisticsClient : IJsonClient
    {
        /// <summary>
        /// The symbol statistics event.
        /// </summary>
        event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate;

        /// <summary>
        /// Subscribe to all symbols.
        /// </summary>
        /// <param name="callback">An event callback.</param>
        void Subscribe(Action<SymbolStatisticsEventArgs> callback);

        /// <summary>
        /// Unsubscribe from all symbols. If no callback is specified,
        /// then unsubscribe from all symbols (all callbacks).
        /// </summary>
        /// <param name="callback"></param>
        void Unsubscribe(Action<SymbolStatisticsEventArgs> callback);

        /// <summary>
        /// Subscribe to the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from a symbol. If no callback is specified,
        /// then unsubscribe from symbol (all callbacks).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback);
    }
}
