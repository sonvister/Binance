using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

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
        public static Task SubscribeAsync(this IAccountInfoCache cache, IBinanceApiUser user, CancellationToken token)
            => cache.SubscribeAsync(user, null, token);
    }
}
