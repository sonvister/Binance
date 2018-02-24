using Binance.Api;
using Binance.Client.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class UserDataClientExtensions
    {
        /// <summary>
        /// Subscribe listen key and user (w/o callback).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="listenKey"></param>
        /// <param name="user"></param>
        public static IUserDataClient Subscribe(this IUserDataClient client, string listenKey, IBinanceApiUser user)
            => client.Subscribe<UserDataEventArgs>(listenKey, user, null);

        /// <summary>
        /// Unsubscribe listen key (and user along w/ all callbacks).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="listenKey"></param>
        public static IUserDataClient Unsubscribe(this IUserDataClient client, string listenKey)
            => client.Unsubscribe<UserDataEventArgs>(listenKey, null);
    }
}
