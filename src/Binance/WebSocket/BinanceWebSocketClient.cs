using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The Binance web socket client abstract base class.
    /// </summary>
    public abstract class BinanceWebSocketClient<TClient, TEventArgs> : IBinanceWebSocketClient
        where TClient : IJsonClient
        where TEventArgs : EventArgs
    {
        #region Public Events

        public event EventHandler<EventArgs> Open
        {
            add => Stream.WebSocket.Open += value;
            remove => Stream.WebSocket.Open -= value;
        }

        public event EventHandler<JsonMessageEventArgs> Message
        {
            add => Stream.WebSocket.Message += value;
            remove => Stream.WebSocket.Message -= value;
        }

        public event EventHandler<EventArgs> Close
        {
            add => Stream.WebSocket.Close += value;
            remove => Stream.WebSocket.Close -= value;
        }

        #endregion Public Events

        #region Public Properties

        public TClient Client { get; }

        public IBinanceWebSocketStream Stream { get; }

        public IEnumerable<string> ObservedStreams => Client.ObservedStreams;

        #endregion Public Properties

        #region Protected Fields

        protected ILogger<BinanceWebSocketClient<TClient, TEventArgs>> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BinanceWebSocketClient(TClient client, IBinanceWebSocketStream stream, ILogger<BinanceWebSocketClient<TClient, TEventArgs>> logger = null)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(stream, nameof(stream));

            Client = client;
            Stream = stream;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public Task HandleMessageAsync(string stream, string json, CancellationToken token = default)
            => Client.HandleMessageAsync(stream, json, token);

        public void Unsubscribe()
        {
            Stream.Unsubscribe(this);
            Client.Unsubscribe();
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void HandleSubscribe(Action subscribeAction)
        {
            // Get a snapshot of the current client subscribed streams.
            var streams = Client.ObservedStreams.ToArray();

            // Invoke the subscribe action on the client.
            subscribeAction();

            // Get the streams not previously subscribed to by the client.
            streams = Client.ObservedStreams.Except(streams).ToArray();

            // If there are any new streams subscribed to by the client.
            if (streams.Any())
            {
                // Automatically, subscribe the client to the streams.
                Stream.Subscribe(this, streams);
            }
        }

        protected virtual void HandleUnsubscribe(Action unsubscribeAction)
        {
            // Get a snapshot of the current client subscribed streams.
            var streams = Client.ObservedStreams.ToArray();

            // Invoke the unsubscribe action on the client.
            unsubscribeAction();

            // Get the streams no longer subscribed to by the client.
            streams = streams.Except(Client.ObservedStreams).ToArray();

            // If there are any streams no longer subscribed to by the client.
            if (streams.Any())
            {
                // Automatically, unsubscribe the client from the streams.
                Stream.Unsubscribe(this, streams);
            }
        }

        #endregion Protected Methods
    }
}
