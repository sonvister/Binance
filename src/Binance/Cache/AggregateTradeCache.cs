using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache.Events;
using Binance.Market;
using Binance.WebSocket;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentlySynchronizedField

namespace Binance.Cache
{
    public sealed class AggregateTradeCache : WebSocketClientCache<IAggregateTradeWebSocketClient, AggregateTradeEventArgs, AggregateTradeCacheEventArgs>, IAggregateTradeCache
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

        public AggregateTradeCache(IBinanceApi api, IAggregateTradeWebSocketClient client, ILogger<AggregateTradeCache> logger = null)
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

            _symbol = symbol.FormatSymbol();
            _limit = limit;

            base.LinkTo(Client, callback);

            Client.Subscribe(symbol, ClientCallback);
        }

        public override void LinkTo(IAggregateTradeWebSocketClient client, Action<AggregateTradeCacheEventArgs> callback = null)
        {
            // Confirm client is subscribed to only one stream.
            if (client.WebSocket.IsCombined)
                throw new InvalidOperationException($"{nameof(AggregateTradeCache)} can only link to {nameof(IAggregateTradeWebSocketClient)} events from a single stream (not combined streams).");

            base.LinkTo(client, callback);
            Client.AggregateTrade += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.AggregateTrade -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override async ValueTask<AggregateTradeCacheEventArgs> OnAction(AggregateTradeEventArgs @event)
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

            // Ignore trades older than the latest trade in queue.
            if (@event.Trade.Id <= _trades.Last().Id)
            {
                Logger?.LogDebug($"{nameof(AggregateTradeCache)} ({_symbol}): Ignoring event (trade ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
                return null;
            }

            lock (_sync)
            {
                var removed = _trades.Dequeue();
                Logger?.LogDebug($"{nameof(AggregateTradeCache)} ({_symbol}): REMOVE aggregate trade (ID: {removed.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

                _trades.Enqueue(@event.Trade);
                Logger?.LogDebug($"{nameof(AggregateTradeCache)} ({_symbol}): ADD aggregate trade (ID: {@event.Trade.Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
            }

            return new AggregateTradeCacheEventArgs(_trades.ToArray());
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
            Logger?.LogInformation($"{nameof(AggregateTradeCache)} ({_symbol}): Synchronizing aggregate trades...  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

            var trades = await Api.GetAggregateTradesAsync(symbol, limit, token)
                .ConfigureAwait(false);

            lock (_sync)
            {
                _trades.Clear();
                foreach (var trade in trades)
                {
                    _trades.Enqueue(trade);
                }
            }

            Logger?.LogInformation($"{nameof(AggregateTradeCache)} ({_symbol}): Synchronization complete (latest trade ID: {trades.Last().Id}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
        }

        #endregion Private Methods
    }
}
