using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.Api.WebSocket
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
        public static Task StreamAsync(this IUserDataWebSocketClient client, IBinanceApiUser user, CancellationToken token)
            => client.StreamAsync(user, null, token);
    }
}
