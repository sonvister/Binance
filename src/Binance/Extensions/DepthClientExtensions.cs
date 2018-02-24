using System;
using Binance.Client.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class DepthClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static IDepthClient Subscribe(this IDepthClient client, string symbol)
            => client.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IDepthClient Subscribe(this IDepthClient client, string symbol, int limit)
            => client.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IDepthClient Subscribe(this IDepthClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Subscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        public static IDepthClient Subscribe(this IDepthClient client, params string[] symbols)
            => Subscribe(client, null, default, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="limit"></param>
        /// <param name="symbols"></param>
        public static IDepthClient Subscribe(this IDepthClient client, int limit, params string[] symbols)
            => Subscribe(client, null, limit, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="limit"></param>
        /// <param name="symbols"></param>
        public static IDepthClient Subscribe(this IDepthClient client, Action<DepthUpdateEventArgs> callback, int limit, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Subscribe(symbol, limit, callback);
            }

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static IDepthClient Unsubscribe(this IDepthClient client, string symbol)
            => client.Unsubscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        public static IDepthClient Unsubscribe(this IDepthClient client, string symbol, int limit)
            => client.Unsubscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        public static IDepthClient Unsubscribe(this IDepthClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Unsubscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static IDepthClient Unsubscribe(this IDepthClient client, params string[] symbols)
            => Unsubscribe(client, null, default, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="limit"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static IDepthClient Unsubscribe(this IDepthClient client, int limit, params string[] symbols)
            => Unsubscribe(client, null, limit, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="limit"></param>
        /// <param name="symbols"></param>
        public static IDepthClient Unsubscribe(this IDepthClient client, Action<DepthUpdateEventArgs> callback, int limit, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Unsubscribe(symbol, limit, callback);
            }

            return client;
        }
    }
}
