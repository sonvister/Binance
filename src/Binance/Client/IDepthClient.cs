using System;
using Binance.Client.Events;

namespace Binance.Client
{
    public interface IDepthClient : IJsonClient
    {
        /// <summary>
        /// The depth update event.
        /// </summary>
        event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        /// <summary>
        /// Subscribe to the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback">An event callback.</param>
        IDepthClient Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from a symbol. If no callback is specified,
        /// then unsubscribe from ymbol (all callbacks).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback"></param>
        IDepthClient Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback);

        /// <summary>
        /// Unsubscribe from all symbols (and callbacks).
        /// </summary>
        /// <returns></returns>
        new IDepthClient Unsubscribe();
    }
}
