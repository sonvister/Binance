using System;
using System.Threading.Tasks;
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
        public async Task GetAggregateTradesThrows()
        {
            var startTime = DateTime.UtcNow.AddHours(-1);
            var endTime = DateTime.UtcNow;
            var localTime = DateTime.Now;

            await Assert.ThrowsAsync<ArgumentException>("startTime", () => _api.GetAggregateTradesAsync(Symbol.BTC_USDT, localTime, endTime));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesAsync(Symbol.BTC_USDT, startTime, localTime));
            await Assert.ThrowsAsync<ArgumentException>("endTime", () => _api.GetAggregateTradesAsync(Symbol.BTC_USDT, endTime, startTime));
        }

        #endregion Market Data

        #region Account

        [Fact]
        public async Task PlaceThrows()
        {
            var user = new BinanceApiUser("api-key");

            await Assert.ThrowsAsync<ArgumentNullException>("clientOrder", () => _api.PlaceAsync(null));
        }

        [Fact]
        public async Task TestPlaceThrows()
        {
            var user = new BinanceApiUser("api-key");

            await Assert.ThrowsAsync<ArgumentNullException>("clientOrder", () => _api.TestPlaceAsync(null));
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
