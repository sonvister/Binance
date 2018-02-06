using System;

namespace Binance.WebSocket.Manager
{
    public class WebSocketManagerException : Exception
    {
        public IBinanceWebSocketClient Client { get; }

        public WebSocketManagerException(IBinanceWebSocketClient client)
            : this(client, null, null)
        { }

        public WebSocketManagerException(IBinanceWebSocketClient client, string message)
            : this(client, message, null)
        { }

        public WebSocketManagerException(IBinanceWebSocketClient client, Exception innerException)
            : this(client, null, innerException)
        { }

        public WebSocketManagerException(IBinanceWebSocketClient client, string message, Exception innerException)
            : base(message, innerException)
        {
            Throw.IfNull(client, nameof(client));

            Client = client;
        }
    }
}
