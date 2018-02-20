using Binance.Client;

namespace Binance.Stream
{
    /// <summary>
    /// A <see cref="IJsonClient"/> with <see cref="IJsonStream"/>.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IJsonStreamClient<out TStream> : IJsonClient
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the JSON stream.
        /// </summary>
        TStream Stream { get; }
    }
}
