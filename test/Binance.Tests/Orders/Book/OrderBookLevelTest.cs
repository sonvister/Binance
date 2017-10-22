using System;
using Xunit;

namespace Binance.Orders.Book
{
    public class OrderBookLevelTest
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
            var price = 0;
            var quantity = 0;

            var level = new OrderBookPriceLevel(price, quantity);

            Assert.Equal(price, level.Price);
            Assert.Equal(quantity, level.Quantity);
        }

        [Fact]
        public void Properties()
        {
            var price = 0.123456789m;
            var quantity = 0.987654321m;

            var level = new OrderBookPriceLevel(price, quantity);

            Assert.Equal(price, level.Price);
            Assert.Equal(quantity, level.Quantity);
        }

        [Fact]
        public void Clone()
        {
            var price = 0.123456789m;
            var quantity = 0.987654321m;

            var level = new OrderBookPriceLevel(price, quantity);

            var clone = level.Clone();

            Assert.Equal(price, clone.Price);
            Assert.Equal(quantity, clone.Quantity);

            Assert.NotEqual(clone, level);
        }
    }
}
