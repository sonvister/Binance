using Binance.Trades;
using System;
using Xunit;

namespace Binance.Tests.Accounts
{
    public class AggregateTradeTests
    {
        [Fact]
        public void Throws()
        {
            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            bool isBuyerMaker = true;
            bool isBestPriceMatch = true;

            Assert.Throws<ArgumentNullException>("symbol", () => new AggregateTrade(null, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("id", () => new AggregateTrade(symbol, -1, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("price", () => new AggregateTrade(symbol, id, -1, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("quantity", () => new AggregateTrade(symbol, id, price, -1, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("quantity", () => new AggregateTrade(symbol, id, price, 0, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("firstTradeId", () => new AggregateTrade(symbol, id, price, quantity, -1, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("lastTradeId", () => new AggregateTrade(symbol, id, price, quantity, firstTradeId, -1, timestamp, isBuyerMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("lastTradeId", () => new AggregateTrade(symbol, id, price, quantity, lastTradeId, firstTradeId, timestamp, isBuyerMaker, isBestPriceMatch));
        }

        [Fact]
        public void Properties()
        {
            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long firstTradeId = 123456;
            long lastTradeId = 234567;
            bool isBuyerMaker = true;
            bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            Assert.Equal(symbol, trade.Symbol);
            Assert.Equal(id, trade.Id);
            Assert.Equal(price, trade.Price);
            Assert.Equal(quantity, trade.Quantity);
            Assert.Equal(firstTradeId, trade.FirstTradeId);
            Assert.Equal(lastTradeId, trade.LastTradeId);
            Assert.Equal(timestamp, trade.Timestamp);
            Assert.Equal(isBuyerMaker, trade.IsBuyerMaker);
            Assert.Equal(isBestPriceMatch, trade.IsBestPriceMatch);
        }
    }
}
