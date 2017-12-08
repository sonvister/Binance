using System;
using System.Threading.Tasks;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Binance.Tests.Api
{
    public class BinanceHttpClientExtensionsTest
    {
        private readonly IBinanceHttpClient _client;

        public BinanceHttpClientExtensionsTest()
        {
            // Configure services.
            var serviceProvider = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            // Get IBinanceHttpClient service.
            _client = serviceProvider.GetService<IBinanceHttpClient>();
        }

        #region Market Data

        [Fact]
        public Task GetOrderBookThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetOrderBookAsync(null));
        }

        [Fact]
        public async Task GetAggregateTradesThrows()
        {
            var now = DateTimeOffset.UtcNow;

            var startTime = DateTimeOffset.UtcNow.AddHours(-25).ToUnixTimeMilliseconds();
            var endTime = now.ToUnixTimeMilliseconds();

            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetAggregateTradesAsync(null));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _client.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime: startTime, endTime: endTime));
        }

        [Fact]
        public Task GetCandlesticksThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetCandlesticksAsync(null, CandlestickInterval.Day));
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

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.PlaceOrderAsync(null, symbol, orderSide, orderType, quantity, price));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.PlaceOrderAsync(user, null, orderSide, orderType, quantity, price));
            await Assert.ThrowsAsync<ArgumentException>("quantity", () => _client.PlaceOrderAsync(user, symbol, orderSide, orderType, -1, price));
        }

        [Fact]
        public async Task GetOrderThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetOrderAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetOrderAsync(user, null));
            await Assert.ThrowsAsync<ArgumentException>(() => _client.GetOrderAsync(user, symbol));
        }

        [Fact]
        public async Task CancelOrderThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.CancelOrderAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.CancelOrderAsync(user, null));
            await Assert.ThrowsAsync<ArgumentException>(() => _client.GetOrderAsync(user, symbol));
        }

        [Fact]
        public async Task GetOpenOrdersThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetOpenOrdersAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetOpenOrdersAsync(user, null));
        }

        [Fact]
        public async Task GetOrdersThrows()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetOrdersAsync(null, symbol));
            await Assert.ThrowsAsync<ArgumentNullException>("symbol", () => _client.GetOrdersAsync(user, null));
        }

        [Fact]
        public Task GetAccountThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetAccountInfoAsync(null));
        }

        [Fact]
        public Task GetTradesThrows()
        {
            var symbol = Symbol.BTC_USDT;

            return Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetAccountTradesAsync(null, symbol));
        }

        [Fact]
        public async Task WithdrawThrows()
        {
            var user = new BinanceApiUser("api-key");
            var asset = Asset.BTC;
            const string address = "12345678901234567890";
            const decimal amount = 1;

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.WithdrawAsync(null, asset, address, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("asset", () => _client.WithdrawAsync(user, null, address, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("address", () => _client.WithdrawAsync(user, asset, null, amount));
            await Assert.ThrowsAsync<ArgumentNullException>("address", () => _client.WithdrawAsync(user, asset, string.Empty, amount));
            await Assert.ThrowsAsync<ArgumentException>("amount", () => _client.WithdrawAsync(user, asset, address, -1));
            await Assert.ThrowsAsync<ArgumentException>("amount", () => _client.WithdrawAsync(user, asset, address, 0));
        }

        [Fact]
        public Task GetDepositsThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetDepositsAsync(null));
        }

        [Fact]
        public Task GetWithdrawalsThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.GetWithdrawalsAsync(null));
        }

        #endregion Account

        #region User Stream

        [Fact]
        public async Task UserStreamStartThrows()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.UserStreamStartAsync(((IBinanceApiUser)null)));
            await Assert.ThrowsAsync<ArgumentNullException>("apiKey", () => _client.UserStreamStartAsync(((string)null)));
        }

        [Fact]
        public async Task UserStreamKeepAliveThrows()
        {
            var user = new BinanceApiUser("api-key");
            const string listenKey = "listen-key";

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.UserStreamKeepAliveAsync((IBinanceApiUser)null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("apiKey", () => _client.UserStreamKeepAliveAsync((string)null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _client.UserStreamKeepAliveAsync(user, null));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _client.UserStreamKeepAliveAsync(user, string.Empty));
        }

        [Fact]
        public async Task UserStreamCloseThrows()
        {
            var user = new BinanceApiUser("api-key");
            const string listenKey = "listen-key";

            await Assert.ThrowsAsync<ArgumentNullException>("user", () => _client.UserStreamCloseAsync((IBinanceApiUser)null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("apiKey", () => _client.UserStreamCloseAsync((string)null, listenKey));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _client.UserStreamCloseAsync(user, null));
            await Assert.ThrowsAsync<ArgumentNullException>("listenKey", () => _client.UserStreamCloseAsync(user, string.Empty));
        }

        #endregion User Stream
    }
}
