using Binance.Stream;

namespace Binance.Utility
{
    /// <summary>
    /// A JSON stream task controller.
    /// </summary>
    public interface IJsonStreamController : IJsonStreamController<IJsonStream>
    { }

    /// <summary>
    /// A JSON stream task controller.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IJsonStreamController<out TStream> : IRetryTaskController
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the JSON stream.
        /// </summary>
        TStream Stream { get; }

        /// <summary>
        /// Get the watchdog timer.
        /// </summary>
        IWatchdogTimer Watchdog { get; }
    }
}
