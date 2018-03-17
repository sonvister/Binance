using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class LimitOrderTest
    {
        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new LimitOrder(user);

            Assert.Equal(OrderType.Limit, clientOrder.Type);
            Assert.Equal(0, clientOrder.Price);
            Assert.Equal(0, clientOrder.IcebergQuantity);
            Assert.Equal(TimeInForce.GTC, clientOrder.TimeInForce);
        }
    }
}
