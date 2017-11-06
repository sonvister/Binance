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
    public class CandlesticksCache : WebSocketClientCache<ICandlestickWebSocketClient, CandlestickEventArgs, CandlesticksCacheEventArgs>, ICandlesticksCache
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

        public CandlesticksCache(IBinanceApi api, ICandlestickWebSocketClient client, bool leaveClientOpen = false, ILogger<CandlesticksCache> logger = null)
            : base(api, client, leaveClientOpen, logger)
        {
            _candlesticks = new List<Candlestick>();
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, CandlestickInterval interval, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, interval, null, limit, token);

        public Task SubscribeAsync(string symbol, CandlestickInterval interval, Action<CandlesticksCacheEventArgs> callback, int limit = default, CancellationToken token = default)
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

        #endregion Public Methods

        #region Protected Methods

        protected override void OnLinkTo()
        {
            Client.Candlestick += OnClientEvent;
        }

        protected override async Task<CandlesticksCacheEventArgs> OnAction(CandlestickEventArgs @event)
        {
            if (_candlesticks.Count == 0)
            {
                await SynchronizeCandlesticksAsync(_symbol, _interval, _limit, Token)
                    .ConfigureAwait(false);
            }

            Logger?.LogDebug($"{nameof(CandlesticksCache)}: Updating candlestick [open time: {@event.Candlestick.OpenTime}].");

            // Get the candlestick with matching open time.
            var candlestick = _candlesticks.FirstOrDefault(c => c.OpenTime == @event.Candlestick.OpenTime);

            lock (_sync)
            {
                _candlesticks.Remove(candlestick ?? _candlesticks.First());
                _candlesticks.Add(@event.Candlestick);
            }

            return new CandlesticksCacheEventArgs(_candlesticks.ToArray());
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
            Logger?.LogInformation($"{nameof(CandlesticksCache)}: Synchronizing candlesticks...");

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
                Client.Candlestick -= OnClientEvent;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
