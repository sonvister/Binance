using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.WebSocket
{
    public interface IClientWebSocket : IDisposable
    {
        WebSocketState State { get; }

        Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken);

        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
    }
}
