using System;
using Xunit;

namespace Binance.Tests
{
    public class ErrorEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("exception", () => new ErrorEventArgs(null));
        }

        [Fact]
        public void Properties()
        {
            var exception = new Exception();

            var args = new ErrorEventArgs(exception);

            Assert.Equal(exception, args.Exception);
        }
    }
}
