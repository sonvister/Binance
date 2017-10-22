//#define LIVE

using Binance.Api;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Api
{
    public class BinanceApiTest
    {
#if LIVE

        [Fact]
        public async Task PingAsync()
        {
            var api = new BinanceApi();

            Assert.True(await api.PingAsync());
        }

        [Fact]
        public async Task TimeAsync()
        {
            var api = new BinanceApi();

            var time = await api.GetTimeAsync();

            Assert.True(time > DateTime.UtcNow.AddSeconds(-30));
            Assert.Equal(DateTimeKind.Utc, time.Kind);
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
