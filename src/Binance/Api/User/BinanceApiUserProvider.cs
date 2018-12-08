using Microsoft.Extensions.Options;
using Binance.Api;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// A <see cref="IBinanceApiUser"/> provider allowing for the application of <see cref="IOptions{BinanceApiOptions}"/>.
    /// </summary>
    public sealed class BinanceApiUserProvider : IBinanceApiUserProvider
    {
        #region Private Fields

        private readonly IApiRateLimiterProvider _apiRateLimiterProvider;

        private readonly IOptions<BinanceApiOptions> _options;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// The default constructor with default <see cref="IApiRateLimiterProvider"/>.
        /// </summary>
        public BinanceApiUserProvider()
            : this(new ApiRateLimiterProvider())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="apiRateLimiterProvider">The API rate limiter provider (required).</param>
        /// <param name="options">The API options (optional).</param>
        public BinanceApiUserProvider(IApiRateLimiterProvider apiRateLimiterProvider, IOptions<BinanceApiOptions> options = null)
        {
            Throw.IfNull(apiRateLimiterProvider, nameof(apiRateLimiterProvider));

            _apiRateLimiterProvider = apiRateLimiterProvider;
            _options = options;
        }

        #endregion Constructors

        #region Public Methods

        public IBinanceApiUser CreateUser(string apiKey, string apiSecret = null)
        {
            return new BinanceApiUser(apiKey, apiSecret, _apiRateLimiterProvider.CreateApiRateLimiter(), _options);
        }

        #endregion Public Methods
    }
}
