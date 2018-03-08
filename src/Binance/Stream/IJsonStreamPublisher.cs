namespace Binance.Stream
{
    /// <summary>
    /// An <see cref="IJsonPublisher"/> using an <see cref="IJsonStream"/>.
    /// </summary>
    /// <typeparam name="TStream"></typeparam>
    public interface IJsonStreamPublisher<out TStream> : IJsonPublisher
        where TStream : IJsonStream
    {
        TStream Stream { get; }
    }
}
