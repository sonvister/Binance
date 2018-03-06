using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// A <see cref="IBinanceApiUser"/> provider allowing for the application of <see cref="BinanceApiOptions"/>.
    /// </summary>
    public sealed class BinanceApiUserProvider : IBinanceApiUserProvider
    {
        #region Private Fields

        private readonly IServiceProvider _services;

        private readonly IOptions<BinanceApiOptions> _options;

        #endregion Private Fields

        #region Constructors

        public BinanceApiUserProvider(IServiceProvider services, IOptions<BinanceApiOptions> options = null)
        {
            Throw.IfNull(services, nameof(services));

            _services = services;
            _options = options;
        }

        #endregion Constructors

        #region Public Methods

        public IBinanceApiUser CreateUser(string apiKey, string apiSecret = null)
        {
            return new BinanceApiUser(apiKey, apiSecret, _services.GetService<IApiRateLimiter>(), _options);
        }

        #endregion Public Methods
    }
}
