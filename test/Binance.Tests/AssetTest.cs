using System;
using Xunit;

namespace Binance.Tests
{
    public class AssetTest
    {
        [Fact]
        public void Throws()
        {
            const int precision = 8;

            Assert.Throws<ArgumentNullException>("symbol", () => new Asset(null, precision));
            Assert.Throws<ArgumentNullException>("symbol", () => new Asset(string.Empty, precision));
        }

        [Fact]
        public void ImplicitOperators()
        {
            var btc1 = Asset.BTC;
            var btc2 = new Asset(btc1.Symbol, btc1.Precision);
            var btc3 = Asset.BCH;

            Assert.True(btc1 == btc2);
            Assert.True(btc1 != btc3);

            Assert.True(btc1 == btc1.Symbol);
            Assert.True(btc1 != btc3.Symbol);
        }

        [Fact]
        public void Properties()
        {
            const string symbol = "BTC";
            const int precision = 8;

            var btc = new Asset(symbol, precision);

            Assert.Equal(symbol, btc.Symbol);
            Assert.Equal(precision, btc.Precision);
        }

        [Fact]
        public void IsValid()
        {
            var validAsset = Asset.BTC;
            var invalidAsset = new Asset("...", 0);

            Assert.True(Asset.IsValid(validAsset));
            Assert.False(Asset.IsValid(invalidAsset));
        }

        [Fact]
        public void IsAmountValid()
        {
            var asset = Asset.BTC;

            Assert.Equal(8, asset.Precision);

            Assert.True(asset.IsAmountValid(100.000000000000m));
            Assert.True(asset.IsAmountValid(010.000000000000m));
            Assert.True(asset.IsAmountValid(001.000000000000m));
            Assert.True(asset.IsAmountValid(000.100000000000m));
            Assert.True(asset.IsAmountValid(000.010000000000m));
            Assert.True(asset.IsAmountValid(000.001000000000m));
            Assert.True(asset.IsAmountValid(000.000100000000m));
            Assert.True(asset.IsAmountValid(000.000010000000m));
            Assert.True(asset.IsAmountValid(000.000001000000m));
            Assert.True(asset.IsAmountValid(000.000000100000m));
            Assert.True(asset.IsAmountValid(000.000000010000m));

            Assert.False(asset.IsAmountValid(000.000000001000m));
            Assert.False(asset.IsAmountValid(000.000000000100m));
            Assert.False(asset.IsAmountValid(000.000000000010m));
            Assert.False(asset.IsAmountValid(000.000000000001m));

            Assert.True(asset.IsAmountValid(000.000000000000m));

            Assert.False(asset.IsAmountValid(-000.000000010000m));
        }

        [Fact]
        public void ValidateAmount()
        {
            var asset = Asset.BTC;

            Assert.Equal(8, asset.Precision);

            asset.ValidateAmount(100.000000000000m);
            asset.ValidateAmount(010.000000000000m);
            asset.ValidateAmount(001.000000000000m);
            asset.ValidateAmount(000.100000000000m);
            asset.ValidateAmount(000.010000000000m);
            asset.ValidateAmount(000.001000000000m);
            asset.ValidateAmount(000.000100000000m);
            asset.ValidateAmount(000.000010000000m);
            asset.ValidateAmount(000.000001000000m);
            asset.ValidateAmount(000.000000100000m);
            asset.ValidateAmount(000.000000010000m);

            Assert.Throws<ArgumentOutOfRangeException>("amount", () => asset.ValidateAmount(000.000000001000m));
            Assert.Throws<ArgumentOutOfRangeException>("amount", () => asset.ValidateAmount(000.000000000100m));
            Assert.Throws<ArgumentOutOfRangeException>("amount", () => asset.ValidateAmount(000.000000000010m));
            Assert.Throws<ArgumentOutOfRangeException>("amount", () => asset.ValidateAmount(000.000000000001m));

            asset.ValidateAmount(000.000000000000m);

            Assert.Throws<ArgumentOutOfRangeException>("amount", () => asset.ValidateAmount(-000.000000010000m));
        }
    }
}
