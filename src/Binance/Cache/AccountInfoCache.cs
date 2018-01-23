using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Api;
using Binance.Cache.Events;
using Binance.WebSocket;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public sealed class AccountInfoCache : WebSocketClientCache<IUserDataWebSocketClient, UserDataEventArgs, AccountInfoCacheEventArgs>, IAccountInfoCache
    {
        #region Public Properties

        public AccountInfo AccountInfo { get; private set; }

        #endregion Public Properties

        #region Constructors

        public AccountInfoCache(IBinanceApi api, IUserDataWebSocketClient client, ILogger<AccountInfoCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            base.LinkTo(Client, callback);

            return Client.SubscribeAsync(user, ClientCallback, token);
        }

        public override void LinkTo(IUserDataWebSocketClient client, Action<AccountInfoCacheEventArgs> callback = null)
        {
            // Confirm client is subscribed to only one stream.
            if (client.WebSocket.IsCombined)
                throw new InvalidOperationException($"{nameof(AccountInfoCache)} can only link to {nameof(IUserDataWebSocketClient)} events from a single stream (not combined streams).");

            base.LinkTo(client, callback);
            Client.AccountUpdate += OnClientEvent;
        }

        public override void UnLink()
        {
            Client.AccountUpdate -= OnClientEvent;
            base.UnLink();
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Task<AccountInfoCacheEventArgs> OnAction(UserDataEventArgs @event)
        {
            if (!(@event is AccountUpdateEventArgs accountInfoEvent))
                return null;

            AccountInfo = accountInfoEvent.AccountInfo;

            return Task.FromResult(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods
    }
}
