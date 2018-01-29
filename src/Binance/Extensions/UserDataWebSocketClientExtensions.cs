using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;

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
            => SubscribeAndStreamAsync(client, user, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SubscribeAndStreamAsync(this IUserDataWebSocketClient client, IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            await client.SubscribeAsync(user, callback, token)
                .ConfigureAwait(false);

            await client.WebSocket.StreamAsync(token)
                .ConfigureAwait(false);
        }
    }
}
