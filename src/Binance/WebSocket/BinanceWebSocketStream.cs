using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default buffered <see cref="IBinanceWebSocketStream"/> implementation.
    /// </summary>
    public sealed class BinanceWebSocketStream : BufferedWebSocketStream, IBinanceWebSocketStream
    {
        #region Public Properties

        public bool IsCombined => ProvidedStreams.Count() > 1;

        #endregion Public Properties

        #region Private Constants

        private const string BaseUri = "wss://stream.binance.com:9443";

        #endregion Private Constants

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IWebSocketClient"/>,
        /// but no logger.
        /// </summary>
        public BinanceWebSocketStream()
            : this(new DefaultWebSocketClient())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket">The web socket client (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public BinanceWebSocketStream(IWebSocketClient webSocket, ILogger<BinanceWebSocketStream> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Constructors

        #region Protected Methods

        protected override async Task StreamAsync(IJsonProvider jsonProvider, CancellationToken token = default)
        {
            var streams = ProvidedStreams;

            // ReSharper disable once PossibleMultipleEnumeration
            var uri = streams.Count() == 1
                // ReSharper disable once PossibleMultipleEnumeration
                ? new Uri($"{BaseUri}/ws/{streams.Single()}")
                // ReSharper disable once PossibleMultipleEnumeration
                : new Uri($"{BaseUri}/stream?streams={string.Join("/", streams)}");

            Logger?.LogInformation($"{nameof(BinanceWebSocketStream)}.{nameof(StreamAsync)}: \"{uri.AbsoluteUri}\"");

            await WebSocket.StreamAsync(uri, token)
                .ConfigureAwait(false);
        }

        protected override async Task ProcessJsonAsync(string json, CancellationToken token = default)
        {
            try
            {
                string streamName = null;
                //IJsonStreamObserver[] subscribers;

                // NOTE: Avoid locking... allowing for eventual consistency of subscribers.
                //lock (_sync)
                //{
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
                                Logger?.LogError($"{nameof(BinanceWebSocketStream)}: No JSON 'data' in message: \"{json}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                                return; // ignore.
                            }

                            json = data;
                        }
                    }

                    if (streamName == null)
                    {
                        // Get stream name.
                        streamName = StreamNames.FirstOrDefault();
                        if (streamName == null)
                        {
                            Logger?.LogError($"{nameof(BinanceWebSocketStream)}: No subscribed streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            return; // ignore.
                        }
                    }

                    if (!Subscribers.TryGetValue(streamName, out var observers))
                        return; // ignore.

                    // Get subscribers.
                    var subscribers = observers?.ToArray();
                //}

                await NotifyListenersAsync(subscribers, streamName, json, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(BinanceWebSocketStream)}: Failed processing JSON message.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
            }
        }

        #endregion Protected Methods
    }
}
