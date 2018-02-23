using System;
using Xunit;

namespace Binance.Tests
{
    public class TimeIntervalTests
    {
        [Fact]
        public void ImplicitOperators()
        {
            var begin = DateTime.Now;
            var end = begin.AddHours(1);

            var interval = (TimeInterval)(begin.ToTimestamp(), end.ToTimestamp());

            Assert.Equal(begin.ToTimestamp(), interval.BeginTime.ToTimestamp());
            Assert.Equal(end.ToTimestamp(), interval.EndTime.ToTimestamp());
        }

        [Fact]
        public void Properties()
        {
            var begin = DateTime.Now;
            var end = begin.AddHours(1);

            var interval = new TimeInterval(begin, end);

            Assert.Equal(begin, interval.BeginTime);
            Assert.Equal(end, interval.EndTime);
            Assert.Equal(begin, interval.Time);
        }
    }
}
