using Xunit;

namespace Binance.Tests.Account
{
    public class FillTest
    {
        [Fact]
        public void Properties()
        {
            const decimal price = 123.45m;
            const decimal quantity = 54.321m;
            const decimal commission = 0.9876m;
            var commissionAsset = Asset.BNB;
            const long tradeId = 987654321;

            var fill = new Fill(price, quantity, commission, commissionAsset, tradeId);

            Assert.Equal(price, fill.Price);
            Assert.Equal(quantity, fill.Quantity);
            Assert.Equal(commission, fill.Commission);
            Assert.Equal(commissionAsset, fill.CommissionAsset);
            Assert.Equal(tradeId, fill.TradeId);
        }
    }
}
