using System;
using Xunit;

namespace Binance.Market.Tests
{
    public class OrderBookTopTest
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            decimal bidPrice = 0.123456789m;
            decimal bidQuantity = 0.987654321m;
            decimal askQuantity = 1.987654321m;

            Assert.Throws<ArgumentException>("askPrice", () => new OrderBookTop(symbol, bidPrice, bidQuantity, bidPrice - 1, askQuantity));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            decimal bidPrice = 0.123456789m;
            decimal bidQuantity = 0.987654321m;
            decimal askPrice = 1.123456789m;
            decimal askQuantity = 1.987654321m;

            var top = new OrderBookTop(symbol, bidPrice, bidQuantity, askPrice, askQuantity);

            Assert.Equal(bidPrice, top.Bid.Price);
            Assert.Equal(bidQuantity, top.Bid.Quantity);

            Assert.Equal(askPrice, top.Ask.Price);
            Assert.Equal(askQuantity, top.Ask.Quantity);
        }

        [Fact]
        public void Clone()
        {
            var symbol = Symbol.BTC_USDT;
            decimal bidPrice = 0.123456789m;
            decimal bidQuantity = 0.987654321m;
            decimal askPrice = 1.123456789m;
            decimal askQuantity = 1.987654321m;

            var top = new OrderBookTop(symbol, bidPrice, bidQuantity, askPrice, askQuantity);

            var clone = top.Clone();

            Assert.Equal(bidPrice, top.Bid.Price);
            Assert.Equal(bidQuantity, top.Bid.Quantity);

            Assert.Equal(askPrice, top.Ask.Price);
            Assert.Equal(askQuantity, top.Ask.Quantity);

            Assert.NotEqual(clone, top);
        }
    }
}
