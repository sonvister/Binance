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
        /// Subscribe to one or more symbols. If no symbols are specified,
        /// subscribe to all symbols.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="symbols"></param>
        void Subscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols);

        /// <summary>
        /// Unsubscribe from one or more symbols. If no symbols are specified,
        /// unsubscrbe from all symbols.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="symbols"></param>
        void Unsubscribe(Action<SymbolStatisticsEventArgs> callback, params string[] symbols);
    }
}
