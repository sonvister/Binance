using System;
using System.Linq;
using Binance.Client.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class AggregateTradeClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static IAggregateTradeClient Subscribe(this IAggregateTradeClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        public static IAggregateTradeClient Subscribe(this IAggregateTradeClient client, params string[] symbols)
            => Subscribe(client, null, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="symbols"></param>
        public static IAggregateTradeClient Subscribe(this IAggregateTradeClient client, Action<AggregateTradeEventArgs> callback, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            if (callback == null && !symbols.Any())
                throw new ArgumentException($"{nameof(Subscribe)}: At least one symbol is required.", nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Subscribe(symbol, callback);
            }

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static IAggregateTradeClient Unsubscribe(this IAggregateTradeClient client, string symbol)
            => client.Unsubscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static IAggregateTradeClient Unsubscribe(this IAggregateTradeClient client, params string[] symbols)
            => Unsubscribe(client, null, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="symbols"></param>
        public static IAggregateTradeClient Unsubscribe(this IAggregateTradeClient client, Action<AggregateTradeEventArgs> callback, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            if (callback == null && !symbols.Any())
                throw new ArgumentException($"{nameof(Unsubscribe)}: At least one symbol is required.", nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Unsubscribe(symbol, callback);
            }

            return client;
        }
    }
}
