using System;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Account
{
    public class AccountBalanceTest
    {
        [Fact]
        public void Throws()
        {
            var asset = Asset.BTC;
            const decimal free = 0.123m;
            const decimal locked = 0.456m;

            Assert.Throws<ArgumentNullException>("asset", () => new AccountBalance(null, free, locked));
            Assert.Throws<ArgumentException>("free", () => new AccountBalance(asset, -1, locked));
            Assert.Throws<ArgumentException>("locked", () => new AccountBalance(asset, free, -1));
        }

        [Fact]
        public void Properties()
        {
            var asset = Asset.BTC;
            const decimal free = 0.123m;
            const decimal locked = 0.456m;

            var balance = new AccountBalance(asset, free, locked);

            Assert.Equal(asset, balance.Asset);
            Assert.Equal(free, balance.Free);
            Assert.Equal(locked, balance.Locked);
        }

        [Fact]
        public void Serialization()
        {
            var asset = Asset.BTC;
            const decimal free = 0.123m;
            const decimal locked = 0.456m;

            var balance = new AccountBalance(asset, free, locked);

            var json = JsonConvert.SerializeObject(balance);

            balance = JsonConvert.DeserializeObject<AccountBalance>(json);

            Assert.Equal(asset, balance.Asset);
            Assert.Equal(free, balance.Free);
            Assert.Equal(locked, balance.Locked);
        }
    }
}
