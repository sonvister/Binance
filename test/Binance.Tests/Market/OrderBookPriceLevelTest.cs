using System;
using Binance.Market;
using Xunit;

namespace Binance.Tests.Market
{
    public class OrderBookPriceLevelTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentException>("price", () => new OrderBookPriceLevel(-1, 1));
            Assert.Throws<ArgumentException>("quantity", () => new OrderBookPriceLevel(1, -1));
        }

        [Fact]
        public void Zeroed()
        {
            const decimal price = 0;
            const decimal quantity = 0;

            var level = new OrderBookPriceLevel(price, quantity);

            Assert.Equal(price, level.Price);
            Assert.Equal(quantity, level.Quantity);
        }

        [Fact]
        public void Properties()
        {
            const decimal price = 0.123456789m;
            const decimal quantity = 0.987654321m;

            var level = new OrderBookPriceLevel(price, quantity);

            Assert.Equal(price, level.Price);
            Assert.Equal(quantity, level.Quantity);
        }
    }
}
