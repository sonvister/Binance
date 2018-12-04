using System;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class OrderTest
    {
        [Fact]
        public void Throws()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            const int id = 123456;
            const string clientOrderId = "test-order";
            const decimal price = 4999;
            const decimal originalQuantity = 1;
            const decimal executedQuantity = 0.5m;
            const decimal cummulativeQuoteAssetQuantity = executedQuantity * price;
            const OrderStatus status = OrderStatus.PartiallyFilled;
            const TimeInForce timeInForce = TimeInForce.IOC;
            const OrderType orderType = OrderType.Market;
            const OrderSide orderSide = OrderSide.Sell;
            const decimal stopPrice = 5000;
            const decimal icebergQuantity = 0.1m;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isWorking = true;

            Assert.Throws<ArgumentNullException>("user", () => new Order(null, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentNullException>("symbol", () => new Order(user, null, id, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("id", () => new Order(user, symbol, -1, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("price", () => new Order(user, symbol, id, clientOrderId, -1, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("stopPrice", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, -1, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("originalQuantity", () => new Order(user, symbol, id, clientOrderId, price, -1, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("executedQuantity", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, -1, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking));
            Assert.Throws<ArgumentException>("icebergQuantity", () => new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, -1, time, time, isWorking));
        }

        [Fact]
        public void Properties()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            const int id = 123456;
            const string clientOrderId = "test-order";
            const decimal price = 4999;
            const decimal originalQuantity = 1;
            const decimal executedQuantity = 0.5m;
            const decimal cummulativeQuoteAssetQuantity = executedQuantity * price;
            const OrderStatus status = OrderStatus.PartiallyFilled;
            const TimeInForce timeInForce = TimeInForce.IOC;
            const OrderType orderType = OrderType.Market;
            const OrderSide orderSide = OrderSide.Sell;
            const decimal stopPrice = 5000;
            const decimal icebergQuantity = 0.1m;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isWorking = true;

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, cummulativeQuoteAssetQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking);

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
            Assert.Equal(time, order.Time);
            Assert.Equal(isWorking, order.IsWorking);
        }
    }
}
