namespace Binance.Producer
{
    /// <summary>
    /// An <see cref="IJsonStream"/> that buffers data from an <see cref="IJsonProducer"/>.
    /// </summary>
    public interface IBufferedJsonStream<out TProvider> : IBufferedJsonProducer<TProvider>, IJsonStream
        where TProvider : IJsonProducer
    { }
}
