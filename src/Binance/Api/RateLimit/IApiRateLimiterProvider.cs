// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface IApiRateLimiterProvider
    {
        /// <summary>
        /// Create a new <see cref="IApiRateLimiter"/>.
        /// </summary>
        /// <returns></returns>
        IApiRateLimiter CreateApiRateLimiter();
    }
}
