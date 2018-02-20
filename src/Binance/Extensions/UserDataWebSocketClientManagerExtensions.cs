using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Client.Events;

namespace Binance.WebSocket.Manager
{
    public static class UserDataWebSocketClientManagerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task UnsubscribeAsync(this IUserDataWebSocketClientManager manager, IBinanceApiUser user, CancellationToken token = default)
            => manager.UnsubscribeAsync<UserDataEventArgs>(user, null, token);
    }
}
