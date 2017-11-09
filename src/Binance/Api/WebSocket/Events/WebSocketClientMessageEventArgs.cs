using System;

namespace Binance.Api.WebSocket.Events
{
    public class WebSocketClientMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public WebSocketClientMessageEventArgs(string message)
        {
            Throw.IfNullOrWhiteSpace(message, nameof(message));

            Message = message;
        }
    }
}
