using System;
using Binance.Cache.Events;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Cache.Events
{
    public class OrderBookCacheEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("orderBook", () => new OrderBookCacheEventArgs(null));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567899;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            var args = new OrderBookCacheEventArgs(orderBook);

            Assert.Equal(orderBook, args.OrderBook);
        }
    }
}
