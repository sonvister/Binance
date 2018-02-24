using System;
using System.Collections.Generic;
using System.Linq;
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
    /// The default <see cref="IAggregateTradeCache"/> implemenation.
    /// </summary>
    public sealed class AggregateTradeCache : JsonClientCache<IAggregateTradeClient, AggregateTradeEventArgs, AggregateTradeCacheEventArgs>, IAggregateTradeCache
    {
        #region Public Events

        public event EventHandler<EventArgs> OutOfSync;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<AggregateTrade> Trades
        {
            get { lock (_sync) { return _trades?.ToArray() ?? new AggregateTrade[] { }; } }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly Queue<AggregateTrade> _trades;

        private readonly object _sync = new object();

        private string _symbol;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="IAggregateTradeClient"/>, but no logger.
        /// </summary>
        public AggregateTradeCache()
            : this(new BinanceApi(), new AggregateTradeClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeCache(IBinanceApi api, IAggregateTradeClient client, ILogger<AggregateTradeCache> logger = null)
            : base(api, client, logger)
        {
            _trades = new Queue<AggregateTrade>();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, int limit, Action<AggregateTradeCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(AggregateTradeCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (_symbol != null)
                throw new InvalidOperationException($"{nameof(AggregateTradeCache)}.{nameof(Subscribe)}: Already subscribed to a symbol: \"{_symbol}\"");

            _symbol = symbol.FormatSymbol();
            _limit = limit;

            OnSubscribe(callback);
            SubscribeToClient();
        }

        public override IJsonClient Unsubscribe()
        {
            if (_symbol == null)
                return this;

            UnsubscribeFromClient();
            OnUnsubscribe();

            lock (_sync)
            {
                _trades.Clear();
            }

            _symbol = default;
            _limit = default;

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (_symbol == null)
                return;

            Client.Subscribe(_symbol, ClientCallback);
        }

        protected override void UnsubscribeFromClient()
        {
            if (_symbol == null)
                return;

            Client.Unsubscribe(_symbol, ClientCallback);
        }

        protected override async ValueTask<AggregateTradeCacheEventArgs> OnAction(AggregateTradeEventArgs @event)
        {
            var synchronize = false;

            // If trades have not been initialized or are out-of-sync (gap in data).
            lock (_sync)
            {
                if (_trades.Count == 0 || @event.Trade.Id > _trades.Last().Id + 1)
                {
                    if (_trades.Count > 0)
                    {
                        OutOfSync?.Invoke(this, EventArgs.Empty);
                    }

                    synchronize = true;
                }
            }

            if (synchronize)
            {
                await SynchronizeTradesAsync(_symbol, _limit, @event.Token)
                    .ConfigureAwait(false);
            }

            lock (_sync)
            {
                if (_trades.Count == 0 || @event.Trade.Id > _trades.Last().Id + 1)
                {
                    Logger?.LogError($"{nameof(AggregateTradeCache)} ({_symbol}): Failed to synchronize trades.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    return null;
                }

                // Ignore trades older than the latest trade in queue.
                if (@event.Trade.Id <= _trades.Last().Id)
                {
                    Logger?.LogDebug($"{nameof(AggregateTradeCache)} ({_symbol}): Ignoring event (trade ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    return null;
                }

                var removed = _trades.Dequeue();
                Logger?.LogTrace($"{nameof(AggregateTradeCache)} ({_symbol}): REMOVE aggregate trade (ID: {removed.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                _trades.Enqueue(@event.Trade);
                Logger?.LogTrace($"{nameof(AggregateTradeCache)} ({_symbol}): ADD aggregate trade (ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                return new AggregateTradeCacheEventArgs(_trades.ToArray());
            }
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Get latest trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task SynchronizeTradesAsync(string symbol, int limit, CancellationToken token)
        {
            Logger?.LogInformation($"{nameof(AggregateTradeCache)} ({_symbol}): Synchronizing aggregate trades...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            var trades = await Api.GetAggregateTradesAsync(symbol, limit, token)
                .ConfigureAwait(false);

            lock (_sync)
            {
                _trades.Clear();
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var trade in trades)
                {
                    _trades.Enqueue(trade);
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            Logger?.LogInformation($"{nameof(AggregateTradeCache)} ({_symbol}): Synchronization complete (latest trade ID: {trades.Last().Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
        }

        #endregion Private Methods
    }
}
