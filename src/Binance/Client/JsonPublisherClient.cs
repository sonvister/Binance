using System;
using System.Collections.Generic;
using System.Linq;
using Binance.Producer;
using Microsoft.Extensions.Logging;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="IJsonPublisherClient{TPublisher}"/> abstract base class.
    /// Coordinates client subscriptions with publisher subscriptions.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TPublisher"></typeparam>
    public abstract class JsonPublisherClient<TClient, TPublisher> : IJsonPublisherClient<TPublisher>
        where TClient : IJsonClient
        where TPublisher : IJsonPublisher
    {
        #region Public Properties

        public IEnumerable<string> SubscribedStreams => Client.SubscribedStreams;

        public TPublisher Publisher { get; }

        #endregion Public Properties

        #region Protected Fields

        protected TClient Client;

        protected ILogger<JsonPublisherClient<TClient, TPublisher>> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The JSON publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonPublisherClient(TClient client, TPublisher publisher, ILogger<JsonPublisherClient<TClient, TPublisher>> logger = null)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(publisher, nameof(publisher));

            Client = client;
            Publisher = publisher;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void HandleMessage(string stream, string json)
            => Client.HandleMessage(stream, json);

        public virtual IJsonSubscriber Unsubscribe()
        {
            // Unsubscribe this observer from all streams.
            Publisher.Unsubscribe(this);

            // Unsubscribe client from all streams.
            Client.Unsubscribe();

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual IJsonClient HandleSubscribe(Action subscribeAction)
        {
            // Get a snapshot of the current client subscribed streams.
            var streams = Client.SubscribedStreams.ToArray();

            // Invoke the subscribe action on the client.
            subscribeAction();

            // Get the streams not previously subscribed to by the client.
            streams = Client.SubscribedStreams.Except(streams).ToArray();

            // If there are any new streams subscribed to by the client.
            if (streams.Any())
            {
                // Subscribe the client to the stream(s).
                Publisher.Subscribe(this, streams);
            }

            return this;
        }

        protected virtual IJsonClient HandleUnsubscribe(Action unsubscribeAction)
        {
            // Get a snapshot of the current client subscribed streams.
            var streams = Client.SubscribedStreams.ToArray();

            // Invoke the unsubscribe action on the client.
            unsubscribeAction();

            // Get the streams no longer subscribed to by the client.
            streams = streams.Except(Client.SubscribedStreams).ToArray();

            // If there are any streams no longer subscribed to by the client.
            if (streams.Any())
            {
                // Unsubscribe the client from the stream(s).
                Publisher.Unsubscribe(this, streams);
            }

            return this;
        }

        #endregion Protected Methods
    }
}
