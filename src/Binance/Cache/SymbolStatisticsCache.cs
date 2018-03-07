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
    /// The default <see cref="ISymbolStatisticsCache"/> implementation.
    /// </summary>
    public class SymbolStatisticsCache : SymbolStatisticsCache<ISymbolStatisticsClient>, ISymbolStatisticsCache
    {
        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="ISymbolStatisticsClient"/>, but no logger.
        /// </summary>
        public SymbolStatisticsCache()
            : this(new BinanceApi(), new SymbolStatisticsClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public SymbolStatisticsCache(IBinanceApi api, ISymbolStatisticsClient client, ILogger<SymbolStatisticsCache> logger = null)
            : base(api, client, logger)
        { }
    }

    /// <summary>
    /// The default <see cref="ISymbolStatisticsCache{TClient}"/> implemenation.
    /// </summary>
    public abstract class SymbolStatisticsCache<TClient> : JsonClientCache<TClient, SymbolStatisticsEventArgs, SymbolStatisticsCacheEventArgs>, ISymbolStatisticsCache<TClient>
        where TClient : class, ISymbolStatisticsClient
    {
        #region Public Properties

        public IEnumerable<SymbolStatistics> Statistics
        {
            get
            {
                lock (_sync)
                {
                    return _symbols
                        .Where(s => _statistics.ContainsKey(s))
                        .Select(s => _statistics[s])
                        .ToArray();
                }
            }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly IList<string> _symbols = new List<string>();

        private readonly IDictionary<string, SymbolStatistics> _statistics
            = new Dictionary<string, SymbolStatistics>();

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected SymbolStatisticsCache(IBinanceApi api, TClient client, ILogger<SymbolStatisticsCache<TClient>> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public SymbolStatistics GetStatistics(string symbol)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            lock (_sync)
            {
                return _statistics.ContainsKey(symbol) ? _statistics[symbol] : null;
            }
        }

        public IEnumerable<SymbolStatistics> GetStatistics(params string[] symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            lock (_sync)
            {
                foreach (var symbol in symbols)
                    yield return GetStatistics(symbol);
            }
        }

        public void Subscribe(Action<SymbolStatisticsCacheEventArgs> callback, params string[] symbols)
        {
            OnSubscribe(callback);

            if (symbols != null)
            {
                foreach (var s in symbols)
                {
                    Throw.IfNullOrWhiteSpace(s, nameof(s));

                    var symbol = s.FormatSymbol();

                    if (_symbols.Contains(symbol))
                        continue;

                    _symbols.Add(symbol);
                }
            }

            SubscribeToClient();
        }

        public override IJsonSubscriber Unsubscribe()
        {
            UnsubscribeFromClient();
            OnUnsubscribe();

            lock (_sync)
            {
                _statistics.Clear();
            }

            _symbols.Clear();

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (!_symbols.Any())
            {
                Client.Subscribe(ClientCallback);
            }
            else
            {
                Client.Subscribe(ClientCallback, _symbols.ToArray());
            }
        }

        protected override void UnsubscribeFromClient()
        {
            if (!_symbols.Any())
            {
                Client.Unsubscribe(ClientCallback);
            }
            else
            {
                Client.Unsubscribe(ClientCallback, _symbols.ToArray());
            }
        }

        protected override async ValueTask<SymbolStatisticsCacheEventArgs> OnActionAsync(SymbolStatisticsEventArgs @event, CancellationToken token = default)
        {
            try
            {
                // ReSharper disable once InconsistentlySynchronizedField
                if (_statistics.Count == 0 && !_symbols.Any())
                {
                    Logger?.LogInformation($"{nameof(SymbolStatisticsCache)}.{nameof(OnActionAsync)}: Initializing all symbol statistics...");

                    var statistics = await Api.Get24HourStatisticsAsync(token)
                        .ConfigureAwait(false);

                    lock (_sync)
                    {
                        foreach (var stats in statistics)
                        {
                            _statistics[stats.Symbol] = stats;
                        }
                    }
                }

                lock (_sync)
                {
                    foreach (var stats in @event.Statistics)
                    {
                        _statistics[stats.Symbol] = stats;
                    }

                    if (!_symbols.Any())
                    {
                        return new SymbolStatisticsCacheEventArgs(_statistics.Values.ToArray());
                    }

                    var statistics = _symbols
                        .Where(s => _statistics.ContainsKey(s))
                        .Select(s => _statistics[s]);

                    return new SymbolStatisticsCacheEventArgs(statistics.ToArray());
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(SymbolStatisticsCache)}.{nameof(OnActionAsync)}: Failed.");
                return null;
            }
        }

        #endregion Protected Methods
    }
}
