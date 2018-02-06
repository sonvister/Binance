using System.Collections.Generic;
using System.Threading.Tasks;
using Binance.Utility;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket.Manager
{
    public static class BinanceWebSocketClientManagerExtensions
    {
        /// <summary>
        /// Get the <see cref="IRetryTaskController"/> associated with the
        /// <see cref="IBinanceWebSocketClient"/> web socket stream.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static IRetryTaskController GetController(this IBinanceWebSocketManager manager, IBinanceWebSocketClient client)
        {
            Throw.IfNull(manager, nameof(manager));
            Throw.IfNull(client, nameof(client));

            return manager.GetController(client.WebSocket);
        }

        /// <summary>
        /// Get all managed <see cref="IBinanceWebSocketClient"/> clients.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<IBinanceWebSocketClient> Clients(this IBinanceWebSocketManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            yield return manager.AggregateTradeClient;
            yield return manager.CandlestickClient;
            yield return manager.DepthClient;
            yield return manager.StatisticsClient;
            yield return manager.TradeClient;
        }

        /// <summary>
        /// Begin all controller actions.
        /// </summary>
        /// <param name="manager"></param>
        public static void BeginAll(this IBinanceWebSocketManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            foreach (var client in Clients(manager))
            {
                GetController(manager, client).Begin();
            }
        }

        /// <summary>
        /// Cancel all controller actions.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static async Task CancelAllAsync(this IBinanceWebSocketManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            foreach (var client in Clients(manager))
            {
                await GetController(manager, client)
                    .CancelAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Cancel all controller actions and unsubscribe all streams from all clients.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static async Task UnsubscribeAllAsync(this IBinanceWebSocketManager manager)
        {
            Throw.IfNull(manager, nameof(manager));

            await CancelAllAsync(manager)
                .ConfigureAwait(false);

            var wasAutoStreamingDisabled = manager.IsAutoStreamingDisabled;
            manager.IsAutoStreamingDisabled = true; // disable auto-streaming.

            foreach (var client in Clients(manager))
            {
                client.UnsubscribeAll();
            }

            manager.IsAutoStreamingDisabled = wasAutoStreamingDisabled; // restore.
        }
    }
}
