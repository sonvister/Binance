using Binance.Orders.Book;
using Binance.Orders.Book.Cache;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    /// <summary>
    /// A live depth of market with update events to maintain an order book cache.
    /// </summary>
    public interface IOrderBookCache : IDisposable
    {
        #region Public Events

        /// <summary>
        /// Order book cache update event.
        /// </summary>
        event EventHandler<OrderBookCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The order book.
        /// </summary>
        OrderBook OrderBook { get; }
        
        /// <summary>
        /// The client that provides order book synchronization.
        /// </summary>
        IDepthWebSocketClient Client { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Subscribe the client to the symbol and synchronize this depth of
        /// market with updates processed by the current <see cref="Thread"/>
        /// or <see cref="Task"/>. This method will not return until the token
        /// is canceled, this <see cref="IOrderBookCache"> is disposed, or an
        /// internal exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Subscribe the client to the symbol and synchronize this depth of
        /// market with updates processed by the current <see cref="Thread"/>
        /// or <see cref="Task"/>. This method will not return until the token
        /// is canceled, this <see cref="IOrderBookCache"> is disposed, or an
        /// internal exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, Action<OrderBookCacheEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
