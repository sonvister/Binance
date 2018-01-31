using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class UserDataWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this IUserDataWebSocketClient client, IBinanceApiUser user, CancellationToken token)
            => client.SubscribeAndStreamAsync(user, null, token);
    }
}
