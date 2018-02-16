using System;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Api;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public sealed class AccountInfoCache : JsonClientCache<IUserDataClient, UserDataEventArgs, AccountInfoCacheEventArgs>, IAccountInfoCache
    {
        #region Public Properties

        public AccountInfo AccountInfo { get; private set; }

        #endregion Public Properties

        #region Private Properties

        private string _listenKey;

        private IBinanceApiUser _user;

        #endregion Private Properties

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IBinanceApi"/>
        /// and default <see cref="IUserDataClient"/>, but no logger.
        /// </summary>
        public AccountInfoCache()
            : this(new BinanceApi(), new UserDataClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AccountInfoCache(IBinanceApi api, IUserDataClient client, ILogger<AccountInfoCache> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string listenKey, IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));
            Throw.IfNull(user, nameof(user));

            _listenKey = listenKey;
            _user = user;

            OnSubscribe(callback);
            SubscribeToClient();
        }

        public override void Unsubscribe()
        {
            if (_listenKey == null)
                return;

            UnsubscribeFromClient();
            OnUnsubscribe();

            AccountInfo = null;

            _listenKey = default;
            _user = default;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (_listenKey == null)
                return;

            Client.Subscribe(_listenKey, _user, ClientCallback);
        }

        protected override void UnsubscribeFromClient()
        {
            if (_listenKey == null)
                return;

            Client.Unsubscribe(_listenKey, ClientCallback);
        }

        protected override ValueTask<AccountInfoCacheEventArgs> OnAction(UserDataEventArgs @event)
        {
            if (!(@event is AccountUpdateEventArgs accountInfoEvent))
                return new ValueTask<AccountInfoCacheEventArgs>();

            AccountInfo = accountInfoEvent.AccountInfo;

            return new ValueTask<AccountInfoCacheEventArgs>(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods
    }
}
