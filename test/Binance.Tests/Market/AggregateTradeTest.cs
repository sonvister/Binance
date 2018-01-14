using System;
using Binance.Market;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Market
{
    public class AggregateTradeTests
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

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
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

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

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const long firstTradeId = 123456;
            const long lastTradeId = 234567;
            const bool isBuyerMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AggregateTrade(symbol, id, price, quantity, firstTradeId, lastTradeId, timestamp, isBuyerMaker, isBestPriceMatch);

            var json = JsonConvert.SerializeObject(trade);

            trade = JsonConvert.DeserializeObject<AggregateTrade>(json);

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
