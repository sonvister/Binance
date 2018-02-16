using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket.UserData
{
    public static class UserDataWebSocketClientExtensions
    {
        /// <summary>
        /// Subscribe listen key and user (w/o callback).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="listenKey"></param>
        /// <param name="user"></param>
        public static void Subscribe(this IUserDataWebSocketClient client, string listenKey, IBinanceApiUser user)
            => client.Subscribe(listenKey, user, null);

        /// <summary>
        /// Unsubscribe listen key (and user along w/ all callbacks).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="listenKey"></param>
        public static void Unsubscribe(this IUserDataWebSocketClient client, string listenKey)
            => client.Unsubscribe(listenKey, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IUserDataWebSocketClient client, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            return client.Stream.StreamAsync(token);
        }
    }
}
