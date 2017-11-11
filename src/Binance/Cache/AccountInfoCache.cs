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
    public sealed class AccountInfoCache : WebSocketClientCache<IUserDataWebSocketClient, AccountUpdateEventArgs, AccountInfoCacheEventArgs>, IAccountInfoCache
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

        public async Task SubscribeAsync(IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            Token = token;

            LinkTo(Client, callback);

            try
            {
                await Client.SubscribeAsync(user, token)
                    .ConfigureAwait(false);
            }
            finally { UnLink(); }
        }

        public override void LinkTo(IUserDataWebSocketClient client, Action<AccountInfoCacheEventArgs> callback = null)
        {
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

        protected override Task<AccountInfoCacheEventArgs> OnAction(AccountUpdateEventArgs @event)
        {
            AccountInfo = @event.AccountInfo;

            return Task.FromResult(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods
    }
}
