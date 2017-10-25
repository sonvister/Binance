//#define LIVE

using Binance.Orders;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Api.Json
{
    public class BinanceJsonApiTest
    {
#if LIVE

        [Fact]
        public async Task Ping()
        {
            using (var api = new BinanceJsonApi())
            {
                Assert.Equal(BinanceJsonApi.SuccessfulTestResponse, await api.PingAsync());
            }
        }

        [Fact]
        public async Task GetServerTime()
        {
            using (var api = new BinanceJsonApi())
            {
                var json = await api.GetServerTimeAsync();

                Assert.True(IsJsonObject(json));
            }
        }

        [Fact]
        public async Task GetOrderBook()
        {
            using (var api = new BinanceJsonApi())
            {
                var json = await api.GetOrderBookAsync(Symbol.BTC_USDT, BinanceApi.OrderBookLimitMin);

                Assert.True(IsJsonObject(json));
            }
        }

        [Fact]
        public async Task GetTrades()
        {
            using (var api = new BinanceJsonApi())
            {
                var now = DateTimeOffset.UtcNow;

                var json = await api.GetAggregateTradesAsync(Symbol.BTC_USDT, 0, limit: BinanceApi.TradesLimitMin);

                Assert.True(IsJsonArray(json));

                json = await api.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime: now.AddMinutes(-1).ToUnixTimeMilliseconds(), endTime: now.ToUnixTimeMilliseconds());

                Assert.True(IsJsonArray(json));
            }
        }

        [Fact]
        public async Task GetCandlesticks()
        {
            using (var api = new BinanceJsonApi())
            {
                var now = DateTimeOffset.UtcNow;

                var json = await api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour, limit: BinanceApi.CandlesticksLimitMin);

                Assert.True(IsJsonArray(json));

                json = await api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Minute, startTime: now.AddMinutes(-1).ToUnixTimeMilliseconds(), endTime: now.ToUnixTimeMilliseconds());

                Assert.True(IsJsonArray(json));
            }
        }

        [Fact]
        public async Task Get24hrStats()
        {
            using (var api = new BinanceJsonApi())
            {
                var json = await api.Get24hStatsAsync(Symbol.BTC_USDT);

                Assert.True(IsJsonObject(json));
            }
        }

        [Fact]
        public async Task GetPrices()
        {
            using (var api = new BinanceJsonApi())
            {
                var json = await api.GetPrices();

                Assert.True(IsJsonArray(json));
            }
        }

        [Fact]
        public async Task GetOrderBookTops()
        {
            using (var api = new BinanceJsonApi())
            {
                var json = await api.GetOrderBookTopsAsync();

                Assert.True(IsJsonArray(json));
            }
        }

        /*
        [Fact]
        public async Task RateLimit()
        {
            const int count = 3;
            const int intervals = 2;

            using (var api = new BinanceJsonApi())
            {
                api.RateLimiter.IsEnabled = true;
                api.RateLimiter.Configure(count, TimeSpan.FromSeconds(1));

                var stopwatch = Stopwatch.StartNew();

                for (var i = 0; i < count * intervals; i++)
                    await api.PingAsync();

                stopwatch.Stop();

                Assert.True(stopwatch.ElapsedMilliseconds > api.RateLimiter.Duration.TotalMilliseconds * intervals);
                Assert.False(count <= 3 && stopwatch.ElapsedMilliseconds > api.RateLimiter.Duration.TotalMilliseconds * (intervals + 1));
            }
        }
        //*/

        private bool IsJsonObject(string json)
        {
            return !string.IsNullOrWhiteSpace(json)
                && json.StartsWith("{") && json.EndsWith("}");
        }

        private bool IsJsonArray(string json)
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
            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetOrderBookAsync(null));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrderBookAsync(Symbol.BTC_USDT, -1));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrderBookAsync(Symbol.BTC_USDT, 0));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrderBookAsync(Symbol.BTC_USDT, BinanceApi.OrderBookLimitMax + 1));
            }
        }

        [Fact]
        public async Task GetAggregateTradesThrows()
        {
            using (var api = new BinanceJsonApi())
            {
                var now = DateTimeOffset.UtcNow;

                var startTime = DateTimeOffset.UtcNow.AddHours(-24).ToUnixTimeMilliseconds();
                var endTime = now.ToUnixTimeMilliseconds();

                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetAggregateTradesAsync(null));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit: -1));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit: 0));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetAggregateTradesAsync(Symbol.BTC_USDT, limit: BinanceApi.TradesLimitMax + 1));
                await Assert.ThrowsAsync<ArgumentException>("endTime", () => api.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime: startTime, endTime: endTime));
            }
        }

        [Fact]
        public async Task GetCandlesticksThrows()
        {
            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetCandlesticksAsync(null, KlineInterval.Day));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Day, limit: -1));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Day, limit: 0));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Day, limit: BinanceApi.CandlesticksLimitMax + 1));
            }
        }

        [Fact]
        public async Task Get24hStatsThrows()
        {
            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.Get24hStatsAsync(null));
            }
        }

        #endregion Market Data

        #region Account

        [Fact]
        public async Task PlaceOrderThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;
            var orderSide = OrderSide.Sell;
            var orderType = OrderType.Market;
            decimal quantity = 1;
            decimal price = 0;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.PlaceOrderAsync(null, symbol, orderSide, orderType, quantity, price));
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.PlaceOrderAsync(user, null, orderSide, orderType, quantity, price));
                await Assert.ThrowsAsync<ArgumentException>("quantity", () => api.PlaceOrderAsync(user, symbol, orderSide, orderType, -1, price));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.PlaceOrderAsync(user, symbol, orderSide, orderType, quantity, price, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.PlaceOrderAsync(user, symbol, orderSide, orderType, quantity, price, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetOrderThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;
            long orderId = 0;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetOrderAsync(null, symbol));
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetOrderAsync(user, null));
                await Assert.ThrowsAsync<ArgumentException>(() => api.GetOrderAsync(user, symbol));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrderAsync(user, symbol, orderId, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrderAsync(user, symbol, orderId, recvWindow: 0));
            }
        }

        [Fact]
        public async Task CancelOrderThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;
            long orderId = 0;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.CancelOrderAsync(null, symbol));
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.CancelOrderAsync(user, null));
                await Assert.ThrowsAsync<ArgumentException>(() => api.GetOrderAsync(user, symbol));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrderAsync(user, symbol, orderId, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrderAsync(user, symbol, orderId, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetOpenOrdersThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetOpenOrdersAsync(null, symbol));
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetOpenOrdersAsync(user, null));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOpenOrdersAsync(user, symbol, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOpenOrdersAsync(user, symbol, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetOrdersThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetOrdersAsync(null, symbol));
                await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => api.GetOrdersAsync(user, null));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrdersAsync(user, symbol, limit: -1));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrdersAsync(user, symbol, limit: 0));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetOrdersAsync(user, symbol, limit: BinanceApi.OrdersLimitMax + 1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrdersAsync(user, symbol, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetOrdersAsync(user, symbol, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetAccountThrows()
        {
            var user = new BinanceUser("api-key");

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetAccountAsync(null));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetAccountAsync(user, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetAccountAsync(user, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetTradesThrows()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetTradesAsync(null, symbol));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetTradesAsync(user, symbol, limit: -1));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetTradesAsync(user, symbol, limit: 0));
                await Assert.ThrowsAsync<ArgumentException>("limit", () => api.GetTradesAsync(user, symbol, limit: BinanceApi.TradesLimitMax + 1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetTradesAsync(user, symbol, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetTradesAsync(user, symbol, recvWindow: 0));
            }
        }

        [Fact]
        public async Task WithdrawThrows()
        {
            var user = new BinanceUser("api-key");
            var asset = Asset.BTC;
            var address = "12345678901234567890";
            decimal amount = 1;

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.WithdrawAsync(null, asset, address, amount));
                await Assert.ThrowsAsync<ArgumentNullException>("asset", () => api.WithdrawAsync(user, null, address, amount));
                await Assert.ThrowsAsync<ArgumentNullException>("address", () => api.WithdrawAsync(user, asset, null, amount));
                await Assert.ThrowsAsync<ArgumentNullException>("address", () => api.WithdrawAsync(user, asset, string.Empty, amount));
                await Assert.ThrowsAsync<ArgumentException>("amount", () => api.WithdrawAsync(user, asset, address, -1));
                await Assert.ThrowsAsync<ArgumentException>("amount", () => api.WithdrawAsync(user, asset, address, 0));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.WithdrawAsync(user, asset, address, amount, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.WithdrawAsync(user, asset, address, amount, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetDepositsThrows()
        {
            var user = new BinanceUser("api-key");

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetDepositsAsync(null));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetDepositsAsync(user, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetDepositsAsync(user, recvWindow: 0));
            }
        }

        [Fact]
        public async Task GetWithdrawalsThrows()
        {
            var user = new BinanceUser("api-key");

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.GetWithdrawalsAsync(null));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetDepositsAsync(user, recvWindow: -1));
                await Assert.ThrowsAsync<ArgumentException>("recvWindow", () => api.GetDepositsAsync(user, recvWindow: 0));
            }
        }

        #endregion Account

        #region User Stream

        [Fact]
        public async Task UserStreamStartThrows()
        {
            var user = new BinanceUser("api-key");

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.UserStreamStartAsync(null));
            }
        }

        [Fact]
        public async Task UserStreamKeepAliveThrows()
        {
            var user = new BinanceUser("api-key");
            var listenKey = "listen-key";

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.UserStreamKeepAliveAsync(null, listenKey));
                await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => api.UserStreamKeepAliveAsync(user, null));
                await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => api.UserStreamKeepAliveAsync(user, string.Empty));
            }
        }

        [Fact]
        public async Task UserStreamCloseThrows()
        {
            var user = new BinanceUser("api-key");
            var listenKey = "listen-key";

            using (var api = new BinanceJsonApi())
            {
                await Assert.ThrowsAsync<ArgumentNullException>("user", () => api.UserStreamCloseAsync(null, listenKey));
                await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => api.UserStreamCloseAsync(user, null));
                await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => api.UserStreamCloseAsync(user, string.Empty));
            }
        }

        #endregion User Stream
    }
}
