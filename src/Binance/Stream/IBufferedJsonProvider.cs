namespace Binance.Stream
{
    /// <summary>
    /// A JSON provider that buffers data from another JSON provider.
    /// </summary>
    public interface IBufferedJsonProvider<T> : IJsonProvider
    {
        /// <summary>
        /// Get the JSON provider.
        /// </summary>
        T JsonProvider { get; }
    }
}
