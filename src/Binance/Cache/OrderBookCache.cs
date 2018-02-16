using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Client.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    /// <summary>
    /// The default <see cref="IOrderBookCache"/> implementation.
    /// </summary>
    public sealed class OrderBookCache : JsonClientCache<IDepthClient, DepthUpdateEventArgs, OrderBookCacheEventArgs>, IOrderBookCache
    {
        #region Public Events

        public event EventHandler<EventArgs> OutOfSync;

        #endregion Public Events

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

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="IDepthClient"/>, but no logger.
        /// </summary>
        public OrderBookCache()
            : this(new BinanceApi(), new DepthClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public OrderBookCache(IBinanceApi api, IDepthClient client, ILogger<OrderBookCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, int limit, Action<OrderBookCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(OrderBookCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (_symbol != null)
                throw new InvalidOperationException($"{nameof(OrderBookCache)}.{nameof(Subscribe)}: Already subscribed to a symbol: \"{_symbol}\"");

            _symbol = symbol.FormatSymbol();
            _limit = limit;

            OnSubscribe(callback);
            SubscribeToClient();
        }

        public override void Unsubscribe()
        {
            if (_symbol == null)
                return;

            UnsubscribeFromClient();
            OnUnsubscribe();

            _orderBookClone = _orderBook = null;

            _symbol = null;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (_symbol == null)
                return;

            Client.Subscribe(_symbol, _limit, ClientCallback);
        }

        protected override void UnsubscribeFromClient()
        {
            if (_symbol == null)
                return;

            Client.Unsubscribe(_symbol, _limit, ClientCallback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected override async ValueTask<OrderBookCacheEventArgs> OnAction(DepthUpdateEventArgs @event)
        {
            if (_limit > 0)
            {
                // Ignore events with same or earlier order book update.
                if (_orderBookClone != null && @event.LastUpdateId <= _orderBookClone.LastUpdateId)
                {
                    Logger?.LogDebug($"{nameof(OrderBookCache)} ({_symbol}): Ignoring event (last update ID: {@event.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
                    return null;
                }

                // Top <limit> bids and asks, pushed every second.
                // NOTE: LastUpdateId is not contiguous between events when using partial depth stream.
                _orderBookClone = new OrderBook(_symbol, @event.LastUpdateId, @event.Bids, @event.Asks);
            }
            else
            {
                // If order book has not been initialized or is out-of-sync (gap in data).
                while (_orderBook == null || @event.FirstUpdateId > _orderBook.LastUpdateId + 1)
                {
                    if (_orderBook != null)
                    {
                        OutOfSync?.Invoke(this, EventArgs.Empty);
                    }

                    // Synchronize.
                    await SynchronizeOrderBookAsync(@event.Token)
                        .ConfigureAwait(false);
                }

                // Ignore events prior to order book snapshot.
                if (@event.LastUpdateId <= _orderBook.LastUpdateId)
                {
                    Logger?.LogDebug($"{nameof(OrderBookCache)} ({_symbol}): Ignoring event (last update ID: {@event.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
                    return null;
                }

                Logger?.LogDebug($"{nameof(OrderBookCache)} ({_symbol}): Updating order book (last update ID: {_orderBook.LastUpdateId} => {@event.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

                _orderBook.Modify(@event.LastUpdateId, @event.Bids, @event.Asks);

                _orderBookClone = _orderBook.Clone();
            }

            return new OrderBookCacheEventArgs(_orderBookClone);
        }

        #endregion Protected Methods

        #region Private Methods

        private async Task SynchronizeOrderBookAsync(CancellationToken token)
        {
            Logger?.LogInformation($"{nameof(OrderBookCache)} ({_symbol}): Synchronizing order book...  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

            // Get order book snapshot with the maximum limit.
            _orderBook = await Api.GetOrderBookAsync(_symbol, 1000, token) // TODO
                .ConfigureAwait(false);

            Logger?.LogInformation($"{nameof(OrderBookCache)} ({_symbol}): Synchronization complete (last update ID: {_orderBook.LastUpdateId}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
        }

        #endregion Private Methods
    }
}
