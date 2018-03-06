using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A trade web socket client.
    /// </summary>
    public interface ITradeWebSocketClient : IWebSocketPublisherClient, ITradeClient
    { }
}
