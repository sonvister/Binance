using System;
using System.Collections.Generic;
using Binance.Client;

namespace Binance.Cache
{
    public interface ICandlestickCache : ICandlestickCache<ICandlestickClient>
    { }

    public interface ICandlestickCache<TClient> : IJsonClientCache<TClient, CandlestickCacheEventArgs>
        where TClient : ICandlestickClient
    {
        /// <summary>
        /// The candlesticks. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<Candlestick> Candlesticks { get; }

        /// <summary>
        /// Subscribe to a symbol, interval combination.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        void Subscribe(string symbol, CandlestickInterval interval, int limit, Action<CandlestickCacheEventArgs> callback);
    }
}
