using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A user data web socket stream (listen key) manager.
    /// </summary>
    public interface IUserDataWebSocketStreamControl : IDisposable
    {
        /// <summary>
        /// Listen key update event.
        /// </summary>
        event EventHandler<UserDataListenKeyUpdateEventArgs> ListenKeyUpdate;

        /// <summary>
        /// Get users.
        /// </summary>
        IEnumerable<IBinanceApiUser> Users { get; }

        /// <summary>
        /// Stream keep-alive timer period.
        /// </summary>
        TimeSpan KeepAliveTimerPeriod { get; set; }

        /// <summary>
        /// Get the stream name associated with the user or null.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetStreamNameAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// Open user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> OpenStreamAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// Close user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task CloseStreamAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// Close all managed user data streams.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task CloseAllStreamsAsync(CancellationToken token = default);
    }
}
