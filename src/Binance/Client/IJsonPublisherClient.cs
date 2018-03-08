using Binance.Stream;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="IJsonPublisherClient{IJsonPublisher}"/> implementation.
    /// </summary>
    public interface IJsonPublisherClient : IJsonPublisherClient<IJsonPublisher>
    { }

    /// <summary>
    /// A <see cref="IJsonClient"/> with <see cref="IJsonPublisher"/>.
    /// </summary>
    /// <typeparam name="TPublisher"></typeparam>
    public interface IJsonPublisherClient<out TPublisher> : IJsonClient
        where TPublisher : IJsonPublisher
    {
        /// <summary>
        /// Get the JSON publisher.
        /// </summary>
        TPublisher Publisher { get; }
    }
}
