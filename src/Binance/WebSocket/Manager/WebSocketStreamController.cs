using System;
using System.Linq;
using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    public sealed class WebSocketStreamController : RetryTaskController, IWebSocketStreamController
    {
        /// <summary>
        /// Get the web socket stream.
        /// </summary>
        public IWebSocketStream WebSocket { get; }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="onError"></param>
        public WebSocketStreamController(IWebSocketStream webSocket, Action<Exception> onError = null)
            : base(tkn => webSocket.StreamAsync(tkn), onError)
        {
            Throw.IfNull(webSocket, nameof(webSocket));

            WebSocket = webSocket;
        }

        /// <summary>
        /// Begin streaming after verifying web socket is not streaming
        /// and has at least one subscription.
        /// </summary>
        public override void Begin()
        {
            if (!WebSocket.Client.IsStreaming && WebSocket.SubscribedStreams.Any())
            {
                base.Begin();
            }
        }
    }
}
