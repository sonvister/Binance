using Binance.Stream;
using Binance.Utility;

namespace Binance.Manager
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
    }
}
