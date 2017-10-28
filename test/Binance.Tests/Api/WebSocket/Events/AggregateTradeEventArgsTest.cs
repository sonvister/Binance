using Binance.Market;
using System;
using Xunit;

namespace Binance.Api.WebSocket.Events.Tests
{
    public class AggregateTradeEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            bool isBuyerMaker = true;
            bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            Assert.Throws<ArgumentException>("timestamp", () => new AggregateTradeEventArgs(-1, trade));
            Assert.Throws<ArgumentNullException>("trade", () => new AggregateTradeEventArgs(timestamp, null));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            bool isBuyerMaker = true;
            bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            var args = new AggregateTradeEventArgs(timestamp, trade);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(trade, args.Trade);
        }
    }
}
