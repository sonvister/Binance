using Binance.Accounts;
using System;
using Xunit;

namespace Binance.Tests.Accounts
{
    public class AccountBalanceTest
    {
        [Fact]
        public void Throws()
        {
            string asset = Asset.BTC;
            decimal free = 0.123m;
            decimal locked = 0.456m;

            var balance = new AccountBalance(asset, 0, 0);

            Assert.Throws<ArgumentNullException>("asset", () => new AccountBalance(null, free, locked));
            Assert.Throws<ArgumentException>("free", () => new AccountBalance(asset, -1, locked));
            Assert.Throws<ArgumentException>("locked", () => new AccountBalance(asset, free, -1));
        }

        [Fact]
        public void Properties()
        {
            string asset = Asset.BTC;
            decimal free = 0.123m;
            decimal locked = 0.456m;

            var balance = new AccountBalance(asset, free, locked);

            Assert.Equal(asset, balance.Asset);
            Assert.Equal(free, balance.Free);
            Assert.Equal(locked, balance.Locked);
        }
    }
}
