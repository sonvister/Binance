using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class BinanceWebSocketClientManagerExtensions
    {
        /// <summary>
        /// Get all <see cref="IControllerManager"/> managers.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<IWebSocketPublisherClient> Clients(this IBinanceWebSocketClientManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            yield return manager.AggregateTradeClient as IWebSocketPublisherClient;
            yield return manager.CandlestickClient as IWebSocketPublisherClient;
            yield return manager.DepthClient as IWebSocketPublisherClient;
            yield return manager.StatisticsClient as IWebSocketPublisherClient;
            yield return manager.TradeClient as IWebSocketPublisherClient;
        }
    }
}
