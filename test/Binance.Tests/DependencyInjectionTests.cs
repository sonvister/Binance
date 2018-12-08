using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Binance.Api;

namespace Binance.Tests
{
    [Collection("Binance HTTP Client Tests")]
    public class DependencyInjectionTests
    {
        [Fact]
        public Task ServiceProviderDispose()
        {
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            var httpClient = serviceProvider.GetService<IBinanceHttpClient>();

            serviceProvider.Dispose();

            // Verify static BinanceHttpClient singleton is disposed.
            return Assert.ThrowsAsync<ObjectDisposedException>(() => httpClient.GetAsync("test"));
        }

        [Fact]
        public void NonStaticServiceProviderUse()
        {
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            var httpClient = serviceProvider.GetService<IBinanceHttpClient>();

            serviceProvider.Dispose();


            serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            httpClient = serviceProvider.GetService<IBinanceHttpClient>();

            // Verify the RateLimiter is not disposed (no exception is thrown).
            httpClient.RateLimiter.Configure(TimeSpan.FromSeconds(1), 1);
        }
    }
}
