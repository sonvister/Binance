using System;
using System.Threading.Tasks;
using Binance.Account.Orders;
using Binance.Api;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable PossibleMultipleEnumeration

namespace Binance.Tests.Api
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

        #region Market Data

        [Fact]
        public Task GetAggregateTradeFromThrows()
        {
            return Assert.ThrowsAsync<ArgumentException>("fromId", () => _api.GetAggregateTradesFromAsync(Symbol.BTC_USDT, -1));
        }

        [Fact]
        public async Task GetAggregateTradeInThrows()
        {
            await Assert.ThrowsAsync<ArgumentException>("startTime", () => _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, -1, 1));
            await Assert.ThrowsAsync<ArgumentException>("startTime", () => _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, 0, 1));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, 1, -1));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, 1, 0));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesInAsync(Symbol.BTC_USDT, 2, 1));
        }

        #endregion Market Data

        #region Account

        [Fact]
        public async Task PlaceThrows()
        {
            var user = new BinanceApiUser("api-key");

            await Assert.ThrowsAsync<ArgumentNullException>("clientOrder", () => _api.PlaceAsync(null));
            await Assert.ThrowsAsync<InvalidOperationException>(() => _api.PlaceAsync(new LimitOrder(user) { Symbol = Symbol.BTC_USDT, Quantity = 0.01m }));
        }

        [Fact]
        public async Task TestPlaceThrows()
        {
            var user = new BinanceApiUser("api-key");

            await Assert.ThrowsAsync<ArgumentNullException>("clientOrder", () => _api.TestPlaceAsync(null));
            await Assert.ThrowsAsync<InvalidOperationException>(() => _api.TestPlaceAsync(new LimitOrder(user) { Symbol = Symbol.BTC_USDT, Quantity = 0.01m }));
        }

        [Fact]
        public Task GetOrderThrows()
        {
            return Assert.ThrowsAsync<ArgumentNullException>("order", () => _api.GetAsync(null));
        }

        [Fact]
        public async Task CancelOrderThrows()
        {
            var user = new BinanceApiUser("api-key");

            await Assert.ThrowsAsync<ArgumentException>("orderId", () => _api.CancelOrderAsync(user, Symbol.BTC_USDT, BinanceApi.NullId));
            await Assert.ThrowsAsync<ArgumentNullException>("origClientOrderId", () => _api.CancelOrderAsync(user, Symbol.BTC_USDT, null));
            await Assert.ThrowsAsync<ArgumentNullException>("origClientOrderId", () => _api.CancelOrderAsync(user, Symbol.BTC_USDT, string.Empty));
            await Assert.ThrowsAsync<ArgumentNullException>("order", () => _api.CancelAsync(null));
        }

        #endregion Account
    }
}
