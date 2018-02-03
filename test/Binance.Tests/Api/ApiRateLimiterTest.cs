using Binance.Api;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Tests.Api
{
    [Collection("Rate Limiter Tests")]
    public class ApiRateLimiterTest
    {
        [Fact]
        public void ConfigureThrows()
        {
            var rateLimiter = new ApiRateLimiter();

            Assert.Throws<ArgumentException>("count", () => rateLimiter.Configure(TimeSpan.FromSeconds(1), -1));
        }

        [Fact]
        public void Properties()
        {
            var enabled = !ApiRateLimiter.EnabledDefault;

            var rateLimiter = new ApiRateLimiter
            {
                IsEnabled = enabled
            };

            Assert.Equal(enabled, rateLimiter.IsEnabled);
        }

        [Fact]
        public async Task RateLimit()
        {
            const int count = 25;
            var duration = TimeSpan.FromSeconds(3);

            const int burstCount = 10;
            var burstDuration = TimeSpan.FromSeconds(1);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= duration.TotalMilliseconds - 100);
            Assert.False(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + 100);
        }

        [Fact]
        public async Task RateLimitBurst()
        {
            const int count = 25;
            var duration = TimeSpan.FromSeconds(3);

            const int burstCount = 10;
            var burstDuration = TimeSpan.FromSeconds(1);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < burstCount + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= burstDuration.TotalMilliseconds - 100);
            Assert.False(stopwatch.ElapsedMilliseconds > burstDuration.TotalMilliseconds + 100);
        }

        [Fact]
        public async Task RateLimitBurstTwice()
        {
            const int count = 25;
            var duration = TimeSpan.FromSeconds(3);

            const int burstCount = 10;
            var burstDuration = TimeSpan.FromSeconds(1);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < burstCount * 2 + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= (burstDuration.TotalMilliseconds - 45) * 2 + 15);
            Assert.False(stopwatch.ElapsedMilliseconds > (burstDuration.TotalMilliseconds + 45) * 2 + 15);
        }

        [Fact]
        public async Task RateLimitPlusBurst()
        {
            const int count = 25;
            var duration = TimeSpan.FromSeconds(3);

            const int burstCount = 10;
            var burstDuration = TimeSpan.FromSeconds(1);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count + burstCount + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + burstDuration.TotalMilliseconds - 100);
            Assert.False(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + burstDuration.TotalMilliseconds + 100);
        }
    }
}
