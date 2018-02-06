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
    public sealed class CandlestickCache : WebSocketClientCache<ICandlestickWebSocketClient, CandlestickEventArgs, CandlestickCacheEventArgs>, ICandlestickCache
    {
        #region Public Properties

        public IEnumerable<Candlestick> Candlesticks
        {
            get { lock (_sync) { return _candlesticks?.ToArray() ?? new Candlestick[] { }; } }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly List<Candlestick> _candlesticks;

        private readonly object _sync = new object();

        private string _symbol;
        private CandlestickInterval _interval;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        public CandlestickCache(IBinanceApi api, ICandlestickWebSocketClient client, ILogger<CandlestickCache> logger = null)
            : base(api, client, logger)
        {
            _candlesticks = new List<Candlestick>();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, CandlestickInterval interval, int limit, Action<CandlestickCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (limit < 0)
                throw new ArgumentException($"{nameof(CandlestickCache)}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (_symbol != null)
                throw new InvalidOperationException($"{nameof(CandlestickCache)}.{nameof(Subscribe)}: Already subscribed to a symbol: \"{_symbol}\"");

            _symbol = symbol.FormatSymbol();
            _interval = interval;
            _limit = limit;

            base.LinkTo(Client, callback);

            Client.Subscribe(_symbol, _interval, ClientCallback);
        }

        public void Unsubscribe()
        {
            if (_symbol == null)
                return;

            Client.Unsubscribe(_symbol, _interval, ClientCallback);

            UnLink();

            lock (_sync)
            {
                _candlesticks.Clear();
            }

            _symbol = null;
        }

        public override void LinkTo(ICandlestickWebSocketClient client, Action<CandlestickCacheEventArgs> callback = null)
        {
            // Confirm client is subscribed to only one stream.
            if (client.WebSocket.IsCombined)
                throw new InvalidOperationException($"{nameof(CandlestickCache)} can only link to {nameof(ICandlestickWebSocketClient)} events from a single stream (not combined streams).");

            base.LinkTo(client, callback);
            Client.Candlestick += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.Candlestick -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override async ValueTask<CandlestickCacheEventArgs> OnAction(CandlestickEventArgs @event)
        {
            if (_candlesticks.Count == 0)
            {
                await SynchronizeCandlesticksAsync(_symbol, _interval, _limit, @event.Token)
                    .ConfigureAwait(false);
            }

            Logger?.LogDebug($"{nameof(CandlestickCache)} ({_symbol}): Updating candlestick (open time: {@event.Candlestick.OpenTime}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(@event.Token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

            // Get the candlestick with matching open time.
            var candlestick = _candlesticks.FirstOrDefault(c => c.OpenTime == @event.Candlestick.OpenTime);

            lock (_sync)
            {
                _candlesticks.Remove(candlestick ?? _candlesticks.First());
                _candlesticks.Add(@event.Candlestick);
            }

            return new CandlestickCacheEventArgs(_candlesticks.ToArray());
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Get latest trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task SynchronizeCandlesticksAsync(string symbol, CandlestickInterval interval, int limit, CancellationToken token)
        {
            Logger?.LogInformation($"{nameof(CandlestickCache)} ({_symbol}): Synchronizing candlesticks...  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

            var candlesticks = await Api.GetCandlesticksAsync(symbol, interval, limit, token: token)
                .ConfigureAwait(false);

            lock (_sync)
            {
                _candlesticks.Clear();
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var candlestick in candlesticks)
                {
                    _candlesticks.Add(candlestick);
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            Logger?.LogInformation($"{nameof(CandlestickCache)} ({_symbol}): Synchronization complete (latest open time: {candlesticks.Last().OpenTime}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
        }

        #endregion Private Methods
    }
}
