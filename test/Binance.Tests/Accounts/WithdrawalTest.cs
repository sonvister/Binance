using Binance.Accounts;
using System;
using Xunit;

namespace Binance.Tests.Accounts
{
    public class WithdrawalTest
    {
        [Fact]
        public void Throws()
        {
            var asset = Asset.BTC;
            var amount = 1.23m;
            var timestamp = 1234567890;
            var status = WithdrawalStatus.Completed;
            var address = "0x12345678901234567890";

            Assert.Throws<ArgumentNullException>("asset", () => new Withdrawal(null, amount, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(asset, -1, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(asset, 0, timestamp, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Withdrawal(asset, amount, -1, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Withdrawal(asset, amount, 0, status, address));
        }

        [Fact]
        public void Properties()
        {
            var asset = Asset.BTC;
            var amount = 1.23m;
            var timestamp = 1234567890;
            var status = WithdrawalStatus.Completed;
            var address = "0x12345678901234567890";
            var txId = "21436587092143658709";

            var withdrawal = new Withdrawal(asset, amount, timestamp, status, address, txId);

            Assert.Equal(asset, withdrawal.Asset);
            Assert.Equal(amount, withdrawal.Amount);
            Assert.Equal(timestamp, withdrawal.Timestamp);
            Assert.Equal(status, withdrawal.Status);
            Assert.Equal(address, withdrawal.Address);
            Assert.Equal(txId, withdrawal.TxId);
        }
    }
}
