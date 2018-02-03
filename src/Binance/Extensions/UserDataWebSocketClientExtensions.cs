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
    }
}
