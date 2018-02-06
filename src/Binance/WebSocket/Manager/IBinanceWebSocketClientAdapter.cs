using System.Threading.Tasks;

namespace Binance.WebSocket.Manager
{
    public interface IBinanceWebSocketClientAdapter
    {
        Task Task { get; }
    }
}
