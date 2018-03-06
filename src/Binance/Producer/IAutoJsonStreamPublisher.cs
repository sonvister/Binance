using Binance.Utility;

namespace Binance.Producer
{
    /// <summary>
    /// An <see cref="IJsonStreamPublisher{TStream}"/> with automatic stream control.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IAutoJsonStreamPublisher<out TStream> : IJsonStreamPublisher<TStream>
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the JSON stream controller.
        /// </summary>
        IJsonStreamController<TStream> Controller { get; }
    }
}
