#define LIVE

using System;
using System.Threading.Tasks;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Api.Json;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Binance.Tests.Api.Json
{
    public class BinanceJsonApiTest
    {
        private readonly IBinanceJsonApi _api;

        public BinanceJsonApiTest()
        {
            // Configure services.
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            // Get IBinanceApi service.
            _api = serviceProvider.GetService<IBinanceJsonApi>();
        }

#if LIVE

        #region Connectivity

        [Fact]
        public async Task Ping()
        {
            Assert.Equal(BinanceJsonApi.SuccessfulTestResponse, await _api.PingAsync());
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
        public async Task GetTrades()
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

        /*
        [Fact]
        public async Task RateLimit()
        {
            const int count = 3;
            const int intervals = 2;

            _api.RateLimiter.IsEnabled = true;
            _api.RateLimiter.Configure(count, TimeSpan.FromSeconds(1));

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count * intervals; i++)
                await _api.PingAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > _api.RateLimiter.Duration.TotalMilliseconds * intervals);
            Assert.False(count <= 3 && stopwatch.ElapsedMilliseconds > _api.RateLimiter.Duration.TotalMilliseconds * (intervals + 1));
        }
        //*/

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

#else

#endif

        #region Market Data

        [Fact]
        public async Task GetOrderBookThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetOrderBookAsync(null));
        }

        [Fact]
        public async Task GetAggregateTradesThrows()
        {
            var now = DateTimeOffset.UtcNow;

            var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
            var endTime = now.ToUnixTimeMilliseconds();

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetAggregateTradesAsync(null));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime: startTime, endTime: endTime));
        }

        [Fact]
        public async Task GetCandlesticksThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetCandlesticksAsync(null, CandlestickInterval.Day));
        }

        [Fact]
        public async Task Get24HourStatisticsThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.Get24HourStatisticsAsync(null));
        }

        #endregion Market Data

        #region Account

        [Fact]
        public async Task PlaceOrderThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            var orderSide = OrderSide.Sell;
            var orderType = OrderType.Market;
            decimal quantity = 1;
            decimal price = 0;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.PlaceOrderAsync(null, symbol, orderSide, orderType, quantity, price));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.PlaceOrderAsync(user, null, orderSide, orderType, quantity, price));
            await Assert.ThrowsAsync<ArgumentException>("quantity", () => _api.PlaceOrderAsync(user, symbol, orderSide, orderType, -1, price));
        }

        [Fact]
        public async Task GetOrderThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetOrderAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetOrderAsync(user, null));
            await Assert.ThrowsAsync<ArgumentException>(() => _api.GetOrderAsync(user, symbol));
        }

        [Fact]
        public async Task CancelOrderThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.CancelOrderAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.CancelOrderAsync(user, null));
            await Assert.ThrowsAsync<ArgumentException>(() => _api.GetOrderAsync(user, symbol));
        }

        [Fact]
        public async Task GetOpenOrdersThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetOpenOrdersAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetOpenOrdersAsync(user, null));
        }

        [Fact]
        public async Task GetOrdersThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetOrdersAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _api.GetOrdersAsync(user, null));
        }

        [Fact]
        public async Task GetAccountThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetAccountInfoAsync(null));
        }

        [Fact]
        public async Task GetTradesThrows()
        {
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetTradesAsync(null, symbol));
        }

        [Fact]
        public async Task WithdrawThrows()
        {
            var user = new BinanceApiUser("api-key");
            var asset = Asset.BTC;
            const string address = "12345678901234567890";
            const decimal amount = 1;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.WithdrawAsync(null, asset, address, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("asset", () => _api.WithdrawAsync(user, null, address, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("address", () => _api.WithdrawAsync(user, asset, null, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("address", () => _api.WithdrawAsync(user, asset, string.Empty, amount));
            await Assert.ThrowsAsync<ArgumentException>("amount", () => _api.WithdrawAsync(user, asset, address, -1));
            await Assert.ThrowsAsync<ArgumentException>("amount", () => _api.WithdrawAsync(user, asset, address, 0));
        }

        [Fact]
        public async Task GetDepositsThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetDepositsAsync(null));
        }

        [Fact]
        public async Task GetWithdrawalsThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.GetWithdrawalsAsync(null));
        }

        #endregion Account

        #region User Stream

        [Fact]
        public async Task UserStreamStartThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.UserStreamStartAsync(null));
        }

        [Fact]
        public async Task UserStreamKeepAliveThrows()
        {
            var user = new BinanceApiUser("api-key");
            const string listenKey = "listen-key";

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.UserStreamKeepAliveAsync(null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _api.UserStreamKeepAliveAsync(user, null));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _api.UserStreamKeepAliveAsync(user, string.Empty));
        }

        [Fact]
        public async Task UserStreamCloseThrows()
        {
            var user = new BinanceApiUser("api-key");
            const string listenKey = "listen-key";

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _api.UserStreamCloseAsync(null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _api.UserStreamCloseAsync(user, null));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _api.UserStreamCloseAsync(user, string.Empty));
        }

        #endregion User Stream
    }
}
