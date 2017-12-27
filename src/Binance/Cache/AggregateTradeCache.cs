using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Binance.Cache.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;

// ReSharper disable InconsistentlySynchronizedField

namespace Binance.Cache
{
    public sealed class AggregateTradeCache : WebSocketClientCache<IAggregateTradeWebSocketClient, AggregateTradeEventArgs, AggregateTradeCacheEventArgs>, IAggregateTradeCache
    {
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

        public async Task SubscribeAsync(string symbol, int limit, Action<AggregateTradeCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            _symbol = symbol.FormatSymbol();
            _limit = limit;
            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(_symbol, token)
                    .ConfigureAwait(false);
            }
            finally { UnLink(); }
        }

        public override void LinkTo(IAggregateTradeWebSocketClient client, Action<AggregateTradeCacheEventArgs> callback = null)
        {
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

        protected override async Task<AggregateTradeCacheEventArgs> OnAction(AggregateTradeEventArgs @event)
        {
            if (_trades.Count == 0)
            {
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);
            }

            // If there is a gap in the trades received (out-of-sync).
            if (@event.Trade.Id > _trades.Last().Id + 1)
            {
                Logger?.LogError($"{nameof(AggregateTradeCache)}: Synchronization failure (trade ID > last trade ID + 1).");

                await Task.Delay(1000, Token)
                    .ConfigureAwait(false); // wait a bit.

                // Re-synchronize.
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);

                // If still out-of-sync.
                if (@event.Trade.Id > _trades.Last().Id + 1)
                {
                    Logger?.LogError($"{nameof(AggregateTradeCache)}: Re-Synchronization failure (trade ID > last trade ID + 1).");

                    // Reset and wait for next event.
                    lock (_sync) _trades.Clear();
                    return null;
                }
            }

            // If the trade exists in the queue already (occurs after synchronization).
            if (_trades.Any(t => t.Id == @event.Trade.Id))
                return null;

            AggregateTrade removed;
            lock (_sync)
            {
                removed = _trades.Dequeue();
                _trades.Enqueue(@event.Trade);
            }

            Logger?.LogDebug($"{nameof(AggregateTradeCache)}: Added aggregate trade [ID: {@event.Trade.Id}] and removed [ID: {removed.Id}].");

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
            Logger?.LogInformation($"{nameof(AggregateTradeCache)}: Synchronizing aggregate trades...");

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
        }

        #endregion Private Methods
    }
}
