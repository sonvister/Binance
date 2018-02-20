using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket.UserData
{
    public static class UserDataWebSocketManagerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this _IUserDataWebSocketManager manager, IBinanceApiUser user, CancellationToken token)
        {
            Throw.IfNull(manager, nameof(manager));

            return manager.SubscribeAndStreamAsync(user, null, token);
        }
    }
}
