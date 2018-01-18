using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Binance.Cache.Events;
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

        public async Task StreamAsync(IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            Token = token;

            base.LinkTo(Client, callback);

            await Client.StreamAsync(user, ClientCallback, token)
                .ConfigureAwait(false);
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
            var accountInfoEvent = @event as AccountUpdateEventArgs;

            if (accountInfoEvent == null)
                return null;

            AccountInfo = accountInfoEvent.AccountInfo;

            return Task.FromResult(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods
    }
}
