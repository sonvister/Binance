using System;
using Binance.Account;
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
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            const string commissionAsset = "BNB";
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            Assert.Throws<ArgumentNullException>("symbol", () => new AccountTrade(null, id, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("id", () => new AccountTrade(symbol, -1, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("price", () => new AccountTrade(symbol, id, -1, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("quantity", () => new AccountTrade(symbol, id, price, -1, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("quantity", () => new AccountTrade(symbol, id, price, 0, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));

            Assert.Throws<ArgumentException>("commission", () => new AccountTrade(symbol, id, price, quantity, -1, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));
            Assert.Throws<ArgumentException>("commission", () => new AccountTrade(symbol, id, price, quantity, 101, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch));
        }

        [Fact]
        public void Properties()
        {
            var symbol = Symbol.BTC_USDT;
            const long id = 12345;
            const decimal price = 5000;
            const decimal quantity = 1;
            const decimal commission = 10;
            const string commissionAsset = "BNB";
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const bool isBuyer = true;
            const bool isMaker = true;
            const bool isBestPriceMatch = true;

            var trade = new AccountTrade(symbol, id, price, quantity, commission, commissionAsset, timestamp, isBuyer, isMaker, isBestPriceMatch);

            Assert.Equal(symbol, trade.Symbol);
            Assert.Equal(id, trade.Id);
            Assert.Equal(price, trade.Price);
            Assert.Equal(quantity, trade.Quantity);
            Assert.Equal(commission, trade.Commission);
            Assert.Equal(commissionAsset, trade.CommissionAsset);
            Assert.Equal(timestamp, trade.Timestamp);
            Assert.Equal(isBuyer, trade.IsBuyer);
            Assert.Equal(isMaker, trade.IsMaker);
            Assert.Equal(isBestPriceMatch, trade.IsBestPriceMatch);
        }
    }
}
