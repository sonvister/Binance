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
    public class AccountInfoCache : WebSocketClientCache<IUserDataWebSocketClient, AccountUpdateEventArgs, AccountInfoCacheEventArgs>, IAccountInfoCache
    {
        #region Public Properties

        public AccountInfo AccountInfo { get; private set; }

        #endregion Public Properties

        #region Constructors

        public AccountInfoCache(IBinanceApi api, IUserDataWebSocketClient client, bool leaveClientOpen = false, ILogger<AccountInfoCache> logger = null)
            : base(api, client, leaveClientOpen, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(IBinanceApiUser user, CancellationToken token = default)
            => SubscribeAsync(user, null, token);

        public Task SubscribeAsync(IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            token.ThrowIfCancellationRequested();

            Token = token;

            LinkTo(Client, callback, LeaveClientOpen);

            return Client.SubscribeAsync(user, token);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnLinkTo()
        {
            Client.AccountUpdate += OnClientEvent;
        }

        protected override Task<AccountInfoCacheEventArgs> OnAction(AccountUpdateEventArgs @event)
        {
            AccountInfo = @event.AccountInfo;

            return Task.FromResult(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.AccountUpdate -= OnClientEvent;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
