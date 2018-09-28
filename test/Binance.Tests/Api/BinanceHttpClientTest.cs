using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Binance.Tests.Api
{
    public class BinanceHttpClientTest
    {
        [Fact]
        public void ServiceProviderDisposeNoThrows()
        {
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            var httpClient = serviceProvider.GetService<IBinanceHttpClient>();

            //Restart simulation
            serviceProvider.Dispose();

            var objectDisposedException = false;

            serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            var newHttpClient = serviceProvider.GetService<IBinanceHttpClient>();

            try
            {
                newHttpClient.RateLimiter.Configure(TimeSpan.FromSeconds(5), 5);
            }
            catch (ObjectDisposedException)
            {
                objectDisposedException = true;
            }

            Assert.False(objectDisposedException);
            Assert.False(newHttpClient.Equals(httpClient));
        }
    }
}
