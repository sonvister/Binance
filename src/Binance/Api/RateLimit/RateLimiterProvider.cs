using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class RateLimiterProvider : IRateLimiterProvider
    {
        private IServiceProvider _services;

        public RateLimiterProvider(IServiceProvider services = null)
        {
            _services = services;
        }

        public IRateLimiter CreateRateLimiter()
            => _services?.GetService<IRateLimiter>() ?? new RateLimiter();
    }
}
