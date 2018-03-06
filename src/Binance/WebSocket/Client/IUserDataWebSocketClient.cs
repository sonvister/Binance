using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A user data web socket client.
    /// </summary>
    public interface IUserDataWebSocketClient : IWebSocketPublisherClient, IUserDataClient
    { }
}
