using System;

namespace Binance
{
    public sealed class TimeInterval : IChronological
    {
        #region Public Properties

        public DateTime BeginTime { get; }

        public DateTime EndTime { get; }

        public DateTime Time => BeginTime;

        #endregion Public Properties

        #region Implicit Operators

        public static implicit operator (long, long)(TimeInterval interval)
            => (interval.BeginTime.ToTimestamp(), interval.EndTime.ToTimestamp());

        public static implicit operator TimeInterval((long, long) interval)
            => new TimeInterval(interval.Item1, interval.Item2);

        #endregion Implicit Operators

        #region Constructors

        public TimeInterval(long beginTime, long endTime)
            : this(beginTime.ToDateTime(), endTime.ToDateTime())
        { }

        public TimeInterval(DateTime beginTime, DateTime endTime)
        {
            BeginTime = beginTime;
            EndTime = endTime;
        }

        #endregion Constructors
    }
}
