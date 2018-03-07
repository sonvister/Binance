using Binance.Client;

namespace Binance.WebSocket
{
    public interface IWebSocketPublisherClient : IJsonPublisherClient<IAutoWebSocketStreamPublisher>, IError
    { }
}
