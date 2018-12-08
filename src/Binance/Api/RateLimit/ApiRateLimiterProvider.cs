using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    public sealed class ApiRateLimiterProvider : IApiRateLimiterProvider
    {
        private IServiceProvider _services;

        public ApiRateLimiterProvider(IServiceProvider services = null)
        {
            _services = services;
        }

        public IApiRateLimiter CreateApiRateLimiter()
            => _services?.GetService<IApiRateLimiter>() ?? new ApiRateLimiter();
    }
}
