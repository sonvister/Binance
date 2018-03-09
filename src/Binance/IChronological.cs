using System;

namespace Binance
{
    public interface IChronological
    {
        /// <summary>
        /// Get the time (UTC).
        /// </summary>
        DateTime Time { get; }
    }
}
