using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client.Events;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class DepthWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this IDepthWebSocketClient client, string symbol)
            => client.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe(this IDepthWebSocketClient client, string symbol, int limit)
            => client.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe(this IDepthWebSocketClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Subscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Unsubscribe(this IDepthWebSocketClient client, string symbol)
            => client.Unsubscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        public static void Unsubscribe(this IDepthWebSocketClient client, string symbol, int limit)
            => client.Unsubscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        public static void Unsubscribe(this IDepthWebSocketClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Unsubscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IDepthWebSocketClient client, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            return client.Stream.StreamAsync(token);
        }
    }
}
