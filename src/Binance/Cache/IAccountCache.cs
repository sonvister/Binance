using Binance.Account;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Cache
{
    public interface IAccountCache : IDisposable
    {
        #region Public Events

        /// <summary>
        /// Account cache update event.
        /// </summary>
        event EventHandler<AccountCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        /// <summary>
        /// The account. Can be null if not yet synchronized or out-of-sync.
        /// </summary>
        AccountInfo Account { get; }

        /// <summary>
        /// The client that provides account synchronization.
        /// </summary>
        IUserDataWebSocketClient Client { get; }
        
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(IBinanceApiUser user, Action<AccountCacheEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
