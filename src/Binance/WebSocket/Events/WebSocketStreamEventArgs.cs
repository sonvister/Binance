using System;
using System.Threading;

namespace Binance.WebSocket.Events
{
    public class WebSocketStreamEventArgs : EventArgs
    {
        /// <summary>
        /// Get the event stream name.
        /// </summary>
        public string StreamName { get; }

        /// <summary>
        /// Get the event JSON data (raw payload).
        /// </summary>
        public string Json { get; }

        /// <summary>
        /// Get the cancellation token.
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="json"></param>
        /// <param name="token"></param>
        public WebSocketStreamEventArgs(string streamName, string json, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(streamName, nameof(streamName));
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            StreamName = streamName;
            Json = json;
            Token = token;
        }
    }
}
