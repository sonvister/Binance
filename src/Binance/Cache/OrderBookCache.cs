using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public sealed class OrderBookCache : WebSocketClientCache<IDepthWebSocketClient, DepthUpdateEventArgs, OrderBookCacheEventArgs>, IOrderBookCache
    {
        #region Public Properties

        public OrderBook OrderBook => _orderBookClone;

        #endregion Public Properties

        #region Private Fields

        private OrderBook _orderBook;
        private volatile OrderBook _orderBookClone;

        private string _symbol;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        public OrderBookCache(IBinanceApi api, IDepthWebSocketClient client, ILogger<OrderBookCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, int limit, Action<OrderBookCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(OrderBookCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            _symbol = symbol.FormatSymbol();
            _limit = limit;

            base.LinkTo(Client, callback);

            Client.Subscribe(symbol, limit, ClientCallback);
        }

        public override void LinkTo(IDepthWebSocketClient client, Action<OrderBookCacheEventArgs> callback = null)
        {
            // Confirm client is subscribed to only one stream.
            if (client.WebSocket.IsCombined)
                throw new InvalidOperationException($"{nameof(OrderBookCache)} can only link to {nameof(IDepthWebSocketClient)} events from a single stream (not combined streams).");

            base.LinkTo(client, callback);
            Client.DepthUpdate += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.DepthUpdate -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override async Task<OrderBookCacheEventArgs> OnAction(DepthUpdateEventArgs @event)
        {
            if (_limit > 0)
            {
                // Ignore events with same or earlier order book update.
                if (_orderBookClone != null && @event.LastUpdateId <= _orderBookClone.LastUpdateId)
                    return null;

                // Top <level> bids and asks, pushed every second.
                // NOTE: LastUpdateId is not contiguous between events when using partial depth stream.
                _orderBookClone = new OrderBook(_symbol, @event.LastUpdateId, @event.Bids, @event.Asks);
            }
            else
            {
                // If order book has not been initialized.
                if (_orderBook == null)
                {
                    // Synchronize.
                    await SynchronizeOrderBookAsync(@event.Token)
                        .ConfigureAwait(false);
                }

                // If there is a gap in events (order book out-of-sync).
                // ReSharper disable once InvertIf
                if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                {
                    Logger?.LogError($"{nameof(OrderBookCache)}: Synchronization failure ({@event.FirstUpdateId} > {_orderBook.LastUpdateId} + 1).  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {@event.Token.IsCancellationRequested}]");

                    await Task.Delay(1000, @event.Token)
                        .ConfigureAwait(false); // wait a bit.

                    // Re-synchronize.
                    await SynchronizeOrderBookAsync(@event.Token)
                        .ConfigureAwait(false);

                    // If still out-of-sync.
                    // ReSharper disable once InvertIf
                    if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                    {
                        Logger?.LogError($"{nameof(OrderBookCache)}: Re-Synchronization failure ({@event.FirstUpdateId} > {_orderBook.LastUpdateId} + 1).  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {@event.Token.IsCancellationRequested}]");

                        // Reset and wait for next event.
                        _orderBook = null;
                        return null;
                    }
                }

                // Ignore events prior to order book snapshot (occurs after synchronization).
                if (@event.LastUpdateId <= _orderBook.LastUpdateId)
                {
                    Logger?.LogDebug($"{nameof(OrderBookCache)}: Ignoring event (last update ID: {@event.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {@event.Token.IsCancellationRequested}]");
                    return null;
                }

                Logger?.LogDebug($"{nameof(OrderBookCache)}: Updating order book (last update ID: {_orderBook.LastUpdateId} => {@event.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {@event.Token.IsCancellationRequested}]");

                _orderBook.Modify(@event.LastUpdateId, @event.Bids, @event.Asks);

                _orderBookClone = _orderBook.Clone();
            }

            return new OrderBookCacheEventArgs(_orderBookClone);
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task SynchronizeOrderBookAsync(CancellationToken token)
        {
            Logger?.LogInformation($"{nameof(OrderBookCache)}: Synchronizing order book...  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {token.IsCancellationRequested}]");

            // Get order book snapshot with the maximum limit.
            _orderBook = await Api.GetOrderBookAsync(_symbol, 1000, token)
                .ConfigureAwait(false);

            Logger?.LogInformation($"{nameof(OrderBookCache)}: Synchronizing complete.  [thread: {Thread.CurrentThread.ManagedThreadId}, cancelled: {@token.IsCancellationRequested}]");
        }

        #endregion Private Methods
    }
}
