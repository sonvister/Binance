// ReSharper disable once CheckNamespace
namespace Binance.Api
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
