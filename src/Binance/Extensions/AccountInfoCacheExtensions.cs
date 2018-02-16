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
        /// <param name="listenKey"></param>
        /// <param name="user"></param>
        public static void Subscribe(this IAccountInfoCache cache, string listenKey, IBinanceApiUser user)
            => cache.Subscribe(listenKey, user, null);
    }
}
