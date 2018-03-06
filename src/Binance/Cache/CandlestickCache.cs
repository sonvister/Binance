using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    /// <summary>
    /// The default <see cref="ICandlestickCache"/> implementation.
    /// </summary>
    public class CandlestickCache : CandlestickCache<ICandlestickClient>, ICandlestickCache
    {
        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="ICandlestickClient"/>, but no logger.
        /// </summary>
        public CandlestickCache()
            : this(new BinanceApi(), new CandlestickClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickCache(IBinanceApi api, ICandlestickClient client, ILogger<CandlestickCache> logger = null)
            : base(api, client, logger)
        { }
    }

    /// <summary>
    /// The default <see cref="ICandlestickCache{TClient}"/> implemenation.
    /// </summary>
    public abstract class CandlestickCache<TClient> : JsonClientCache<TClient, CandlestickEventArgs, CandlestickCacheEventArgs>, ICandlestickCache<TClient>
        where TClient : class, ICandlestickClient
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

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected CandlestickCache(IBinanceApi api, TClient client, ILogger<CandlestickCache<TClient>> logger = null)
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
                throw new ArgumentException($"{GetType().Name}: {nameof(limit)} must be greater than or equal to 0.", nameof(limit));

            if (_symbol != null)
                throw new InvalidOperationException($"{GetType().Name}.{nameof(Subscribe)}: Already subscribed to a symbol: \"{_symbol}\"");

            _symbol = symbol.FormatSymbol();
            _interval = interval;
            _limit = limit;

            OnSubscribe(callback);
            SubscribeToClient();
        }

        public override IJsonSubscriber Unsubscribe()
        {
            if (_symbol == null)
                return this;

            UnsubscribeFromClient();
            OnUnsubscribe();

            lock (_sync)
            {
                _candlesticks.Clear();
            }

            _symbol = default;
            _interval = default;
            _limit = default;

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (_symbol == null)
                return;

            Client.Subscribe(_symbol, _interval, ClientCallback);
        }

        protected override void UnsubscribeFromClient()
        {
            if (_symbol == null)
                return;

            Client.Unsubscribe(_symbol, _interval, ClientCallback);
        }

        protected override async ValueTask<CandlestickCacheEventArgs> OnActionAsync(CandlestickEventArgs @event, CancellationToken token = default)
        {
            bool synchronize;

            lock (_sync)
            {
                synchronize = _candlesticks.Count == 0;
            }

            if (synchronize)
            {
                await SynchronizeCandlesticksAsync(_symbol, _interval, _limit, token)
                    .ConfigureAwait(false);
            }

            Logger?.LogTrace($"{GetType().Name} ({_symbol}): Updating candlestick (open time: {@event.Candlestick.OpenTime}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            lock (_sync)
            {
                // Get the candlestick with matching open time.
                var candlestick = _candlesticks.FirstOrDefault(c => c.OpenTime == @event.Candlestick.OpenTime);

                _candlesticks.Remove(candlestick ?? _candlesticks.First());
                _candlesticks.Add(@event.Candlestick);

                return new CandlestickCacheEventArgs(_candlesticks.ToArray());
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
        private async Task SynchronizeCandlesticksAsync(string symbol, CandlestickInterval interval, int limit, CancellationToken token)
        {
            Logger?.LogInformation($"{GetType().Name} ({_symbol}): Synchronizing candlesticks...  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");

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
            Logger?.LogInformation($"{GetType().Name} ({_symbol}): Synchronization complete (latest open time: {candlesticks.Last().OpenTime}).  [thread: {Thread.CurrentThread.ManagedThreadId}{(token.IsCancellationRequested ? ", canceled" : string.Empty)}]");
        }

        #endregion Private Methods
    }
}
