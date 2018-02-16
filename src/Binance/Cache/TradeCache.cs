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
    /// The default <see cref="ITradeCache"/> implemenation.
    /// </summary>
    public sealed class TradeCache : JsonClientCache<ITradeClient, TradeEventArgs, TradeCacheEventArgs>, ITradeCache
    {
        #region Public Events

        public event EventHandler<EventArgs> OutOfSync;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<Trade> Trades
        {
            get { lock (_sync) { return _trades?.ToArray() ?? new Trade[] { }; } }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly Queue<Trade> _trades;

        private readonly object _sync = new object();

        private string _symbol;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="ITradeClient"/>, but no logger.
        /// </summary>
        public TradeCache()
            : this(new BinanceApi(), new TradeClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeCache(IBinanceApi api, ITradeClient client, ILogger<TradeCache> logger = null)
            : base(api, client, logger)
        {
            _trades = new Queue<Trade>();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, int limit, Action<TradeCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(TradeCache)}.{nameof(Subscribe)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (_symbol != null)
                throw new InvalidOperationException($"{nameof(TradeCache)}.{nameof(Subscribe)}: Already subscribed to a symbol: \"{_symbol}\"");

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

            lock (_sync)
            {
                _trades.Clear();
            }

            _symbol = null;
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

        protected override async ValueTask<TradeCacheEventArgs> OnAction(TradeEventArgs @event)
        {
            // If trades have not been initialized or are out-of-sync (gap in data).
            while (_trades.Count == 0 || @event.Trade.Id > _trades.Last().Id + 1)
            {
                if (_trades.Count > 0)
                {
                    OutOfSync?.Invoke(this, EventArgs.Empty);
                }

                await SynchronizeTradesAsync(_symbol, _limit, @event.Token)
                    .ConfigureAwait(false);
            }

            lock (_sync)
            {
                // Ignore trades older than the latest trade in queue.
                if (@event.Trade.Id <= _trades.Last().Id)
                {
                    Logger?.LogDebug($"{nameof(TradeCache)} ({_symbol}): Ignoring event (trade ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
                    return null;
                }

                var removed = _trades.Dequeue();
                Logger?.LogDebug($"{nameof(TradeCache)} ({_symbol}): REMOVE trade (ID: {removed.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

                _trades.Enqueue(@event.Trade);
                Logger?.LogDebug($"{nameof(TradeCache)} ({_symbol}): ADD trade (ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
            }

            return new TradeCacheEventArgs(_trades.ToArray());
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
            Logger?.LogInformation($"{nameof(TradeCache)} ({_symbol}): Synchronizing trades...  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

            var trades = await Api.GetTradesAsync(symbol, limit, token)
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
            Logger?.LogInformation($"{nameof(TradeCache)} ({_symbol}): Synchronization complete (latest trade ID: {trades.Last().Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
        }

        #endregion Private Methods
    }
}
