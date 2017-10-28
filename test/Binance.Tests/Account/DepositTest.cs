using System;
using Xunit;

namespace Binance.Account.Tests
{
    public class DepositTest
    {
        [Fact]
        public void Throws()
        {
            var asset = Asset.BTC;
            var amount = 1.23m;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var status = DepositStatus.Success;

            Assert.Throws<ArgumentNullException>("asset", () => new Deposit(null, amount, timestamp, status));
            Assert.Throws<ArgumentException>("amount", () => new Deposit(asset, -1, timestamp, status));
            Assert.Throws<ArgumentException>("amount", () => new Deposit(asset, 0, timestamp, status));
            Assert.Throws<ArgumentException>("timestamp", () => new Deposit(asset, amount, -1, status));
            Assert.Throws<ArgumentException>("timestamp", () => new Deposit(asset, amount, 0, status));
        }

        [Fact]
        public void Properties()
        {
            var asset = Asset.BTC;
            var amount = 1.23m;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var status = DepositStatus.Success;

            var deposit = new Deposit(asset, amount, timestamp, status);

            Assert.Equal(asset, deposit.Asset);
            Assert.Equal(amount, deposit.Amount);
            Assert.Equal(timestamp, deposit.Timestamp);
            Assert.Equal(status, deposit.Status);
        }
    }
}
