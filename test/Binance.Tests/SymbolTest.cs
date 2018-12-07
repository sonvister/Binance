using System;
using System.Linq;
using Moq;
using Xunit;

namespace Binance.Tests
{
    public class SymbolTest
    {
        [Fact]
        public void Throws()
        {
            const SymbolStatus status = SymbolStatus.Trading;
            var baseAsset = Asset.BTC;
            var quoteAsset = Asset.USDT;
            var quantityRange = new InclusiveRange(0.01m, 10.0m, 0.01m);
            var priceRange = new InclusiveRange(0.01m, 100.0m, 0.01m);
            const decimal minNotionalValue = 0.001m;
            const bool isIcebergAllowed = true;
            var orderTypes = new [] { OrderType.Limit, OrderType.Market };

            Assert.Throws<ArgumentNullException>("baseAsset", () => new Symbol(status, null, quoteAsset, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, orderTypes));
            Assert.Throws<ArgumentNullException>("quoteAsset", () => new Symbol(status, baseAsset, null, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, orderTypes));
            Assert.Throws<ArgumentNullException>("quantity", () => new Symbol(status, baseAsset, quoteAsset, null, priceRange, minNotionalValue, isIcebergAllowed, orderTypes));
            Assert.Throws<ArgumentNullException>("price", () => new Symbol(status, baseAsset, quoteAsset, quantityRange, null, minNotionalValue, isIcebergAllowed, orderTypes));
            Assert.Throws<ArgumentNullException>("orderTypes", () => new Symbol(status, baseAsset, quoteAsset, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, null));
        }

        [Fact]
        public void ImplicitOperators()
        {
            var symbol1 = Symbol.BTC_USDT;
            var symbol2 = new Symbol(symbol1.Status, symbol1.BaseAsset, symbol1.QuoteAsset, symbol1.Quantity, symbol1.Price, symbol1.NotionalMinimumValue, symbol1.IsIcebergAllowed, symbol1.OrderTypes);
            var symbol3 = Symbol.XRP_USDT;

            Assert.True(symbol1 == symbol2);
            Assert.True(symbol1 != symbol3);

            Assert.True(symbol1 == symbol1.ToString());
            Assert.True(symbol1 == symbol2.ToString());
            Assert.True(symbol1 != symbol3.ToString());

            var baseAsset = new Asset("TEST", 8);
            var quoteAsset = Asset.BTC;
            var quantityRange = new InclusiveRange(0.01m, 10.0m, 0.01m);
            var priceRange = new InclusiveRange(0.01m, 100.0m, 0.01m);
            const decimal minNotionalValue = 0.001m;
            const bool isIcebergAllowed = true;
            var orderTypes = new[] { OrderType.Limit, OrderType.Market };

            var newSymbol = new Symbol(SymbolStatus.Trading, baseAsset, quoteAsset, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, orderTypes);

            Assert.True(newSymbol == baseAsset.Symbol + quoteAsset.Symbol);
        }

        [Fact]
        public void Properties()
        {
            const SymbolStatus status = SymbolStatus.Trading;
            var baseAsset = Asset.BTC;
            var quoteAsset = Asset.USDT;
            var quantityRange = new InclusiveRange(0.01m, 10.0m, 0.01m);
            var priceRange = new InclusiveRange(0.01m, 100.0m, 0.01m);
            const decimal minNotionalValue = 0.001m;
            const bool isIcebergAllowed = true;
            var orderTypes = new [] { OrderType.Limit, OrderType.Market };

            var symbol = new Symbol(status, baseAsset, quoteAsset, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, orderTypes);

            Assert.Equal(status, symbol.Status);
            Assert.Equal(baseAsset, symbol.BaseAsset);
            Assert.Equal(quoteAsset, symbol.QuoteAsset);
            Assert.Equal(quantityRange, symbol.Quantity);
            Assert.Equal(priceRange, symbol.Price);
            Assert.Equal(minNotionalValue, symbol.NotionalMinimumValue);
            Assert.Equal(orderTypes, symbol.OrderTypes);
        }

        [Fact]
        public void IsValid()
        {
            const SymbolStatus status = SymbolStatus.Trading;
            var quoteAsset = Asset.USDT;
            var quantityRange = new InclusiveRange(0.01m, 10.0m, 0.01m);
            var priceRange = new InclusiveRange(0.01m, 100.0m, 0.01m);
            const decimal minNotionalValue = 0.001m;
            const bool isIcebergAllowed = true;
            var orderTypes = new [] { OrderType.Limit, OrderType.Market };

            var validSymbol = Symbol.BTC_USDT;
            var invalidSymbol = new Symbol(status, new Asset("...", 0), quoteAsset, quantityRange, priceRange, minNotionalValue, isIcebergAllowed, orderTypes);

            Assert.True(Symbol.IsValid(validSymbol));
            Assert.False(Symbol.IsValid(invalidSymbol));
        }

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
            //const decimal price = 10000;
            //const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            /* TODO
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
            */
        }

