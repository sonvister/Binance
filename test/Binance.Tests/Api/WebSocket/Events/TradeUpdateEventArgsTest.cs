using System;
using System.Threading;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api;
using Binance.WebSocket.Events;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class TradeUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

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
            const bool isWorking = true;

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, isWorking);

            const OrderRejectedReason orderRejectedReason = OrderRejectedReason.None;
            const string newClientOrderId = "new-test-order";

            const long tradeId = 12345;
            const long orderId = 54321;
            const decimal quantity = 1;
            const decimal commission = 10;
            const string commissionAsset = "BNB";
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, tradeId, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch);

            decimal quantityOfLastFilledTrade = 1;

            using (var cts = new CancellationTokenSource())
            {
                Assert.Throws<ArgumentNullException>("order", () => new AccountTradeUpdateEventArgs(time, cts.Token, null, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade));
            }
        }

        [Fact]
        public void Properties()
        {
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;

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
            const bool isWorking = true;

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, time, isWorking);

            const OrderRejectedReason orderRejectedReason = OrderRejectedReason.None;
            const string newClientOrderId = "new-test-order";

            const long tradeId = 12345;
            const long orderId = 54321;
            const decimal quantity = 1;
            const decimal commission = 10;
            const string commissionAsset = "BNB";
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, tradeId, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch);

            const decimal quantityOfLastFilledTrade = 1;

            using (var cts = new CancellationTokenSource())
            {
                var args = new AccountTradeUpdateEventArgs(time, cts.Token, order, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade);

                Assert.Equal(time, args.Time);
                Assert.Equal(order, args.Order);
                Assert.Equal(OrderExecutionType.Trade, args.OrderExecutionType);
                Assert.Equal(orderRejectedReason, args.OrderRejectedReason);
                Assert.Equal(newClientOrderId, args.NewClientOrderId);
                Assert.Equal(trade, args.Trade);
                Assert.Equal(quantityOfLastFilledTrade, args.QuantityOfLastFilledTrade);
            }
        }
    }
}
