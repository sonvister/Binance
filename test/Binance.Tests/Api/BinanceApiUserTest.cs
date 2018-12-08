using System;
using Moq;
using Xunit;
using Binance.Api;

namespace Binance.Tests.Api
{
    public class BinanceApiUserTest
    {
        [Fact]
        public void Throws()
        {
            Assert.Throws<ArgumentNullException>("apiKey", () => new BinanceApiUser(null));
        }

        [Fact]
        public void Sign_Throws()
        {
            var apiKey = "api-key";

            var user = new BinanceApiUser(apiKey);

            Assert.Throws<ArgumentNullException>("totalParams", () => user.Sign(null));
            Assert.Throws<InvalidOperationException>(() => user.Sign("valid-string"));
        }

        [Fact]
        public void Properties()
        {
            var apiKey = "api-key";
            var rateLimiter = new Mock<IApiRateLimiter>().Object;

            var user = new BinanceApiUser(apiKey, null, rateLimiter);

            Assert.Equal(apiKey, user.ApiKey);
            Assert.Equal(rateLimiter, user.RateLimiter);
        }
    }
}
