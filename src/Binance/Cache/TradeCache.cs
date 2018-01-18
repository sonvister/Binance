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
    public sealed class TradeCache : WebSocketClientCache<ITradeWebSocketClient, TradeEventArgs, TradeCacheEventArgs>, ITradeCache
    {
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

        public TradeCache(IBinanceApi api, ITradeWebSocketClient client, ILogger<TradeCache> logger = null)
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
                throw new ArgumentException($"{nameof(TradeCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            _symbol = symbol.FormatSymbol();
            _limit = limit;

            base.LinkTo(Client, callback);

            Client.Subscribe(symbol, ClientCallback);
        }

        public override void LinkTo(ITradeWebSocketClient client, Action<TradeCacheEventArgs> callback = null)
        {
            // Confirm client is subscribed to only one stream.
            if (client.WebSocket.IsCombined)
                throw new InvalidOperationException($"{nameof(TradeCache)} can only link to {nameof(ITradeWebSocketClient)} events from a single stream (not combined streams).");

            base.LinkTo(client, callback);
            Client.Trade += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.Trade -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override async Task<TradeCacheEventArgs> OnAction(TradeEventArgs @event)
        {
            if (_trades.Count == 0)
            {
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);
            }

            // If there is a gap in the trades received (out-of-sync).
            if (@event.Trade.Id > _trades.Last().Id + 1)
            {
                Logger?.LogError($"{nameof(TradeCache)}: Synchronization failure (trade ID > last trade ID + 1).");

                await Task.Delay(1000, Token)
                    .ConfigureAwait(false); // wait a bit.

                // Re-synchronize.
                await SynchronizeTradesAsync(_symbol, _limit, Token)
                    .ConfigureAwait(false);

                // If still out-of-sync.
                if (@event.Trade.Id > _trades.Last().Id + 1)
                {
                    Logger?.LogError($"{nameof(TradeCache)}: Re-Synchronization failure (trade ID > last trade ID + 1).");

                    // Reset and wait for next event.
                    lock (_sync) _trades.Clear();
                    return null;
                }
            }

            // If the trade exists in the queue already (occurs after synchronization).
            if (_trades.Any(t => t.Id == @event.Trade.Id))
                return null;

            Trade removed;
            lock (_sync)
            {
                removed = _trades.Dequeue();
                _trades.Enqueue(@event.Trade);
            }

            Logger?.LogDebug($"{nameof(TradeCache)}: Added trade [ID: {@event.Trade.Id}] and removed [ID: {removed.Id}].");

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
            Logger?.LogInformation($"{nameof(TradeCache)}: Synchronizing trades...");

            var trades = await Api.GetTradesAsync(symbol, limit, token)
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
