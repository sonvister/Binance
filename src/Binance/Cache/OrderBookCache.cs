using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Binance.Cache.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public class OrderBookCache : WebSocketClientCache<IDepthWebSocketClient, DepthUpdateEventArgs, OrderBookCacheEventArgs>, IOrderBookCache
    {
        #region Public Properties

        public OrderBook OrderBook { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private OrderBook _orderBook;

        private string _symbol;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        public OrderBookCache(IBinanceApi api, IDepthWebSocketClient client, bool leaveClientOpen = false, ILogger<OrderBookCache> logger = null)
            : base(api, client, leaveClientOpen, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, null, limit, token);

        public Task SubscribeAsync(string symbol, Action<OrderBookCacheEventArgs> callback, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            _symbol = symbol;
            _limit = limit;
            Token = token;

            LinkTo(Client, callback, LeaveClientOpen);

            return Client.SubscribeAsync(symbol, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLinkTo()
        {
            Client.DepthUpdate += OnClientEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override async Task<OrderBookCacheEventArgs> OnAction(DepthUpdateEventArgs @event)
        {
            // If order book has not been initialized.
            if (_orderBook == null)
            {
                _orderBook = await Api.GetOrderBookAsync(_symbol, token: Token)
                    .ConfigureAwait(false);
            }

            // If there is a gap in events (order book out-of-sync).
            // ReSharper disable once InvertIf
            if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
            {
                Logger?.LogError($"{nameof(OrderBookCache)}: Synchronization failure (first update ID > last update ID + 1).");

                await Task.Delay(1000, Token)
                    .ConfigureAwait(false); // wait a bit.

                // Re-synchronize.
                _orderBook = await Api.GetOrderBookAsync(_symbol, token: Token)
                    .ConfigureAwait(false);

                // If still out-of-sync.
                // ReSharper disable once InvertIf
                if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                {
                    Logger?.LogError($"{nameof(OrderBookCache)}: Re-Synchronization failure (first update ID > last update ID + 1).");

                    // Reset and wait for next event.
                    _orderBook = null;
                    return null;
                }
            }

            return Modify(@event.LastUpdateId, @event.Bids, @event.Asks, _limit);
        }

        /// <summary>
        /// Update the order book.
        /// </summary>
        /// <param name="lastUpdateId"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        /// <param name="limit"></param>
        protected virtual OrderBookCacheEventArgs Modify(long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks, int limit)
        {
            if (lastUpdateId < _orderBook.LastUpdateId)
                return null;

            _orderBook.Modify(lastUpdateId, bids, asks);

            OrderBook = limit > 0 ? _orderBook.Clone(limit) : _orderBook.Clone();

            return new OrderBookCacheEventArgs(OrderBook);
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.DepthUpdate -= OnClientEvent;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
