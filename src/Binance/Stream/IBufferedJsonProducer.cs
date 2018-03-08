namespace Binance.Stream
{
    /// <summary>
    /// An <see cref="IJsonProducer"/> that buffers JSON from
    /// another <see cref="IJsonProducer"/>.
    /// </summary>
    public interface IBufferedJsonProducer<out TProducer> : IJsonProducer
        where TProducer : IJsonProducer
    {
        /// <summary>
        /// Get the JSON producer.
        /// </summary>
        TProducer JsonProducer { get; }
    }
}
