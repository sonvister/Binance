using System;
using System.Collections.Generic;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Market;

namespace Binance.Cache
{
    public interface ICandlestickCache : IJsonClientCache<ICandlestickClient, CandlestickCacheEventArgs>
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
