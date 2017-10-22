//#define LIVE

using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Api.Json
{
    public class BinanceJsonApiTest
    {
#if LIVE

        [Fact]
        public async Task PingAsync()
        {
            var api = new BinanceJsonApi();

            Assert.Equal(BinanceJsonApi.SuccessfulTestResponse, await api.PingAsync());
        }

        [Fact]
        public async Task RateLimitAsync()
        {
            var api = new BinanceJsonApi();

            api.RateLimiter.IsEnabled = true;

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < api.RateLimiter.Count * 2; i++)
                await api.PingAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > api.RateLimiter.Duration.TotalMilliseconds * 2);
        }

#else
        /*
        [Fact]
        public async Task PingAsync()
        {
            await Task.CompletedTask; // TODO
        }

        [Fact]
        public async Task TimeAsync()
        {
            await Task.CompletedTask; // TODO
        }
        */
#endif
    }
}
