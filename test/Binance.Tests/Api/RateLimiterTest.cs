using Binance.Api;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Tests.Api
{
    public class RateLimiterTest
    {
        [Fact]
        public void ConfigureThrows()
        {
            var rateLimiter = new RateLimiter();

            Assert.Throws<ArgumentException>("count", () => rateLimiter.Configure(-1, TimeSpan.FromSeconds(1)));
            Assert.Throws<ArgumentException>("count", () => rateLimiter.Configure(0, TimeSpan.FromSeconds(1)));
        }

        [Fact]
        public void Configure()
        {
            const int count = 3;
            var duration = TimeSpan.FromSeconds(3);

            var enabled = !RateLimiter.EnabledDefault;

            var rateLimiter = new RateLimiter
            {
                IsEnabled = enabled
            };
            rateLimiter.Configure(count, duration);

            Assert.Equal(count, rateLimiter.Count);
            Assert.Equal(duration, rateLimiter.Duration);
            Assert.Equal(enabled, rateLimiter.IsEnabled);
        }

        [Fact]
        public async Task RateLimit()
        {
            const int count = 3;
            const int intervals = 2;

            var rateLimiter = new RateLimiter
            {
                IsEnabled = true
            };
            rateLimiter.Configure(count, TimeSpan.FromSeconds(1));

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count * intervals + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            // Assume elapsed milliseconds is within +/- 30 milliseconds of expected time (15 msec time resolution).
            // NOTE: Accounts for the error in the timestamp, ignoring Delay() and Stopwatch errors, and processing time.
            Assert.True(stopwatch.ElapsedMilliseconds > rateLimiter.Duration.TotalMilliseconds * intervals - 30);
            Assert.False(stopwatch.ElapsedMilliseconds > rateLimiter.Duration.TotalMilliseconds * intervals + 30);
        }
    }
}
