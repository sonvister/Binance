using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Producer;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    public class WebSocketStream : BufferedJsonStream<IWebSocketClient>, IWebSocketStream
    {
        #region Public Properties

        public IWebSocketClient WebSocket => JsonProducer;

        public virtual Uri Uri
        {
            get => _uri;
            set
            {
                if (Equals(value, _uri))
                    return;

                if (IsStreaming && !IsStreamingPaused)
                    throw new InvalidOperationException($"Cancel streaming before setting a new {nameof(Uri)} value.");

                _uri = value;
            }
        }

        #endregion Public Properties

        #region Private Fields

        private Uri _uri;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="logger"></param>
        public WebSocketStream(IWebSocketClient webSocket, ILogger<WebSocketStream> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Constructors

        #region Protected Methods

        protected override async Task StreamActionAsync(CancellationToken token = default)
        {
            if (_uri == null)
                throw new InvalidOperationException($"{nameof(WebSocketStream)}: URI must be set (not null) before streaming.");

            Logger?.LogInformation($"{nameof(WebSocketStream)}.{nameof(StreamActionAsync)}: Begin streaming...{Environment.NewLine}\"{_uri.AbsoluteUri}\"");

            try
            {
                await JsonProducer.StreamAsync(_uri, token)
                    .ConfigureAwait(false);
            }
            finally
            {
                Logger?.LogInformation($"{nameof(WebSocketStream)}.{nameof(StreamActionAsync)}: End streaming...{Environment.NewLine}\"{_uri.AbsoluteUri}\"");
            }
        }

        protected override void ProcessJson(string json)
        {
            OnMessage(json, _uri.AbsoluteUri);
        }

        #endregion Protected Methods
    }
}
