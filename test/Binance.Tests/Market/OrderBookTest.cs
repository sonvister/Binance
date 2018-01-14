using System;
using Binance.Market;
using Binance.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Market
{
    public class OrderBookTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            Assert.Throws<ArgumentNullException>("symbol", () => new OrderBook(null, lastUpdateId, bids, asks));
            Assert.Throws<ArgumentNullException>("symbol", () => new OrderBook("", lastUpdateId, bids, asks));

            Assert.Throws<ArgumentException>("lastUpdateId", () => new OrderBook(symbol, -1, bids, asks));
            Assert.Throws<ArgumentException>("lastUpdateId", () => new OrderBook(symbol, 0, bids, asks));

            Assert.Throws<ArgumentNullException>("bids", () => new OrderBook(symbol, lastUpdateId, null, asks));
            Assert.Throws<ArgumentNullException>("asks", () => new OrderBook(symbol, lastUpdateId, bids, null));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(symbol, orderBook.Symbol);
            Assert.Equal(lastUpdateId, orderBook.LastUpdateId);

            Assert.NotEmpty(orderBook.Bids);
            Assert.NotEmpty(orderBook.Asks);

            Assert.Equal(3, orderBook.Top.Bid.Price);
            Assert.Equal(30, orderBook.Top.Bid.Quantity);

            Assert.Equal(4, orderBook.Top.Ask.Price);
            Assert.Equal(40, orderBook.Top.Ask.Quantity);
        }

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new OrderBookJsonConverter());

            var json = JsonConvert.SerializeObject(orderBook, settings);

            orderBook = JsonConvert.DeserializeObject<OrderBook>(json, settings);

            Assert.Equal(symbol, orderBook.Symbol);
            Assert.Equal(lastUpdateId, orderBook.LastUpdateId);

            Assert.NotEmpty(orderBook.Bids);
            Assert.NotEmpty(orderBook.Asks);

            Assert.Equal(3, orderBook.Top.Bid.Price);
            Assert.Equal(30, orderBook.Top.Bid.Quantity);

            Assert.Equal(4, orderBook.Top.Ask.Price);
            Assert.Equal(40, orderBook.Top.Ask.Quantity);
        }

        [Fact]
        public void Clone()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            var clone = orderBook.Clone();

            Assert.Equal(symbol, clone.Symbol);
            Assert.Equal(lastUpdateId, clone.LastUpdateId);

            foreach (var level in bids)
                Assert.Equal(level.Item2, clone.Quantity(level.Item1));

            foreach (var level in asks)
                Assert.Equal(level.Item2, clone.Quantity(level.Item1));

            Assert.Equal(3, orderBook.Top.Bid.Price);
            Assert.Equal(30, orderBook.Top.Bid.Quantity);

            Assert.Equal(4, orderBook.Top.Ask.Price);
            Assert.Equal(40, orderBook.Top.Ask.Quantity);

            Assert.NotEqual(clone, orderBook);
        }

        [Fact]
        public void Quantity()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            foreach (var level in bids)
                Assert.Equal(level.Item2, orderBook.Quantity(level.Item1));

            foreach (var level in asks)
                Assert.Equal(level.Item2, orderBook.Quantity(level.Item1));

            Assert.Equal(0, orderBook.Quantity(0.5m));
            Assert.Equal(0, orderBook.Quantity(10.0m));
        }

        [Fact]
        public void MidMarketPrice()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(3.5m, orderBook.MidMarketPrice());
        }

        [Fact]
        public void Depth()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(0, orderBook.Depth(3.5m));
            Assert.Equal(50, orderBook.Depth(1.5m));
            Assert.Equal(90, orderBook.Depth(5.5m));
        }

        [Fact]
        public void Volume()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(0, orderBook.Volume(3.5m));
            Assert.Equal(130, orderBook.Volume(1.5m)); // 3 * 30 + 2 * 20
            Assert.Equal(410, orderBook.Volume(5.5m)); // 4 * 40 + 5 * 50
        }

        [Fact]
        public void BidPrice()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(3, orderBook.Bids.PriceAt(15));
            Assert.Equal(2, orderBook.Bids.PriceAt(40));
            Assert.Equal(1, orderBook.Bids.PriceAt(55));
            Assert.Equal(1, orderBook.Bids.PriceAt(80));
        }

        [Fact]
        public void AskPrice()
        {
            var symbol = Symbol.BTC_USDT;
            const long lastUpdateId = 1234567890;
            var bids = new(decimal, decimal)[] { (2, 20), (1, 10), (3, 30) };
            var asks = new(decimal, decimal)[] { (6, 60), (4, 40), (5, 50) };

            var orderBook = new OrderBook(symbol, lastUpdateId, bids, asks);

            Assert.Equal(4, orderBook.Asks.PriceAt(20));
            Assert.Equal(5, orderBook.Asks.PriceAt(65));
            Assert.Equal(6, orderBook.Asks.PriceAt(120));
            Assert.Equal(6, orderBook.Asks.PriceAt(200));
        }
    }
}
