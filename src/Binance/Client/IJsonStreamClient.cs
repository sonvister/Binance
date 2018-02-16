using Binance.Stream;

namespace Binance.Client
{
    /// <summary>
    /// An <see cref="IJsonClient"/> with an <see cref="IJsonStream"/>.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IJsonStreamClient<TStream> : IJsonClient
        where TStream : IJsonStream
    {
        /// <summary>
        /// Get the JSON stream.
        /// </summary>
        TStream Stream { get; }
    }
}
