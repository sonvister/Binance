using System;
using System.Threading;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Api.WebSocket.Events;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class OrderUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            const long id = 123456;
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

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp);

            const OrderExecutionType orderExecutionType = OrderExecutionType.New;
            const OrderRejectedReason orderRejectedReason = OrderRejectedReason.None;
            const string newClientOrderId = "new-test-order";

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentException>("timestamp", () => new OrderUpdateEventArgs(-1, cts.Token, order, orderExecutionType, orderRejectedReason, newClientOrderId));
                Assert.Throws<ArgumentException>("timestamp", () => new OrderUpdateEventArgs(0, cts.Token, order, orderExecutionType, orderRejectedReason, newClientOrderId));
                Assert.Throws<ArgumentNullException>("order", () => new OrderUpdateEventArgs(timestamp, cts.Token, null, orderExecutionType, orderRejectedReason, newClientOrderId));
            }
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var user = new BinanceApiUser("api-key");
            var symbol = Symbol.BTC_USDT;
            const long id = 123456;
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

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp);

            const OrderExecutionType orderExecutionType = OrderExecutionType.New;
            const OrderRejectedReason orderRejectedReason = OrderRejectedReason.None;
            const string newClientOrderId = "new-test-order";

            using (var cts = new CancellationTokenSource())
            {
                var args = new OrderUpdateEventArgs(timestamp, cts.Token, order, orderExecutionType, orderRejectedReason, newClientOrderId);

                Assert.Equal(timestamp, args.Timestamp);
                Assert.Equal(order, args.Order);
                Assert.Equal(orderExecutionType, args.OrderExecutionType);
                Assert.Equal(orderRejectedReason, args.OrderRejectedReason);
                Assert.Equal(newClientOrderId, args.NewClientOrderId);
            }
        }
    }
}
