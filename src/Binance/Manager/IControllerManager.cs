using System;
using Binance.Stream;

namespace Binance.Manager
{
    public interface IControllerManager : IControllerManager<IJsonStream>
    { }

    public interface IControllerManager<out TStream> : IDisposable
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the task controller.
        /// </summary>
        IJsonStreamController<TStream> Controller { get; }

        /// <summary>
        /// Get the watchdog timer.
        /// </summary>
        //IWatchdogTimer WatchdogTimer { get; } // TODO
    }
}
