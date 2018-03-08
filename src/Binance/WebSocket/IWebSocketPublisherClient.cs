using Binance.Client;
using Binance.Stream;

namespace Binance.WebSocket
{
    public interface IWebSocketPublisherClient : IJsonPublisherClient<IAutoJsonStreamPublisher<IWebSocketStream>>, IError
    { }
}
