using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    /// <summary>
    /// The default <see cref="IAccountInfoCache"/> implementation.
    /// </summary>
    public class AccountInfoCache : AccountInfoCache<IUserDataClient>, IAccountInfoCache
    {
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
    }

    /// <summary>
    /// The default <see cref="IAccountInfoCache{TClient}"/> implemenation.
    /// </summary>
    public abstract class AccountInfoCache<TClient> : JsonClientCache<TClient, AccountUpdateEventArgs, AccountInfoCacheEventArgs>, IAccountInfoCache<TClient>
        where TClient : class, IUserDataClient
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
        /// The DI constructor.
        /// </summary>
        /// <param name="api">The Binance api (required).</param>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected AccountInfoCache(IBinanceApi api, TClient client, ILogger<AccountInfoCache<TClient>> logger = null)
            : base(api, client, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string listenKey, IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));
            Throw.IfNull(user, nameof(user));

            if (_listenKey != null)
                throw new InvalidOperationException($"{GetType().Name}.{nameof(Subscribe)}: Already subscribed to a (user) listen key: \"{_listenKey}\"");

            _listenKey = listenKey;
            _user = user;

            OnSubscribe(callback);
            SubscribeToClient();
        }

        public override IJsonSubscriber Unsubscribe()
        {
            if (_listenKey == null)
                return this;

            UnsubscribeFromClient();
            OnUnsubscribe();

            AccountInfo = null;

            _listenKey = default;
            _user = default;

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void SubscribeToClient()
        {
            if (_listenKey == null)
                return;

            Client.Subscribe<AccountUpdateEventArgs>(_listenKey, _user, ClientCallback);
        }

        protected override void UnsubscribeFromClient()
        {
            if (_listenKey == null)
                return;

            Client.Unsubscribe<AccountUpdateEventArgs>(_listenKey, ClientCallback);
        }

        protected override ValueTask<AccountInfoCacheEventArgs> OnActionAsync(AccountUpdateEventArgs @event, CancellationToken token = default)
        {
            AccountInfo = @event.AccountInfo;

            return new ValueTask<AccountInfoCacheEventArgs>(new AccountInfoCacheEventArgs(AccountInfo));
        }

        #endregion Protected Methods
    }
}
