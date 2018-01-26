using Binance.Cache.Events;
using Binance.Market;
using System;
using Xunit;

namespace Binance.Tests.Cache.Events
{
    public class TradeCacheEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("trades", () => new TradeCacheEventArgs(null));
        }

        [Fact]
        public void Properties()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            const long buyerOrderId = 123456;
            const long sellerOrderId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new Trade(symbol, id, price, quantity, buyerOrderId, sellerOrderId, time, isBuyerMaker, isBestPriceMatch);

            var trades = new[] { trade };

            var args = new TradeCacheEventArgs(trades);

            Assert.Equal(trades, args.Trades);
        }
    }
}
