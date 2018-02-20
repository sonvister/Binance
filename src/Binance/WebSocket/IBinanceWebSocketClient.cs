using Binance.Stream;

namespace Binance.WebSocket
{
    /// <summary>
    /// A Binance web socket [stream] client.
    /// </summary>
    public interface IBinanceWebSocketClient : IJsonStreamClient<IBinanceWebSocketStream>
    { }
}
