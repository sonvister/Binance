using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IClientWebSocketFactory"/> implementation.
    /// </summary>
    internal class ClientWebSocketFactory : IClientWebSocketFactory
    {
        public IClientWebSocket CreateClientWebSocket()
        {
            return new ClientWebSocketAdapter();
        }

        private class ClientWebSocketAdapter : IClientWebSocket
        {
            public WebSocketState State => _clientWebSocket.State;

            private readonly ClientWebSocket _clientWebSocket;

            public ClientWebSocketAdapter()
            {
                _clientWebSocket = new ClientWebSocket();
                _clientWebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            }

            public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
                => _clientWebSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);

            public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
                => _clientWebSocket.ConnectAsync(uri, cancellationToken);

            public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
                => _clientWebSocket.ReceiveAsync(buffer, cancellationToken);

            public void Dispose()
                => _clientWebSocket.Dispose();
        }
    }
}
