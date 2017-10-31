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
    public class CandlesticksCache : WebSocketClientCache<IKlineWebSocketClient, KlineEventArgs, CandlesticksCacheEventArgs>, ICandlesticksCache
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
        private KlineInterval _interval;
        private int _limit;

        #endregion Private Fields

        #region Constructors

        public CandlesticksCache(IBinanceApi api, IKlineWebSocketClient client, bool leaveClientOpen = false, ILogger<CandlesticksCache> logger = null)
            : base(api, client, leaveClientOpen, logger)
        {
            _candlesticks = new List<Candlestick>();
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, KlineInterval interval, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, interval, null, limit, token);

        public Task SubscribeAsync(string symbol, KlineInterval interval, Action<CandlesticksCacheEventArgs> callback, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            _symbol = symbol;
            _interval = interval;
            _limit = limit;
            Token = token;

            LinkTo(Client, callback, LeaveClientOpen);

            return Client.SubscribeAsync(symbol, interval, token);
        }

        protected override void OnLinkTo()
        {
            Client.Kline += OnClientEvent;
        }

        protected override async Task<CandlesticksCacheEventArgs> OnAction(KlineEventArgs @event)
        {
            if (_candlesticks.Count == 0)
            {
                await SynchronizeCandlesticksAsync(_symbol, _interval, _limit, Token)
                    .ConfigureAwait(false);
            }

            var candlestick = _candlesticks.FirstOrDefault(c => c.OpenTime == @event.Candlestick.OpenTime);

            lock (_sync)
            {
                _candlesticks.Remove(candlestick ?? _candlesticks.First());
                _candlesticks.Add(@event.Candlestick);
            }

            return new CandlesticksCacheEventArgs(_candlesticks.ToArray());
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Get latest trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task SynchronizeCandlesticksAsync(string symbol, KlineInterval interval, int limit, CancellationToken token)
        {
            var candlesticks = await Api.GetCandlesticksAsync(symbol, interval, limit, token: token)
                .ConfigureAwait(false);

            lock (_sync)
            {
                _candlesticks.Clear();
                foreach (var candlestick in candlesticks)
                {
                    _candlesticks.Add(candlestick);
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
                Client.Kline -= OnClientEvent;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
