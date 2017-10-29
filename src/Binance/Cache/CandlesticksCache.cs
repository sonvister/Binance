using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Binance.Cache.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
// ReSharper disable InconsistentlySynchronizedField

namespace Binance.Cache
{
    public class CandlesticksCache : ICandlesticksCache
    {
        #region Public Events

        public event EventHandler<CandlesticksCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<Candlestick> Candlesticks
        {
            get { lock (_sync) { return _candlesticks?.ToArray() ?? new Candlestick[] { }; } }
        }

        public IKlineWebSocketClient Client { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly IBinanceApi _api;

        private readonly ILogger<CandlesticksCache> _logger;

        private bool _leaveClientOpen;

        private BufferBlock<KlineEventArgs> _bufferBlock;
        private ActionBlock<KlineEventArgs> _actionBlock;

        private Action<CandlesticksCacheEventArgs> _callback;

        private readonly List<Candlestick> _candlesticks;

        private readonly object _sync = new object();

        private string _symbol;
        private KlineInterval _interval;
        private int _limit;
        private CancellationToken _token;

        #endregion Private Fields

        #region Constructors

        public CandlesticksCache(IBinanceApi api, IKlineWebSocketClient client, bool leaveClientOpen = false, ILogger<CandlesticksCache> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            _api = api;
            _logger = logger;

            Client = client;
            _leaveClientOpen = leaveClientOpen;

            _candlesticks = new List<Candlestick>();
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(string symbol, KlineInterval interval, int limit = default, CancellationToken token = default)
            => SubscribeAsync(symbol, interval, null, limit, token);

        public Task SubscribeAsync(string symbol, KlineInterval interval, Action<CandlesticksCacheEventArgs> callback, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            _symbol = symbol;
            _interval = interval;
            _limit = limit;
            _token = token;

            LinkTo(Client, callback, _leaveClientOpen);

            return Client.SubscribeAsync(symbol, interval, token);
        }

        public void LinkTo(IKlineWebSocketClient client, Action<CandlesticksCacheEventArgs> callback = null, bool leaveClientOpen = true)
        {
            Throw.IfNull(client, nameof(client));

            if (_bufferBlock != null)
            {
                if (client == Client)
                    throw new InvalidOperationException($"{nameof(CandlesticksCache)} is already linked to this {nameof(IKlineWebSocketClient)}.");

                throw new InvalidOperationException($"{nameof(CandlesticksCache)} is linked to another {nameof(IKlineWebSocketClient)}.");
            }

            _callback = callback;
            _leaveClientOpen = leaveClientOpen;

            _bufferBlock = new BufferBlock<KlineEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = _token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
            });

            _actionBlock = new ActionBlock<KlineEventArgs>(async @event =>
            {
                try
                {
                    if (_candlesticks.Count == 0)
                    {
                        await SynchronizeCandlesticksAsync(_symbol, _interval, _limit, _token)
                            .ConfigureAwait(false);
                    }

                    var candlestick = _candlesticks.FirstOrDefault(c => c.OpenTime == @event.Candlestick.OpenTime);

                    lock (_sync)
                    {
                        _candlesticks.Remove(candlestick ?? _candlesticks.First());
                        _candlesticks.Add(@event.Candlestick);
                    }

                    var eventArgs = new CandlesticksCacheEventArgs(_candlesticks.ToArray());

                    _callback?.Invoke(eventArgs);
                    RaiseUpdateEvent(eventArgs);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(CandlesticksCache)}: \"{e.Message}\"");
                }
            }, new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = 1,
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                CancellationToken = _token,
                SingleProducerConstrained = true,
            });

            _bufferBlock.LinkTo(_actionBlock);

            Client.Kline += OnKline;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise candlesticks cache update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(CandlesticksCacheEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Update?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(CandlesticksCache)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
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
        private async Task SynchronizeCandlesticksAsync(string symbol, KlineInterval interval, int limit, CancellationToken token)
        {
            var candlesticks = await _api.GetCandlesticksAsync(symbol, interval, limit: limit, token: token)
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

        /// <summary>
        /// <see cref="IKlineWebSocketClient"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="IKlineWebSocketClient"/>.</param>
        /// <param name="event">The event arguments.</param>
        private void OnKline(object sender, KlineEventArgs @event)
        {
            // Post event to buffer block (queue).
            _bufferBlock.Post(@event);
        }

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        private void LogException(Exception e, string source)
        {
            if (!e.IsLogged())
            {
                _logger?.LogError(e, $"{source}: \"{e.Message}\"");
                e.Logged();
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.Kline -= OnKline;

                if (!_leaveClientOpen)
                {
                    Client.Dispose();
                }

                _bufferBlock?.Complete();
                _actionBlock?.Complete();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
