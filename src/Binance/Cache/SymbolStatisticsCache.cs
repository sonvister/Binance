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

namespace Binance.Cache
{
    public sealed class SymbolStatisticsCache : WebSocketClientCache<ISymbolStatisticsWebSocketClient, SymbolStatisticsEventArgs, SymbolStatisticsCacheEventArgs>, ISymbolStatisticsCache
    {
        #region Public Properties

        public IEnumerable<SymbolStatistics> Statistics
        {
            get { lock (_sync) { return _statistics.Values.ToArray(); } }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly IDictionary<string, SymbolStatistics> _statistics
            = new Dictionary<string, SymbolStatistics>();

        private readonly object _sync = new object();

        private string _symbol;

        #endregion Private Fields

        #region Constructors

        public SymbolStatisticsCache(IBinanceApi api, ISymbolStatisticsWebSocketClient client, ILogger<SymbolStatisticsCache> logger = null)
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

        public async Task SubscribeAsync(Action<SymbolStatisticsCacheEventArgs> callback, CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(token)
                    .ConfigureAwait(false);
            }
            finally { UnLink(); }
        }

        public async Task SubscribeAsync(string symbol, Action<SymbolStatisticsCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            _symbol = symbol.FormatSymbol();

            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(_symbol, token)
                    .ConfigureAwait(false);
            }
            finally { UnLink(); }
        }

        public override void LinkTo(ISymbolStatisticsWebSocketClient client, Action<SymbolStatisticsCacheEventArgs> callback = null)
        {
            base.LinkTo(client, callback);
            Client.StatisticsUpdate += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.StatisticsUpdate -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override async Task<SymbolStatisticsCacheEventArgs> OnAction(SymbolStatisticsEventArgs @event)
        {
            if (_statistics.Count == 0)
            {
                Logger?.LogInformation($"{nameof(SymbolStatisticsCache)}: Initializing symbol statistics...");

                if (_symbol == null)
                {
                    var statistics = await Api.Get24HourStatisticsAsync(Token)
                        .ConfigureAwait(false);

                    lock (_sync)
                    {
                        foreach (var stats in statistics)
                        {
                            _statistics[stats.Symbol] = stats;
                        }
                    }
                }
            }

            lock (_sync)
            {
                foreach (var stats in @event.Statistics)
                {
                    _statistics[stats.Symbol] = stats;
                }

                return new SymbolStatisticsCacheEventArgs(_statistics.Values.ToArray());
            }
        }

        #endregion Protected Methods
    }
}
