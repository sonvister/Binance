using System.Collections.Generic;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Manager
{
    public static class BinanceJsonClientManagerExtensions
    {
        /// <summary>
        /// Get all <see cref="IJsonClient"/> clients.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<IJsonClient> Clients(this IBinanceJsonClientManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            yield return manager.AggregateTradeClient;
            yield return manager.CandlestickClient;
            yield return manager.DepthClient;
            yield return manager.StatisticsClient;
            yield return manager.TradeClient;
        }

        /// <summary>
        /// Unsubscribe all clients.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static void UnsubscribeAll(this IBinanceJsonClientManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            foreach (var client in Clients(manager))
            {
                client.Unsubscribe();
            }
        }
    }
}
