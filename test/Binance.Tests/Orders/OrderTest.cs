using Binance.Orders;
using System;
using Xunit;

namespace Binance.Tests.Orders
{
    public class OrderTest
    {
        [Fact]
        public void Throws()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;
            var id = 123456;
            var clientOrderId = "test-order";
            decimal price = 4999;
            decimal originalQuantity = 1;
            decimal executedQuantity = 0.5m;
            var status = OrderStatus.PartiallyFilled;
            var timeInForce = TimeInForce.IOC;
            var orderType = OrderType.Market;
            var orderSide = OrderSide.Sell;
            decimal stopPrice = 5000;
            decimal icebergQuantity = 0.1m;
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Assert.Throws<ArgumentNullException>("user", () => new Order(null, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentNullException>("symbol", () => new Order(user, null, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("id", () => new Order(user, symbol, -1, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("price", () => new Order(user, symbol, id, clientOrderId, -1, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("stopPrice", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, -1, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("originalQuantity", () => new Order(user, symbol, id, clientOrderId, price, -1, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("executedQuantity", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, -1, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp));
            Assert.Throws<ArgumentException>("icebergQuantity", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, -1, timestamp));
            Assert.Throws<ArgumentException>("timestamp", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, -1));
            Assert.Throws<ArgumentException>("timestamp", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, 0));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceUser("api-key");
            var symbol = Symbol.BTC_USDT;
            var id = 123456;
            var clientOrderId = "test-order";
            decimal price = 4999;
            decimal originalQuantity = 1;
            decimal executedQuantity = 0.5m;
            var status = OrderStatus.PartiallyFilled;
            var timeInForce = TimeInForce.IOC;
            var orderType = OrderType.Market;
            var orderSide = OrderSide.Sell;
            decimal stopPrice = 5000;
            decimal icebergQuantity = 0.1m;
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp);

            Assert.Equal(user, order.User);
            Assert.Equal(symbol, order.Symbol);
            Assert.Equal(id, order.Id);
            Assert.Equal(clientOrderId, order.ClientOrderId);
            Assert.Equal(price, order.Price);
            Assert.Equal(originalQuantity, order.OriginalQuantity);
            Assert.Equal(executedQuantity, order.ExecutedQuantity);
            Assert.Equal(status, order.Status);
            Assert.Equal(timeInForce, order.TimeInForce);
            Assert.Equal(orderType, order.Type);
            Assert.Equal(orderSide, order.Side);
            Assert.Equal(stopPrice, order.StopPrice);
            Assert.Equal(icebergQuantity, order.IcebergQuantity);
            Assert.Equal(timestamp, order.Timestamp);
        }
    }
}
