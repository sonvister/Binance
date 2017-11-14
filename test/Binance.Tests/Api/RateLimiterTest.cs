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
        public void CountThrows()
        {
            var rateLimiter = new RateLimiter();

            Assert.Throws<ArgumentException>(nameof(rateLimiter.Count), () => rateLimiter.Count = -1);
            Assert.Throws<ArgumentException>(nameof(rateLimiter.Count), () => rateLimiter.Count = 0);
        }

        [Fact]
        public void Properties()
        {
            const int count = 3;
            var duration = TimeSpan.FromSeconds(3);

            var rateLimiter = new RateLimiter
            {
                Count = count,
                Duration = duration
            };

            Assert.Equal(count, rateLimiter.Count);
            Assert.Equal(duration, rateLimiter.Duration);
        }

        [Fact]
        public async Task RateLimit()
        {
            const int count = 100;
            var duration = TimeSpan.FromSeconds(0.5);

            const int repetitions = 10;

            var rateLimiter = new RateLimiter
            {
                Count = count,
                Duration = duration
            };

            var stopwatch = new Stopwatch();

            for (var i = 0; i < repetitions; i++)
            {
                stopwatch.Restart();

                for (var j = 0; j < count + 1; j++)
                    await rateLimiter.DelayAsync();

                stopwatch.Stop();

                // Assume elapsed milliseconds is within +/- 45 msec of expected time (15 msec clock resolution).
                // NOTE: Accounts for error in two timestamps and Task.Delay() ignoring any Stopwatch errors.
                Assert.True(stopwatch.ElapsedMilliseconds >= duration.TotalMilliseconds - 45);
                Assert.False(stopwatch.ElapsedMilliseconds > duration.TotalMilliseconds + 45);
            }
        }
    }
}
