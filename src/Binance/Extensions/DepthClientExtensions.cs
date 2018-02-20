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
        public static void Subscribe(this IDepthClient client, string symbol)
            => client.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe(this IDepthClient client, string symbol, int limit)
            => client.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe(this IDepthClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Subscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Unsubscribe(this IDepthClient client, string symbol)
            => client.Unsubscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        public static void Unsubscribe(this IDepthClient client, string symbol, int limit)
            => client.Unsubscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        public static void Unsubscribe(this IDepthClient client, string symbol, Action<DepthUpdateEventArgs> callback)
            => client.Unsubscribe(symbol, default, callback);
    }
}
