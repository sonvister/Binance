using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class CandlestickClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static ICandlestickClient Subscribe(this ICandlestickClient client, string symbol, CandlestickInterval interval)
            => client.Subscribe(symbol, interval,  null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="interval"></param>
        /// <param name="symbols"></param>
        public static ICandlestickClient Subscribe(this ICandlestickClient client, CandlestickInterval interval, params string[] symbols)
            => Subscribe(client, null, interval, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="interval"></param>
        /// <param name="symbols"></param>
        public static ICandlestickClient Subscribe(this ICandlestickClient client, Action<CandlestickEventArgs> callback, CandlestickInterval interval, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            if (callback == null && !symbols.Any())
                throw new ArgumentException($"{nameof(Subscribe)}: At least one symbol is required.", nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Subscribe(symbol, interval, callback);
            }

            return client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        public static ICandlestickClient Unsubscribe(this ICandlestickClient client, string symbol, CandlestickInterval interval)
            => client.Unsubscribe(symbol, interval, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="interval"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static ICandlestickClient Unsubscribe(this ICandlestickClient client, CandlestickInterval interval, params string[] symbols)
            => Unsubscribe(client, null, interval, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <param name="interval"></param>
        /// <param name="symbols"></param>
        public static ICandlestickClient Unsubscribe(this ICandlestickClient client, Action<CandlestickEventArgs> callback, CandlestickInterval interval, params string[] symbols)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(symbols, nameof(symbols));

            if (callback == null && !symbols.Any())
                throw new ArgumentException($"{nameof(Unsubscribe)}: At least one symbol is required.", nameof(symbols));

            foreach (var symbol in symbols)
            {
                client.Unsubscribe(symbol, interval, callback);
            }

            return client;
        }
    }
}
