using System;
using Binance.Account;
using Binance.Serialization;
using Xunit;

namespace Binance.Tests.Serialization
{
    public class AccountTradeSerializerTest
    {
        [Fact]
        public void Equality()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const long orderId = 54321;
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            var commissionAsset = Asset.BNB;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, id, orderId, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch);

            var serializer = new AccountTradeSerializer();

            var json = serializer.Serialize(trade);

            var other = serializer.Deserialize(json);

            Assert.True(trade.Equals(other));
        }
    }
}
