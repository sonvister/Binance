using System;
using Binance.Account;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Account
{
    public class AccountTradeTests
    {
        [Fact]
        public void Throws()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const long orderId = 54321;
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            var commissionAsset = Asset.BNB;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            Assert.Throws<ArgumentNullException>("symbol", () => new AccountTrade(null, id, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("id", () => new AccountTrade(symbol, -1, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("orderId", () => new AccountTrade(symbol, id, -1, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("price", () => new AccountTrade(symbol, id, orderId, -1, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("quantity", () => new AccountTrade(symbol, id, orderId, price, -1, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("quantity", () => new AccountTrade(symbol, id, orderId, price, 0, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("commission", () => new AccountTrade(symbol, id, orderId, price, quantity, -1, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("commission", () => new AccountTrade(symbol, id, orderId, price, quantity, 10001, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const long orderId = 54321;
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            var commissionAsset = Asset.BNB;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, id, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch);

            Assert.Equal(symbol, trade.Symbol);
            Assert.Equal(id, trade.Id);
            Assert.Equal(orderId, trade.OrderId);
            Assert.Equal(price, trade.Price);
            Assert.Equal(quantity, trade.Quantity);
            Assert.Equal(commission, trade.Commission);
            Assert.Equal(commissionAsset, trade.CommissionAsset);
            Assert.Equal(time, trade.Time);
            Assert.Equal(isBuyer, trade.IsBuyer);
            Assert.Equal(isMaker, trade.IsMaker);
            Assert.Equal(isBestPriceMatch, trade.IsBestPriceMatch);
        }

        [Fact]
        public void Serialization()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const long orderId = 54321;
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            var commissionAsset = Asset.BNB;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, id, orderId, price, quantity, commission, commissionAsset, time, isBuyer, isMaker, isBestPriceMatch);

            var json = JsonConvert.SerializeObject(trade);

            trade = JsonConvert.DeserializeObject<AccountTrade>(json);

            Assert.Equal(symbol, trade.Symbol);
            Assert.Equal(id, trade.Id);
            Assert.Equal(orderId, trade.OrderId);
            Assert.Equal(price, trade.Price);
            Assert.Equal(quantity, trade.Quantity);
            Assert.Equal(commission, trade.Commission);
            Assert.Equal(commissionAsset, trade.CommissionAsset);
            Assert.Equal(time, trade.Time);
            Assert.Equal(isBuyer, trade.IsBuyer);
            Assert.Equal(isMaker, trade.IsMaker);
            Assert.Equal(isBestPriceMatch, trade.IsBestPriceMatch);
        }
    }
}
