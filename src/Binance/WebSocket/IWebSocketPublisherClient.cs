using Binance.Client;
using Binance.Producer;

namespace Binance.WebSocket
{
    public interface IWebSocketPublisherClient : IJsonPublisherClient<IAutoJsonStreamPublisher<IWebSocketStream>>, IError
    { }
}
