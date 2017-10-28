using System;
using Xunit;

namespace Binance.Account.Tests
{
    public class AccountTradeTests
    {
        [Fact]
        public void Throws()
        {
            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            decimal commission = 10;
            string commissionAsset = "BNB";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bool isBuyer = true;
            bool isMaker = true;
            bool isBestPriceMatch = true;

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
            string symbol = Symbol.BTC_USDT;
            long id = 12345;
            decimal price = 5000;
            decimal quantity = 1;
            decimal commission = 10;
            string commissionAsset = "BNB";
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            bool isBuyer = true;
            bool isMaker = true;
            bool isBestPriceMatch = true;

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
