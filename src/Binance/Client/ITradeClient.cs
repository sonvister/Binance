using System;
using Binance.Client.Events;

namespace Binance.Client
{
    public interface ITradeClient : IJsonClient
    {
        /// <summary>
        /// The trade event.
        /// </summary>
        event EventHandler<TradeEventArgs> Trade;

        /// <summary>
        /// Subscribe to the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="callback">An event callback.</param>
        ITradeClient Subscribe(string symbol, Action<TradeEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from a symbol. If no callback is specified,
        /// then unsubscribe from symbol (all callbacks).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        ITradeClient Unsubscribe(string symbol, Action<TradeEventArgs> callback);

        /// <summary>
        /// Unsubscribe from all symbols (and callbacks).
        /// </summary>
        /// <returns></returns>
        new ITradeClient Unsubscribe();
    }
}
