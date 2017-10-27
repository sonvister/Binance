using Binance.Accounts;
using Binance.Accounts.Cache;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
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
        /// The account.
        /// </summary>
        Account Account { get; }

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
        Task SubscribeAsync(IBinanceUser user, CancellationToken token = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(IBinanceUser user, Action<AccountCacheEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
