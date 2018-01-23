using System;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;

namespace Binance.Cache
{
    /// <summary>
    /// A live depth of market with update events to maintain an order book cache.
    /// </summary>
    public interface IOrderBookCache
    {
        #region Events

        /// <summary>
        /// Order book cache update event.
        /// </summary>
        event EventHandler<OrderBookCacheEventArgs> Update;

        #endregion Events

        #region Properties

        /// <summary>
        /// The order book. Can be null if not yet synchronized or out-of-sync.
        /// </summary>
        OrderBook OrderBook { get; }
        
        /// <summary>
        /// The client that provides order book synchronization.
        /// </summary>
        IDepthWebSocketClient Client { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Subscribe the web socket client to the symbol and link this cache to client.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback">An event callback (optional).</param>
        void Subscribe(string symbol, int limit, Action<OrderBookCacheEventArgs> callback);

        /// <summary>
        /// Link to a subscribed <see cref="IDepthWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        void LinkTo(IDepthWebSocketClient client, Action<OrderBookCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Methods
    }
}
