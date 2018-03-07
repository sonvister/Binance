using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Binance.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IBinanceWebSocketStream"/> implementation.
    /// </summary>
    public class BinanceWebSocketStream : WebSocketStream, IBinanceWebSocketStream
    {
        #region Public Constants

        /// <summary>
        /// Get the base URI.
        /// </summary>
        public static readonly string BaseUri = "wss://stream.binance.com:9443";

        #endregion Public Constants

        #region Public Properties

        public override Uri Uri
        {
            get => base.Uri;
            set
            {
                if (value != null &&!value.AbsoluteUri.StartsWith(BaseUri))
                    throw new ArgumentException($"{nameof(BinanceWebSocketStream)}: URI must start with {nameof(BaseUri)} ({BaseUri}).");

                Pause(); // pause streaming.

                base.Uri = value;

                if (base.Uri == null)
                    return;

                if (base.Uri.AbsoluteUri.Contains("/ws/"))
                {
                    try { _streamName = base.Uri.PathAndQuery.Substring(base.Uri.PathAndQuery.LastIndexOf('/') + 1); }
                    catch (Exception e)
                    {
                        throw new ArgumentException($"{nameof(BinanceWebSocketStream)}: Failed to get stream name from URI ({base.Uri.PathAndQuery}).", nameof(Uri), e);
                    }
                }
                else _streamName = null;

                Resume(); // resume streaming.
            }
        }

        public bool IsCombined => base.Uri != null && base.Uri.AbsoluteUri.Contains("/stream?streams=");

        #endregion Public Properties

        #region Private Fields

        private string _streamName;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default
        /// <see cref="IWebSocketClient"/>, but no logger.
        /// </summary>
        public BinanceWebSocketStream()
            : this(new DefaultWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="webSocketClient">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public BinanceWebSocketStream(IWebSocketClient webSocketClient, ILogger<BinanceWebSocketStream> logger = null)
            : base(webSocketClient, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public static Uri CreateUri(IJsonSubscriber subscriber)
            => CreateUri(subscriber.SubscribedStreams);

        public static Uri CreateUri(IEnumerable<string> streamNames)
            => CreateUri(streamNames?.ToArray());

        public static Uri CreateUri(params string[] streamNames)
        {
            if (streamNames == null || !streamNames.Any())
                return null;

            var distinctNames = streamNames.Distinct().ToArray();

            return distinctNames.Length == 1
                    ? new Uri($"{BaseUri}/ws/{distinctNames.Single()}")
                    : new Uri($"{BaseUri}/stream?streams={string.Join("/", distinctNames)}");
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void ProcessJson(string json)
        {
            try
            {
                string streamName = null;

                if (json.IsJsonObject())
                {
                    var jObject = JObject.Parse(json);

                    // Get stream name.
                    streamName = jObject["stream"]?.Value<string>();

                    if (streamName != null)
                    {
                        // Get JSON data.
                        var data = jObject["data"]?.ToString(Formatting.None);
                        if (data == null)
                        {
                            Logger?.LogError($"{nameof(BinanceWebSocketStream)}.{nameof(ProcessJson)}: No JSON 'data' in message.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            return; // ignore.
                        }

                        json = data;
                    }
                }

                OnMessage(json, streamName ?? _streamName ?? Uri.AbsoluteUri);
            }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}.{nameof(ProcessJson)}: Failed processing JSON message.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Protected Methods
    }
}
