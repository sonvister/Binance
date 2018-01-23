//#define INTEGRATION

#if INTEGRATION

using System;
using System.Linq;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable PossibleMultipleEnumeration

namespace Binance.Tests.Integration
{
    public class BinanceApiTest
    {
        private readonly IBinanceApi _api;

        public BinanceApiTest()
        {
            // Configure services.
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            // Get IBinanceApi service.
            _api = serviceProvider.GetService<IBinanceApi>();
        }

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

        [Fact]
        public async Task GetRateLimitInfo()
        {
            var rateLimits = await _api.GetRateLimitInfoAsync();

            Assert.NotNull(rateLimits);
            Assert.NotEmpty(rateLimits);
        }

        #endregion Connectivity

        #region Market Data

        [Fact]
        public async Task GetOrderBook()
        {
            const int limit = 5;

            var orderBook = await _api.GetOrderBookAsync(Symbol.BTC_USDT, limit);

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

            var trades = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit);

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

            var limitTrades = await _api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit);

            var startTime = limitTrades.First().Timestamp;
            var endTime = limitTrades.Last().Timestamp;

            var trades = await _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, startTime, endTime);

            Assert.NotNull(trades);
            Assert.NotEmpty(trades);
            Assert.True(trades.Count() >= limit);
            Assert.All(limitTrades, t1 => trades.Single(t2 => t2.Id == t1.Id));
        }

        [Fact]
        public async Task GetCandlesticks()
        {
            const int limit = 24;

            var candlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, limit);

            Assert.NotNull(candlesticks);
            Assert.NotEmpty(candlesticks);
            Assert.True(candlesticks.Count() == limit);
        }

        [Fact]
        public async Task GetCandlesticksIn()
        {
            const int limit = 24;

            var limitCandlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, limit);

            var startTime = limitCandlesticks.First().OpenTime;
            var endTime = limitCandlesticks.Last().OpenTime;
            const int newLimit = 12;

            var candlesticks = await _api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, newLimit, startTime, endTime);

            Assert.NotNull(candlesticks);
            Assert.NotEmpty(candlesticks);
            Assert.True(candlesticks.Count() == newLimit);
            Assert.All(candlesticks, c1 => limitCandlesticks.Single(c2 => c2.OpenTime == c1.OpenTime));
        }

        [Fact]
        public async Task Get24HourStatistics()
        {
            var stats = await _api.Get24HourStatisticsAsync(Symbol.BTC_USDT);

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
    }
}

#endif
