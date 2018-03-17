// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface IRateLimiterProvider
    {
        /// <summary>
        /// Create a new <see cref="IRateLimiter"/>.
        /// </summary>
        /// <returns></returns>
        IRateLimiter CreateRateLimiter();
    }
}
