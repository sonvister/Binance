using System;
using Binance.Api.WebSocket.Events;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class AggregateTradeEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            Assert.Throws<ArgumentException>("timestamp", () => new AggregateTradeEventArgs(-1, trade));
            Assert.Throws<ArgumentNullException>("trade", () => new AggregateTradeEventArgs(timestamp, null));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            var args = new AggregateTradeEventArgs(timestamp, trade);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(trade, args.Trade);
        }
    }
}
