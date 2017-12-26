using System;
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

        public SymbolStatistics Statistics { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private string _symbol;

        #endregion Private Fields

        #region Constructors

        public SymbolStatisticsCache(IBinanceApi api, ISymbolStatisticsWebSocketClient client, ILogger<SymbolStatisticsCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public async Task SubscribeAsync(string symbol, Action<SymbolStatisticsCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            _symbol = symbol;
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

        protected override Task<SymbolStatisticsCacheEventArgs> OnAction(SymbolStatisticsEventArgs @event)
        {
            Statistics = @event.Statistics;

            return Task.FromResult(new SymbolStatisticsCacheEventArgs(Statistics));
        }

        #endregion Protected Methods
    }
}
