using System;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class OrderSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            const int id = 123456;
            const string clientOrderId = "test-order";
            const decimal price = 4999;
            const decimal originalQuantity = 1;
            const decimal executedQuantity = 0.5m;
            const OrderStatus status = OrderStatus.PartiallyFilled;
            const TimeInForce timeInForce = TimeInForce.IOC;
            const OrderType orderType = OrderType.Market;
            const OrderSide orderSide = OrderSide.Sell;
            const decimal stopPrice = 5000;
            const decimal icebergQuantity = 0.1m;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isWorking = true;
            Fill[] fills =
            {
                new Fill(price, originalQuantity, 0.001m, "BNB", 12345678990)
            };

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, time, isWorking, fills);

            var serializer = new OrderSerializer();

            var json = serializer.Serialize(order);

            var other = serializer.Deserialize(json, user);

            Assert.True(order.Equals(other));
        }
    }
}
