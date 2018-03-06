using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A depth web socket client.
    /// </summary>
    public interface IDepthWebSocketClient : IWebSocketPublisherClient, IDepthClient
    { }
}
