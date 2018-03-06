using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// An aggregate trade web socket client.
    /// </summary>
    public interface IAggregateTradeWebSocketClient : IWebSocketPublisherClient, IAggregateTradeClient
    { }
}
