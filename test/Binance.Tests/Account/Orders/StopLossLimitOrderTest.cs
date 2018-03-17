using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class StopLossLimitOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new StopLossLimitOrder(user);

            Assert.Equal(OrderType.StopLossLimit, clientOrder.Type);
            Assert.Equal(0, clientOrder.StopPrice);
        }
    }
}
