using System;
using Binance.Account;
using Newtonsoft.Json;
using Xunit;

namespace Binance.Tests.Account
{
    public class DepositTest
    {
        [Fact]
        public void Throws()
        {
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const DepositStatus status = DepositStatus.Success;
            const string address = "0x12345678901234567890";

            Assert.Throws<ArgumentNullException>("asset", () => new Deposit(null, amount, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Deposit(asset, -1, timestamp, status, address));
            Assert.Throws<ArgumentException>("amount", () => new Deposit(asset, 0, timestamp, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Deposit(asset, amount, -1, status, address));
            Assert.Throws<ArgumentException>("timestamp", () => new Deposit(asset, amount, 0, status, address));
            Assert.Throws<ArgumentNullException>("address", () => new Deposit(asset, amount, timestamp, status, null));
            Assert.Throws<ArgumentNullException>("address", () => new Deposit(asset, amount, timestamp, status, string.Empty));
        }

        [Fact]
        public void Properties()
        {
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const DepositStatus status = DepositStatus.Success;
            const string address = "0x12345678901234567890";
            const string addressTag = "ABCDEF";
            const string txId = "21436587092143658709";

            var deposit = new Deposit(asset, amount, timestamp, status, address, addressTag, txId);

            Assert.Equal(asset, deposit.Asset);
            Assert.Equal(amount, deposit.Amount);
            Assert.Equal(timestamp, deposit.Timestamp);
            Assert.Equal(status, deposit.Status);
            Assert.Equal(address, deposit.Address);
            Assert.Equal(addressTag, deposit.AddressTag);
            Assert.Equal(txId, deposit.TxId);
        }

        [Fact]
        public void Serialization()
        {
            var asset = Asset.BTC;
            const decimal amount = 1.23m;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            const DepositStatus status = DepositStatus.Success;
            const string address = "0x12345678901234567890";
            const string addressTag = "ABCDEF";
            const string txId = "21436587092143658709";

            var deposit = new Deposit(asset, amount, timestamp, status, address, addressTag, txId);

            var json = JsonConvert.SerializeObject(deposit);

            deposit = JsonConvert.DeserializeObject<Deposit>(json);

            Assert.Equal(asset, deposit.Asset);
            Assert.Equal(amount, deposit.Amount);
            Assert.Equal(timestamp, deposit.Timestamp);
            Assert.Equal(status, deposit.Status);
            Assert.Equal(address, deposit.Address);
            Assert.Equal(addressTag, deposit.AddressTag);
            Assert.Equal(txId, deposit.TxId);
        }
    }
}
