//#define INTEGRATION

#if INTEGRATION

using System;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Binance.Tests.Integration
{
    public class BinanceHttpClientTest
    {
        private readonly IBinanceHttpClient _api;

        public BinanceHttpClientTest()
        {
            // Configure services.
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            // Get IBinanceHttpClient service.
            _api = serviceProvider.GetService<IBinanceHttpClient>();
        }

        #region Connectivity

        [Fact]
        public async Task Ping()
        {
            Assert.Equal(BinanceApi.SuccessfulTestResponse, await _api.PingAsync());
        }

        [Fact]
        public async Task GetServerTime()
        {
            var json = await _api.GetServerTimeAsync();

            Assert.True(IsJsonObject(json));
        }

        #endregion Connectivity

        #region Market Data

        [Fact]
        public async Task GetOrderBook()
        {
            var json = await _api.GetOrderBookAsync(Symbol.BTC_USDT, 5);

            Assert.True(IsJsonObject(json));
        }

        [Fact]
        public async Task GetAggregateTrades()
        {
            var now = DateTimeOffset.UtcNow;

            var json = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, 0, 1);

            Assert.True(IsJsonArray(json));

            json = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime: now.AddMinutes(-1).ToUnixTimeMilliseconds(), endTime: now.ToUnixTimeMilliseconds());

            Assert.True(IsJsonArray(json));
        }

        [Fact]
        public async Task GetCandlesticks()
        {
            var now = DateTimeOffset.UtcNow;

            var json = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, 1);

            Assert.True(IsJsonArray(json));

            json = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Minute, startTime: now.AddMinutes(-1).ToUnixTimeMilliseconds(), endTime: now.ToUnixTimeMilliseconds());

            Assert.True(IsJsonArray(json));
        }

        [Fact]
        public async Task Get24HourStatistics()
        {
            var json = await _api.Get24HourStatisticsAsync(Symbol.BTC_USDT);

            Assert.True(IsJsonObject(json));
        }

        [Fact]
        public async Task GetPrices()
        {
            var json = await _api.GetPricesAsync();

            Assert.True(IsJsonArray(json));
        }

        [Fact]
        public async Task GetOrderBookTops()
        {
            var json = await _api.GetOrderBookTopsAsync();

            Assert.True(IsJsonArray(json));
        }

        #endregion Market Data

        private static bool IsJsonObject(string json)
        {
            return !string.IsNullOrWhiteSpace(json)
                && json.StartsWith("{") && json.EndsWith("}");
        }

        private static bool IsJsonArray(string json)
        {
            return !string.IsNullOrWhiteSpace(json)
                && json.StartsWith("[") && json.EndsWith("]");
        }
    }
}

#endif
