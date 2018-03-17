using System;
using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class ClientOrderTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("user", () => new TestClientOrder(null));
        }

        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new TestClientOrder(user);

            Assert.Equal(user, clientOrder.User);

            Assert.Null(clientOrder.Id);
            Assert.Null(clientOrder.Symbol);
            Assert.Null(clientOrder.Side);
            Assert.Equal(0, clientOrder.Quantity);
        }

        private class TestClientOrder : ClientOrder
        {
            public override OrderType Type => throw new NotImplementedException();

            public TestClientOrder(IBinanceApiUser user)
                : base(user)
            { }
        }
    }
}
