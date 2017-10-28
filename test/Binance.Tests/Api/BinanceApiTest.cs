//#define LIVE

using System;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Api.Tests
{
    public class BinanceApiTest
    {
#if LIVE

        #region Connectivity

        [Fact]
        public async Task Ping()
        {
            using (var api = new BinanceApi())
            {
                Assert.True(await api.PingAsync());
            }
        }

        [Fact]
        public async Task GetTimestamp()
        {
            using (var api = new BinanceApi())
            {
                var timestamp = await api.GetTimestampAsync();

                Assert.True(timestamp > DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds());
            }
        }

        [Fact]
        public async Task GetTime()
        {
            using (var api = new BinanceApi())
            {
                var time = await api.GetTimeAsync();

                Assert.True(time > DateTime.UtcNow.AddSeconds(-30));
                Assert.Equal(DateTimeKind.Utc, time.Kind);
            }
        }

        #endregion Connectivity

        #region Market Data



        #endregion Market Data

        #region Accounts



        #endregion Accounts

#else

#endif

        #region Market Data



        #endregion Market Data
    }
}
