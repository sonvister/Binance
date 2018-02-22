using System;
using Binance.Stream;
using Binance.Utility;

namespace Binance.Manager
{
    /// <summary>
    /// A facade for automatic management of a JSON stream controller.
    /// </summary>
    public interface IControllerManager : IControllerManager<IJsonStream>
    { }

    /// <summary>
    /// A facade for automatic management of a JSON stream controller.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IControllerManager<out TStream> : IDisposable
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the task controller.
        /// </summary>
        IJsonStreamController<TStream> Controller { get; }

        /// <summary>
        /// Get the watchdog timer.
        /// 
        /// NOTE: The <see cref="IWatchdogTimer"/> aborts streaming if data
        ///       has not been received for a configurable time interval.
        /// </summary>
        IWatchdogTimer Watchdog { get; }
    }
}
