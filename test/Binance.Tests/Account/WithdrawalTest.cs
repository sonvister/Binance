using System;
using Newtonsoft.Json;
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
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const WithdrawalStatus status = WithdrawalStatus.Completed;
            const string address = "0x12345678901234567890";

            Assert.Throws<ArgumentNullException>("id", () => new Withdrawal(null, asset, amount, time, status, address));
            Assert.Throws<ArgumentNullException>("asset", () => new Withdrawal(id, null, amount, time, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(id, asset, -1, time, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Withdrawal(id, asset, 0, time, status, address));
            Assert.Throws<ArgumentNullException>("address", () => new Withdrawal(id, asset, amount, time, status, null));
            Assert.Throws<ArgumentNullException>("address", () => new Withdrawal(id, asset, amount, time, status, string.Empty));
        }

        [Fact]
        public void Properties()
        {
            const string id = "1234567890";
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const WithdrawalStatus status = WithdrawalStatus.Completed;
            const string address = "0x12345678901234567890";
            const string addressTag = "ABCDEF";
            const string txId = "21436587092143658709";

            var withdrawal = new Withdrawal(id, asset, amount, time, status, address, addressTag, txId);

            Assert.Equal(id, withdrawal.Id);
            Assert.Equal(asset, withdrawal.Asset);
            Assert.Equal(amount, withdrawal.Amount);
            Assert.Equal(time, withdrawal.Time);
            Assert.Equal(status, withdrawal.Status);
            Assert.Equal(address, withdrawal.Address);
            Assert.Equal(addressTag, withdrawal.AddressTag);
            Assert.Equal(txId, withdrawal.TxId);
        }

        [Fact]
        public void Serialization()
        {
            const string id = "1234567890";
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            var time = DateTimeOffset.FromUnixTimeMilliseconds(DateTime.UtcNow.ToTimestamp()).UtcDateTime;
            const WithdrawalStatus status = WithdrawalStatus.Completed;
            const string address = "0x12345678901234567890";
            const string addressTag = "ABCDEF";
            const string txId = "21436587092143658709";

            var withdrawal = new Withdrawal(id, asset, amount, time, status, address, addressTag, txId);

            var json = JsonConvert.SerializeObject(withdrawal);

            withdrawal = JsonConvert.DeserializeObject<Withdrawal>(json);

            Assert.Equal(id, withdrawal.Id);
            Assert.Equal(asset, withdrawal.Asset);
            Assert.Equal(amount, withdrawal.Amount);
            Assert.Equal(time, withdrawal.Time);
            Assert.Equal(status, withdrawal.Status);
            Assert.Equal(address, withdrawal.Address);
            Assert.Equal(addressTag, withdrawal.AddressTag);
            Assert.Equal(txId, withdrawal.TxId);
        }
    }
}
