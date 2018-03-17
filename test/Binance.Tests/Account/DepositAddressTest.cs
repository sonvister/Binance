using System;
using Xunit;

namespace Binance.Tests.Account
{
    public class DepositAddressTest
    {
        [Fact]
        public void Throws()
        {
            var asset = Asset.BTC;
            const string address = "1234567890";

            Assert.Throws<ArgumentNullException>("asset", () => new DepositAddress(null, address));
            Assert.Throws<ArgumentNullException>("address", () => new DepositAddress(asset, null));
        }

        [Fact]
        public void Properties()
        {
            var asset = Asset.BTC;
            const string address = "1234567890";
            const string addressTag = "12341234";

            var depositAddress = new DepositAddress(asset, address, addressTag);

            Assert.Equal(asset, depositAddress.Asset);
            Assert.Equal(address, depositAddress.Address);
            Assert.Equal(addressTag, depositAddress.AddressTag);
        }
    }
}
