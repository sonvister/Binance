#define LIVE

using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Api.Tests
{
    public class BinanceApiTest
    {
        private ServiceProvider _serviceProvider;

        private IBinanceApi _api;

        public BinanceApiTest()
        {
            // Configure services.
            _serviceProvider = new ServiceCollection()
                 .AddBinance().BuildServiceProvider();

            // Get IBinanceApi service.
            _api = _serviceProvider.GetService<IBinanceApi>();
        }

#if LIVE

        #region Connectivity

        [Fact]
        public async Task Ping()
        {
            Assert.True(await _api.PingAsync());
        }

        [Fact]
        public async Task GetTimestamp()
        {
            var timestamp = await _api.GetTimestampAsync();

            Assert.True(timestamp > DateTimeOffset.UtcNow.AddSeconds(-30).ToUnixTimeMilliseconds());
        }

        [Fact]
        public async Task GetTime()
        {
            var time = await _api.GetTimeAsync();

            Assert.True(time > DateTime.UtcNow.AddSeconds(-30));
            Assert.Equal(DateTimeKind.Utc, time.Kind);
        }

        #endregion Connectivity

        #region Market Data

        [Fact]
        public async Task GetOrderBook()
        {
            const int limit = 5;

            var orderBook = await _api.GetOrderBookAsync(Symbol.BTC_USDT, limit: limit);

            Assert.NotNull(orderBook);
            Assert.NotEmpty(orderBook.Bids);
            Assert.NotEmpty(orderBook.Asks);
            Assert.True(orderBook.Bids.Count() == limit);
            Assert.True(orderBook.Asks.Count() == limit);
        }

        [Fact]
        public async Task GetAggregateTrades()
        {
            const int limit = 5;

            var trades = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit: limit);

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.Count() == limit);
        }

        [Fact]
        public async Task GetAggregateTradesFrom()
        {
            const int fromId = 0;
            const int limit = 5;

            var trades = await _api.GetAggregateTradesFromAsync(Symbol.BTC_USDT, fromId, limit);

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.Count() == limit);
            Assert.True(trades.First().Id == fromId);
        }

        [Fact]
        public async Task GetAggregateTradesIn()
        {
            const int limit = 5;

            var _trades = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit: limit);

            var startTime = _trades.First().Timestamp;
            var endTime = _trades.Last().Timestamp;

            var trades = await _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, startTime, endTime);

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.Count() >= limit);
            Assert.All(_trades, (t1) => trades.Single(t2 => t2.Id == t1.Id));
        }

        [Fact]
        public async Task GetCandlesticks()
        {
            const int limit = 24;

            var candlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour, limit: limit);

            Assert.NotNull(candlesticks);
            Assert.NotEmpty(candlesticks);
            Assert.True(candlesticks.Count() == limit);
        }

        [Fact]
        public async Task GetCandlesticksIn()
        {
            const int limit = 24;

            var _candlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour, limit: limit);

            var startTime = _candlesticks.First().OpenTime;
            var endTime = _candlesticks.Last().OpenTime;
            const int newLimit = 12;

            var candlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour, newLimit, startTime, endTime);

            Assert.NotNull(candlesticks);
            Assert.NotEmpty(candlesticks);
            Assert.True(candlesticks.Count() == newLimit);
            Assert.All(candlesticks, (c1) => _candlesticks.Single(c2 => c2.OpenTime == c1.OpenTime));
        }

        [Fact]
        public async Task Get24hrStats()
        {
            var stats = await _api.Get24hrStatsAsync(Symbol.BTC_USDT);

            Assert.NotNull(stats);
        }

        [Fact]
        public async Task GetPrices()
        {
            var prices = await _api.GetPricesAsync();

            Assert.NotNull(prices);
            Assert.NotEmpty(prices);
        }

        [Fact]
        public async Task GetOrderBookTops()
        {
            var tops = await _api.GetOrderBookTopsAsync();

            Assert.NotNull(tops);
            Assert.NotEmpty(tops);
        }

        #endregion Market Data

        #region Accounts



        #endregion Accounts

#else

#endif

        #region Market Data



        #endregion Market Data
    }
}
