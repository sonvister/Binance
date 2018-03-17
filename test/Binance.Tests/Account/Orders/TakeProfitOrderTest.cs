using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class TakeProfitOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new TakeProfitOrder(user);

            Assert.Equal(OrderType.TakeProfit, clientOrder.Type);
            Assert.Equal(0, clientOrder.StopPrice);
        }
    }
}
