using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class AccountInfoCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="listenKey"></param>
        /// <param name="user"></param>
        public static void Subscribe<TClient>(this IAccountInfoCache<TClient> cache, string listenKey, IBinanceApiUser user)
            where TClient : IUserDataClient
            => cache.Subscribe(listenKey, user, null);
    }
}
