using System;
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

        public async Task SubscribeAsync(string symbol, int limit, Action<OrderBookCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(OrderBookCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            _symbol = symbol.FormatSymbol();
            _limit = limit;
            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(_symbol, limit, token)
                    .ConfigureAwait(false);
            }
            finally { UnLink(); }
        }

        public override void LinkTo(IDepthWebSocketClient client, Action<OrderBookCacheEventArgs> callback = null)
        {
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
                    await SynchronizeOrderBookAsync()
                        .ConfigureAwait(false);
                }

                // Ignore events prior to order book snapshot.
                if (@event.LastUpdateId <= _orderBook.LastUpdateId)
                    return null;

                // If there is a gap in events (order book out-of-sync).
                // ReSharper disable once InvertIf
                if (@event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                {
                    Logger?.LogError($"{nameof(OrderBookCache)}: Synchronization failure (first update ID > last update ID + 1).");

                    await Task.Delay(1000, Token)
                        .ConfigureAwait(false); // wait a bit.

                    // Re-synchronize.
                    await SynchronizeOrderBookAsync()
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

                Logger?.LogDebug($"{nameof(OrderBookCache)}: Updating order book [last update ID: {@event.LastUpdateId}].");

                _orderBook.Modify(@event.LastUpdateId, @event.Bids, @event.Asks);

                _orderBookClone = _orderBook.Clone();
            }

            return new OrderBookCacheEventArgs(_orderBookClone);
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task SynchronizeOrderBookAsync()
        {
            Logger?.LogInformation($"{nameof(OrderBookCache)}: Synchronizing order book...");

            // Get order book snapshot with the maximum limit.
            _orderBook = await Api.GetOrderBookAsync(_symbol, 1000, Token)
                .ConfigureAwait(false);
        }

        #endregion Private Methods
    }
}
