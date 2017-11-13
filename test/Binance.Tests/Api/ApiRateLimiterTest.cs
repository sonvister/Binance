using Binance.Api;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Binance.Tests.Api
{
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

            var rateLimiter = new ApiRateLimiter()
            {
                IsEnabled = enabled
            };

            Assert.Equal(enabled, rateLimiter.IsEnabled);
        }

        [Fact]
        public async Task RateLimit()
        {
            const int count = 100;
            var duration = TimeSpan.FromSeconds(1);

            const int burstCount = 75;
            var burstDuration = TimeSpan.FromSeconds(0.5);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= duration.TotalMilliseconds - 60);
            Assert.False(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + 60);
        }

        [Fact]
        public async Task RateLimitBurst()
        {
            const int count = 100;
            var duration = TimeSpan.FromSeconds(1);

            const int burstCount = 75;
            var burstDuration = TimeSpan.FromSeconds(0.5);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < burstCount + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds >= burstDuration.TotalMilliseconds - 60);
            Assert.False(stopwatch.ElapsedMilliseconds > burstDuration.TotalMilliseconds + 60);
        }

        [Fact]
        public async Task RateLimitBurstTwice()
        {
            const int count = 100;
            var duration = TimeSpan.FromSeconds(1);

            const int burstCount = 75;
            var burstDuration = TimeSpan.FromSeconds(0.5);

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
            const int count = 100;
            var duration = TimeSpan.FromSeconds(1);

            const int burstCount = 75;
            var burstDuration = TimeSpan.FromSeconds(0.5);

            var rateLimiter = new ApiRateLimiter();

            rateLimiter.Configure(duration, count);
            rateLimiter.Configure(burstDuration, burstCount);

            var stopwatch = Stopwatch.StartNew();

            for (var i = 0; i < count + burstCount + 1; i++)
                await rateLimiter.DelayAsync();

            stopwatch.Stop();

            Assert.True(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + burstDuration.TotalMilliseconds - 60);
            Assert.False(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + burstDuration.TotalMilliseconds + 60);
        }
    }
}
