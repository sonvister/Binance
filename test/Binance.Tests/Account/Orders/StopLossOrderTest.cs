using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class StopLossOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new StopLossOrder(user);

            Assert.Equal(OrderType.StopLoss, clientOrder.Type);
            Assert.Equal(0, clientOrder.StopPrice);
        }
    }
}
