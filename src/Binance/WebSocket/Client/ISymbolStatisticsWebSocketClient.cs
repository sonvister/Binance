using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A symbol [24-hour] statistics web socket client.
    /// </summary>
    public interface ISymbolStatisticsWebSocketClient : IWebSocketPublisherClient, ISymbolStatisticsClient
    { }
}
