using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A candlestick web socket client.
    /// </summary>
    public interface ICandlestickWebSocketClient : IWebSocketPublisherClient, ICandlestickClient
    { }
}
