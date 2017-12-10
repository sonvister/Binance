using System;
using Binance.Account;
using Xunit;

namespace Binance.Tests.Account
{
    public class WithdrawalTest
    {
        [Fact]
        public void Throws()
        {
            const string id = "1234567890";
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            const long timestamp = 1234567890;
            const WithdrawalStatus status = WithdrawalStatus.Completed;
            const string address = "0x12345678901234567890";

            Assert.Throws<ArgumentNullException>("id", () => new Withdrawal(null, asset, amount, timestamp, status, address));
            Assert.Throws<ArgumentNullException>("asset", () => new Withdrawal(id, null, amount, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(id, asset, -1, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(id, asset, 0, timestamp, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Withdrawal(id, asset, amount, -1, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Withdrawal(id, asset, amount, 0, status, address));
            Assert.Throws<ArgumentNullException>("address", () => new Withdrawal(id, asset, amount, timestamp, status, null));
            Assert.Throws<ArgumentNullException>("address", () => new Withdrawal(id, asset, amount, timestamp, status, string.Empty));
        }

        [Fact]
        public void Properties()
        {
            const string id = "1234567890";
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            const long timestamp = 1234567890;
            const WithdrawalStatus status = WithdrawalStatus.Completed;
            const string address = "0x12345678901234567890";
            const string txId = "21436587092143658709";

            var withdrawal = new Withdrawal(id, asset, amount, timestamp, status, address, txId);

            Assert.Equal(id, withdrawal.Id);
            Assert.Equal(asset, withdrawal.Asset);
            Assert.Equal(amount, withdrawal.Amount);
            Assert.Equal(timestamp, withdrawal.Timestamp);
            Assert.Equal(status, withdrawal.Status);
            Assert.Equal(address, withdrawal.Address);
            Assert.Equal(txId, withdrawal.TxId);
        }
    }
}
