using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class MarketOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new MarketOrder(user);

            Assert.Equal(OrderType.Market, clientOrder.Type);
        }
    }
}
