using System;

namespace Binance
{
    public sealed class TimeInterval : Tuple<long, long>
    {
        #region Public Properties

        public long StartTime => Item1;

        public long EndTime => Item2;

        #endregion Public Properties

        #region Implicit Operators

        public static implicit operator (long, long)(TimeInterval interval)
            => (interval.StartTime, interval.EndTime);

        public static implicit operator TimeInterval((long, long) interval)
            => new TimeInterval(interval.Item1, interval.Item2);

        #endregion Implicit Operators

        #region Constructors

        public TimeInterval(long startTime, long endTime)
            : base (startTime, endTime)
        { }

        #endregion Constructors
    }
}
