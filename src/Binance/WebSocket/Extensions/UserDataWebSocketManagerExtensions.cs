using System.Threading;
using System.Threading.Tasks;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
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
        public static Task SubscribeAsync(this IUserDataWebSocketManager manager, IBinanceApiUser user, CancellationToken token = default)
            => manager.SubscribeAsync<UserDataEventArgs>(user, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task UnsubscribeAsync(this IUserDataWebSocketManager manager, IBinanceApiUser user, CancellationToken token = default)
            => manager.UnsubscribeAsync<UserDataEventArgs>(user, null, token);

        /// <summary>
        /// Wait until web socket is open.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WaitUntilWebSocketOpenAsync(this IUserDataWebSocketManager manager, CancellationToken token = default)
            => manager.Client.Publisher.Stream.WaitUntilWebSocketOpenAsync(token);
    }
}
