using Binance.Accounts;
using Binance.Api.WebSocket.Events;
using Binance.Orders;
using System;
using Xunit;

namespace Binance.Tests.Api.WebSocket.Events
{
    public class TradeUpdateEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp);

            var orderRejectedReason = OrderRejectedReason.None;
            var newClientOrderId = "new-test-order";

            long tradeId = 12345;
            decimal quantity = 1;
            decimal commission = 10;
            string commissionAsset = "BNB";
            bool isBuyer = true;
            bool isMaker = true;
            bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, tradeId, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch);

            decimal quantityOfLastFilledTrade = 1;

            Assert.Throws<ArgumentException>("timestamp", () => new TradeUpdateEventArgs(-1, order, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade));
            Assert.Throws<ArgumentException>("timestamp", () => new TradeUpdateEventArgs(0, order, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade));
            Assert.Throws<ArgumentNullException>("order", () => new TradeUpdateEventArgs(timestamp, null, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade));
        }

        [Fact]
        public void Properties()
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

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

            var order = new Order(user, symbol, id, clientOrderId, price, originalQuantity, executedQuantity, status, timeInForce, orderType, orderSide, stopPrice, icebergQuantity, timestamp);

            var orderRejectedReason = OrderRejectedReason.None;
            var newClientOrderId = "new-test-order";

            long tradeId = 12345;
            decimal quantity = 1;
            decimal commission = 10;
            string commissionAsset = "BNB";
            bool isBuyer = true;
            bool isMaker = true;
            bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, tradeId, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch);

            decimal quantityOfLastFilledTrade = 1;

            var args = new TradeUpdateEventArgs(timestamp, order, orderRejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade);

            Assert.Equal(timestamp, args.Timestamp);
            Assert.Equal(order, args.Order);
            Assert.Equal(OrderExecutionType.Trade, args.OrderExecutionType);
            Assert.Equal(orderRejectedReason, args.OrderRejectedReason);
            Assert.Equal(newClientOrderId, args.NewClientOrderId);
            Assert.Equal(trade, args.Trade);
            Assert.Equal(quantityOfLastFilledTrade, args.QuantityOfLastFilledTrade);
        }
    }
}
