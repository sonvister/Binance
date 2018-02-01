using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;

namespace Binance.WebSocket.UserData
{
    public interface IMultiUserDataWebSocketClient : IUserDataWebSocketClient
    {
        /// <summary>
        /// Subscribe to the specified user (for use with combined streams). 
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default);
    }
}