        [Fact]
        public void IsValidOrder()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;
            //const decimal price = 10000;
            //const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            /* TODO
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
            */
        }

        [Fact]
        public void ValidateOrderType()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;

            Assert.Contains(OrderType.Limit, symbol.OrderTypes);

            symbol.Validate(OrderType.Limit);

            var order = new LimitOrder(new Mock<IBinanceApiUser>().Object);

            symbol.ValidateOrderType(order);

            var takeProfitLimitOrder = new TakeProfitLimitOrder(user);

            if (symbol.OrderTypes.Contains(OrderType.TakeProfitLimit))
            {
                symbol.ValidateOrderType(takeProfitLimitOrder);
            }
            else
            {
                Assert.Throws<ArgumentException>(nameof(takeProfitLimitOrder.Type), () => symbol.ValidateOrderType(takeProfitLimitOrder));
            }
        }

        [Fact]
        public void ValidatePriceQuantity()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;
            //const decimal price = 10000;
            //const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            /* TODO
            symbol.ValidatePriceQuantity(price, quantity);

            Assert.Throws<ArgumentOutOfRangeException>("quantity", () => symbol.ValidatePriceQuantity(price, 0.000000001m));
            Assert.Throws<ArgumentOutOfRangeException>("price", () => symbol.ValidatePriceQuantity(0.000000001m, quantity));

            Assert.Throws<ArgumentOutOfRangeException>("quantity", () => symbol.ValidatePriceQuantity(price, symbol.Quantity.Maximum + symbol.Quantity.Increment));
            Assert.Throws<ArgumentOutOfRangeException>("price", () => symbol.ValidatePriceQuantity(symbol.Price.Minimum - symbol.Price.Increment, quantity));

            symbol.IsPriceQuantityValid(price, symbol.NotionalMinimumValue / price);
            Assert.Throws<ArgumentOutOfRangeException>("notionalValue", () => symbol.ValidatePriceQuantity(price, symbol.NotionalMinimumValue / price - symbol.Quantity.Increment));

            var order = new LimitOrder(user)
            {
                Price = price,
                Quantity = quantity
            };

            symbol.ValidatePriceQuantity(order);

            order.Price = price;
            order.Quantity = 0.000000001m;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Quantity), () => symbol.ValidatePriceQuantity(order));

            order.Price = 0.000000001m;
            order.Quantity = quantity;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Price), () => symbol.ValidatePriceQuantity(order));

            order.Price = price;
            order.Quantity = symbol.Quantity.Maximum + symbol.Quantity.Increment;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Quantity), () => symbol.ValidatePriceQuantity(order));

            order.Price = symbol.Price.Minimum - symbol.Price.Increment;
            order.Quantity = quantity;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Price), () => symbol.ValidatePriceQuantity(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price;
            symbol.ValidatePriceQuantity(order);

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price - symbol.Quantity.Increment;
            Assert.Throws<ArgumentOutOfRangeException>("notionalValue", () => symbol.ValidatePriceQuantity(order));
            */
        }

        [Fact]
        public void Validate()
        {
            var symbol = Symbol.BTC_USDT;
            var user = new Mock<IBinanceApiUser>().Object;
            //const decimal price = 10000;
            //const decimal quantity = 1;

            Assert.Equal(8, symbol.BaseAsset.Precision);
            Assert.Equal(8, symbol.QuoteAsset.Precision);

            /* TODO
            var order = new LimitOrder(user);

            Assert.Throws<ArgumentException>("Symbol", () => symbol.Validate(order));

            order.Symbol = symbol;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Quantity), () => symbol.Validate(order));

            order.Quantity = quantity;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Price), () => symbol.Validate(order));

            order.Price = price;
            symbol.Validate(order);

            order.Price = price;
            order.Quantity = 0.000000001m;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Quantity), () => symbol.Validate(order));

            order.Price = 0.000000001m;
            order.Quantity = quantity;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Price), () => symbol.Validate(order));

            order.Price = price;
            order.Quantity = symbol.Quantity.Maximum + symbol.Quantity.Increment;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Quantity), () => symbol.Validate(order));

            order.Price = symbol.Price.Minimum - symbol.Price.Increment;
            order.Quantity = quantity;
            Assert.Throws<ArgumentOutOfRangeException>(nameof(order.Price), () => symbol.Validate(order));

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price;
            symbol.Validate(order);

            order.Price = price;
            order.Quantity = symbol.NotionalMinimumValue / price - symbol.Quantity.Increment;
            Assert.Throws<ArgumentOutOfRangeException>("notionalValue", () => symbol.Validate(order));
            */
        }
    }
}
