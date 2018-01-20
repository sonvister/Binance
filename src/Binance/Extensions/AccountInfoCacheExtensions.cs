using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Cache.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class AccountInfoCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IAccountInfoCache cache, IBinanceApiUser user, CancellationToken token)
            => StreamAsync(cache, user, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task StreamAsync(this IAccountInfoCache cache, IBinanceApiUser user, Action<AccountInfoCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(cache, nameof(cache));

            await cache.SubscribeAsync(user, callback, token)
                .ConfigureAwait(false);

            await cache.Client.WebSocket.StreamAsync(token)
                .ConfigureAwait(false);
        }
    }
}
