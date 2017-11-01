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
    public class AggregateTradesCache : WebSocketClientCache<ITradesWebSocketClient, AggregateTradeEventArgs, AggregateTradesCacheEventArgs>, IAggregateTradesCache
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

        public AggregateTradesCache(IBinanceApi api, ITradesWebSocketClient client, bool leaveClientOpen = false, ILogger<AggregateTradesCache> logger = null)
            : base(api, client, leaveClientOpen, logger)
        {
            _trades = new Queue<AggregateTrade>();
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, null, limit, token);

        public Task SubscribeAsync(string symbol, Action<AggregateTradesCacheEventArgs> callback, int limit = default, CancellationToken token = default)
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

        protected override void OnLinkTo()
        {
            Client.AggregateTrade += OnClientEvent;
        }

        protected override async Task<AggregateTradesCacheEventArgs> OnAction(AggregateTradeEventArgs @event)
        {
            if (_trades.Count == 0)
            {
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);
            }

            // If there is a gap in the trades received (out-of-sync).
            if (@event.Trade.Id > _trades.Last().Id + 1)
            {
                Logger?.LogError($"{nameof(AggregateTradesCache)}: Synchronization failure (trade ID > last trade ID + 1).");

                await Task.Delay(1000, Token)
                    .ConfigureAwait(false); // wait a bit.

                // Re-synchronize.
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);

                // If still out-of-sync.
                if (@event.Trade.Id > _trades.Last().Id + 1)
                {
                    Logger?.LogError($"{nameof(AggregateTradesCache)}: Re-Synchronization failure (trade ID > last trade ID + 1).");

                    // Reset and wait for next event.
                    lock (_sync) _trades.Clear();
                    return null;
                }
            }

            // If the trade exists in the queue already (occurs after synchronization).
            if (_trades.Any(t => t.Id == @event.Trade.Id))
                return null;

            lock (_sync)
            {
                _trades.Dequeue();
                _trades.Enqueue(@event.Trade);
            }

            return new AggregateTradesCacheEventArgs(_trades.ToArray());
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
            Logger?.LogInformation($"{nameof(AggregateTradesCache)}: Synchronizing aggregate trades...");

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

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.AggregateTrade -= OnClientEvent;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
