using System;
using Xunit;

namespace Binance.Tests
{
    public class JsonMessageEventArgsTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("json", () => new JsonMessageEventArgs(null, null));
            Assert.Throws<ArgumentNullException>("json", () => new JsonMessageEventArgs(string.Empty, null));
        }

        [Fact]
        public void Properties()
        {
            const string json = "{ }";
            const string subject = "unit-test";

            var args = new JsonMessageEventArgs(json, subject);

            Assert.Equal(json, args.Json);
            Assert.Equal(subject, args.Subject);
        }
    }
}
