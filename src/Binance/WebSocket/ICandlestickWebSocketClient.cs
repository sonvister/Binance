using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// A candlestick web socket client.
    /// </summary>
    public interface ICandlestickWebSocketClient : IBinanceWebSocketClient, ICandlestickClient
    { }
}
