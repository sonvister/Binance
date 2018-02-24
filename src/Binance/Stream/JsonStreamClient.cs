using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Microsoft.Extensions.Logging;

namespace Binance.Stream
{
    /// <summary>
    /// The default <see cref="IJsonStreamClient{TStream}"/> abstract base class.
    /// Coordinates client subscriptions with stream subscriptions.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TStream"></typeparam>
    public abstract class JsonStreamClient<TClient, TStream> : IJsonStreamClient<TStream>
        where TClient : IJsonClient
        where TStream : IJsonStream
    {
        #region Public Properties

        public IEnumerable<string> ObservedStreams => Client.ObservedStreams;

        public TStream Stream { get; }

        #endregion Public Properties

        #region Protected Fields

        protected TClient Client;

        protected ILogger<JsonStreamClient<TClient, TStream>> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The JSON stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonStreamClient(TClient client, TStream stream, ILogger<JsonStreamClient<TClient, TStream>> logger = null)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(stream, nameof(stream));

            Client = client;
            Stream = stream;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public virtual Task HandleMessageAsync(string stream, string json, CancellationToken token = default)
            => Client.HandleMessageAsync(stream, json, token);

        public virtual IJsonClient Unsubscribe()
        {
            // Unsubscribe this observer from all streams.
            Stream.Unsubscribe(this);

            // Unsubscribe client from all streams.
            Client.Unsubscribe();

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual IJsonClient HandleSubscribe(Action subscribeAction)
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
                // Subscribe the client to the stream(s).
                Stream.Subscribe(this, streams);
            }

            return this;
        }

        protected virtual IJsonClient HandleUnsubscribe(Action unsubscribeAction)
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
                // Unsubscribe the client from the stream(s).
                Stream.Unsubscribe(this, streams);
            }

            return this;
        }

        #endregion Protected Methods
    }
}
