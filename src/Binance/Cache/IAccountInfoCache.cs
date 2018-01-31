using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Api;
using Binance.Cache.Events;
using Binance.WebSocket;

namespace Binance.Cache
{
    public interface IAccountInfoCache
    {
        #region Events

        /// <summary>
        /// AccountInfo cache update event.
        /// </summary>
        event EventHandler<AccountInfoCacheEventArgs> Update;

        #endregion Events

        #region Properties

        /// <summary>
        /// The account information. Can be null if not yet synchronized or out-of-sync.
        /// </summary>
        AccountInfo AccountInfo { get; }

        /// <summary>
        /// The client that provides account synchronization.
        /// </summary>
        IUserDataWebSocketClient Client { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token = default);

        /// <summary>
        /// Link to a subscribed <see cref="IUserDataWebSocketClient"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void LinkTo(IUserDataWebSocketClient client, Action<AccountInfoCacheEventArgs> callback = null);

        /// <summary>
        /// Unlink from client.
        /// </summary>
        void UnLink();

        #endregion Methods
    }
}
