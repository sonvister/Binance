using System;
using Moq;
using Xunit;

namespace Binance.Tests.Account.Orders
{
    public class LimitMakerOrderTest
    {
        [Fact]
        public void Throws()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new LimitMakerOrder(user);

            Assert.Throws<ArgumentException>("TimeInForce", () => clientOrder.TimeInForce = TimeInForce.IOC);
        }

        [Fact]
        public void Properties()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            var clientOrder = new LimitMakerOrder(user);

            Assert.Equal(OrderType.LimitMaker, clientOrder.Type);
            Assert.Equal(TimeInForce.GTC, clientOrder.TimeInForce);
        }
    }
}
