using Binance.Manager;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A candlestick web socket client manager.
    /// </summary>
    public interface ICandlestickWebSocketClientManager : ICandlestickClientManager<IWebSocketStream>
    { }
}
