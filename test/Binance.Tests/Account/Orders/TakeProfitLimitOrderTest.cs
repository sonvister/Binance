using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class TakeProfitLimitOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new TakeProfitLimitOrder(user);

            Assert.Equal(OrderType.TakeProfitLimit, clientOrder.Type);
            Assert.Equal(0, clientOrder.StopPrice);
        }
    }
}
