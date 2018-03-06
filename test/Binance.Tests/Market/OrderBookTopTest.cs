using System;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Market
{
    public class OrderBookTopTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal bidPrice = 0.123456789m;
            const decimal bidQuantity = 0.987654321m;
            const decimal askQuantity = 1.987654321m;

            Assert.Throws<ArgumentException>("Price", () => OrderBookTop.Create(symbol, bidPrice, bidQuantity, bidPrice - 0.1m, askQuantity));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal bidPrice = 0.123456789m;
            const decimal bidQuantity = 0.987654321m;
            const decimal askPrice = 1.123456789m;
            const decimal askQuantity = 1.987654321m;

            var top = OrderBookTop.Create(symbol, bidPrice, bidQuantity, askPrice, askQuantity);

            Assert.Equal(bidPrice, top.Bid.Price);
            Assert.Equal(bidQuantity, top.Bid.Quantity);

            Assert.Equal(askPrice, top.Ask.Price);
            Assert.Equal(askQuantity, top.Ask.Quantity);
        }

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const decimal bidPrice = 0.123456789m;
            const decimal bidQuantity = 0.987654321m;
            const decimal askPrice = 1.123456789m;
            const decimal askQuantity = 1.987654321m;

            var top = OrderBookTop.Create(symbol, bidPrice, bidQuantity, askPrice, askQuantity);

            var json = JsonConvert.SerializeObject(top);

            top = JsonConvert.DeserializeObject<OrderBookTop>(json);

            Assert.Equal(bidPrice, top.Bid.Price);
            Assert.Equal(bidQuantity, top.Bid.Quantity);

            Assert.Equal(askPrice, top.Ask.Price);
            Assert.Equal(askQuantity, top.Ask.Quantity);
        }
    }
}
