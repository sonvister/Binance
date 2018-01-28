using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;

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
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IDepthWebSocketClient client, string symbol, CancellationToken token)
            => client.StreamAsync(symbol, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IDepthWebSocketClient client, string symbol, int limit, CancellationToken token)
            => client.StreamAsync(symbol, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IDepthWebSocketClient client, string symbol, Action<DepthUpdateEventArgs> callback, CancellationToken token)
            => StreamAsync(client, symbol, default, callback, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IDepthWebSocketClient client, string symbol, int limit, Action<DepthUpdateEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            client.Subscribe(symbol, limit, callback);

            return client.WebSocket.StreamAsync(token);
        }
    }
}
