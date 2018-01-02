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

        public IDictionary<string, SymbolStatistics> Statistics { get; private set; }

        #endregion Public Properties

        #region Constructors

        public SymbolStatisticsCache(IBinanceApi api, ISymbolStatisticsWebSocketClient client, ILogger<SymbolStatisticsCache> logger = null)
            : base(api, client, logger)
        {
            Statistics = new Dictionary<string, SymbolStatistics>();
        }

        #endregion Constructors

        #region Public Methods

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

            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(symbol, token)
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
            // Initialize all symbol statistics.
            if (Statistics.Count == 0)
            {
                Logger?.LogInformation($"{nameof(SymbolStatisticsCache)}: Getting all symbol statistics...");

                var statistics = await Api.Get24HourStatisticsAsync(Token)
                    .ConfigureAwait(false);

                foreach (var stats in statistics)
                {
                    Statistics[stats.Symbol] = stats;
                }
            }

            foreach (var stats in @event.Statistics)
            {
                Statistics[stats.Symbol] = stats;
            }

            return new SymbolStatisticsCacheEventArgs(Statistics.Values.ToArray());
        }

        #endregion Protected Methods
    }
}
