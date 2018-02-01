// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    public interface IBinanceApiUserProvider
    {
        /// <summary>
        /// Create an API user.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="apiSecret">The API secret (optional)</param>
        /// <returns></returns>
        IBinanceApiUser CreateUser(string apiKey, string apiSecret = null);
    }
}
