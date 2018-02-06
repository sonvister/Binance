using System;

namespace Binance.WebSocket.Manager
{
    public class BinanceWebSocketClientManagerException : Exception
    {
        public IBinanceWebSocketClient Client { get; }

        public BinanceWebSocketClientManagerException(IBinanceWebSocketClient client)
            : this(client, null, null)
        { }

        public BinanceWebSocketClientManagerException(IBinanceWebSocketClient client, string message)
            : this(client, message, null)
        { }

        public BinanceWebSocketClientManagerException(IBinanceWebSocketClient client, Exception innerException)
            : this(client, null, innerException)
        { }

        public BinanceWebSocketClientManagerException(IBinanceWebSocketClient client, string message, Exception innerException)
            : base(message, innerException)
        {
            Throw.IfNull(client, nameof(client));

            Client = client;
        }
    }
}
