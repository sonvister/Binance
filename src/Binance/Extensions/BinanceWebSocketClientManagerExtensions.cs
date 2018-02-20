using System.Collections.Generic;
using Binance.Manager;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket.Manager
{
    public static class BinanceWebSocketClientManagerExtensions
    {
        /// <summary>
        /// Get all <see cref="IControllerManager"/> managers.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<IControllerManager<IWebSocketStream>> Managers(this IBinanceWebSocketClientManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            yield return manager.AggregateTradeClient as IControllerManager<IWebSocketStream>;
            yield return manager.CandlestickClient as IControllerManager<IWebSocketStream>;
            yield return manager.DepthClient as IControllerManager<IWebSocketStream>;
            yield return manager.StatisticsClient as IControllerManager<IWebSocketStream>;
            yield return manager.TradeClient as IControllerManager<IWebSocketStream>;
        }
    }
}
