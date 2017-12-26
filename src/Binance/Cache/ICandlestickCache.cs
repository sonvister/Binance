using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket;
using Binance.Cache.Events;
using Binance.Market;

namespace Binance.Cache
{
    public interface ICandlestickCache
    {
        #region Public Events

        /// <summary>
        /// Candlesticks update event.
        /// </summary>
        event EventHandler<CandlestickCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The candlesticks. Can be empty if not yet synchronized or out-of-sync.
        /// </summary>
        IEnumerable<Candlestick> Candlesticks { get; }

        /// <summary>
        /// The client that provides candlestick information.
        /// </summary>
        ICandlestickWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(string symbol, CandlestickInterval interval, int limit, Action<CandlestickCacheEventArgs> callback, CancellationToken token);

        /// <summary>
        /// Link to a subscribed <see cref="ICandlestickWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        void LinkTo(ICandlestickWebSocketClient client, Action<CandlestickCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Public Methods
    }
}
