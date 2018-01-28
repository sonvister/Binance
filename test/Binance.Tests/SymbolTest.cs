using System.Linq;
using Binance.Account.Orders;
using Binance.Api;
using Moq;
using Xunit;

namespace Binance.Tests
{
    public class SymbolTest
    {
        [Fact]
        public void IsSupported()
        {
            var symbol = Symbol.BTC_USDT;

            Assert.Contains(OrderType.Limit, symbol.OrderTypes);

            Assert.True(symbol.IsSupported(OrderType.Limit));
        }

        [Fact]
        public void IsOrderTypeSupported()
        {
            var symbol = Symbol.BTC_USDT;
            var order = new LimitOrder(new Mock<IBinanceApiUser>().Object);

            Assert.Contains(OrderType.Limit, symbol.OrderTypes);

            Assert.True(symbol.IsOrderTypeSupported(order));
        }

        [Fact]
        public void IsPriceQuantityValid()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;
            const decimal price = 10000;
            const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            Assert.True(symbol.IsPriceQuantityValid(price, quantity));

            Assert.False(symbol.IsPriceQuantityValid(price, 0.000000001m));
            Assert.False(symbol.IsPriceQuantityValid(0.000000001m, quantity));

            Assert.False(symbol.IsPriceQuantityValid(price, symbol.Quantity.Maximum + symbol.Quantity.Increment));
            Assert.False(symbol.IsPriceQuantityValid(symbol.Price.Minimum - symbol.Price.Increment, quantity));

            Assert.True(symbol.IsPriceQuantityValid(price, symbol.NotionalMinimumValue / price));
            Assert.False(symbol.IsPriceQuantityValid(price, symbol.NotionalMinimumValue / price - symbol.Quantity.Increment));

            var order = new LimitOrder(user)
            {
                Price = price,
                Quantity = quantity
            };

            Assert.True(symbol.IsPriceQuantityValid(order));

            order.Price = price;
            order.Quantity = 0.000000001m;
            Assert.False(symbol.IsPriceQuantityValid(order));

            order.Price = 0.000000001m;
            order.Quantity = quantity;
            Assert.False(symbol.IsPriceQuantityValid(order));

            order.Price = price;
            order.Quantity = symbol.Quantity.Maximum + symbol.Quantity.Increment;
            Assert.False(symbol.IsPriceQuantityValid(order));

            order.Price = symbol.Price.Minimum - symbol.Price.Increment;
            order.Quantity = quantity;
            Assert.False(symbol.IsPriceQuantityValid(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price;
            Assert.True(symbol.IsPriceQuantityValid(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price - symbol.Quantity.Increment;
            Assert.False(symbol.IsPriceQuantityValid(order));
        }

        [Fact]
        public void IsValid()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;
            const decimal price = 10000;
            const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            var order = new LimitOrder(user);

            Assert.False(symbol.IsValid(order));

            order.Price = price;
            Assert.False(symbol.IsValid(order));

            order.Quantity = quantity;
            Assert.False(symbol.IsValid(order));

            order.Symbol = symbol;
            Assert.True(symbol.IsValid(order));

            var takeProfitLimitOrder = new TakeProfitLimitOrder(user)
            {
                Symbol = symbol,
                Price = price,
                Quantity = quantity
            };

            if (symbol.OrderTypes.Contains(OrderType.TakeProfitLimit))
            {
                Assert.True(symbol.IsValid(takeProfitLimitOrder));
            }
            else
            {
                Assert.False(symbol.IsValid(takeProfitLimitOrder));
            }

            order.Price = price;
            order.Quantity = 0.000000001m;
            Assert.False(symbol.IsValid(order));

            order.Price = 0.000000001m;
            order.Quantity = quantity;
            Assert.False(symbol.IsValid(order));

            order.Price = price;
            order.Quantity = symbol.Quantity.Maximum + symbol.Quantity.Increment;
            Assert.False(symbol.IsValid(order));

            order.Price = symbol.Price.Minimum - symbol.Price.Increment;
            order.Quantity = quantity;
            Assert.False(symbol.IsValid(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price;
            Assert.True(symbol.IsValid(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price - symbol.Quantity.Increment;
            Assert.False(symbol.IsValid(order));
        }
    }
}
