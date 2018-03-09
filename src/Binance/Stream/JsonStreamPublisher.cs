using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Binance.Client;
using Microsoft.Extensions.Logging;

namespace Binance.Stream
{
    public abstract class JsonStreamPublisher<TStream> : JsonProducer, IJsonStreamPublisher<TStream>
        where TStream : class, IJsonStream
    {
        #region Public Properties

        public TStream Stream { get; }

        public IEnumerable<string> PublishedStreams
        {
            get { lock (Sync) { return Subscribers.Keys.ToArray(); } }
        }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IDictionary<string, ICollection<IJsonSubscriber>> Subscribers;

        protected readonly object Sync = new object();

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The JSON stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonStreamPublisher(TStream stream, ILogger<JsonStreamPublisher<TStream>> logger = null)
            : base(logger)
        {
            Throw.IfNull(stream, nameof(stream));

            Stream = stream;

            Subscribers = new Dictionary<string, ICollection<IJsonSubscriber>>();

            Stream.Message += HandleMessage;
        }

        #endregion Constructors

        #region Public Methods

        public IJsonPublisher Subscribe(IJsonSubscriber subscriber, params string[] streamNames)
        {
            if (streamNames == null || !streamNames.Any())
            {
                throw new ArgumentException($"{GetType().Name}.{nameof(Subscribe)}: A a stream name must be specified.");
            }

            lock (Sync)
            {
                var streamsChanged = false;

                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding stream \"{streamName}\".  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        Subscribers[streamName] = new List<IJsonSubscriber>();

                        streamsChanged = true;
                    }

                    if (subscriber == null || Subscribers[streamName].Contains(subscriber))
                        continue;

                    Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding subscriber of stream \"{streamName}\".  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    Subscribers[streamName].Add(subscriber);
                }

                if (streamsChanged)
                    OnPublishedStreamsChanged();
            }

            return this;
        }

        public IJsonPublisher Unsubscribe(IJsonSubscriber subscriber, params string[] streamNames)
        {
            if (streamNames == null || !streamNames.Any())
            {
                Unsubscribe(subscriber);
                return this;
            }

            lock (Sync)
            {
                var streamsChanged = false;

                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    // NOTE: Allow unsubscribe regardless of IJsonSubscriber.SubscribedStreams to support unlink/link functionality.

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogError($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream \"{streamName}\".  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        throw new InvalidOperationException($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream ({streamName}).");
                    }

                    if (subscriber != null && Subscribers[streamName].Contains(subscriber))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: Removing callback for stream \"{streamName}\".  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        Subscribers[streamName].Remove(subscriber);
                    }

                    // Unsubscribe stream if there are no callbacks.
                    // ReSharper disable once InvertIf
                    if (!Subscribers[streamName].Any())
                    {
                        RemoveStream(streamName);
                        streamsChanged = true;
                    }
                }

                if (streamsChanged)
                    OnPublishedStreamsChanged();
            }

            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Handle all messages from JSON stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void HandleMessage(object sender, JsonMessageEventArgs args)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(args.Subject))
                {
                    Logger?.LogError($"{GetType().Name}: No message event subject.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    return;
                }

                // Ignore message events if not subscribed.
                // ReSharper disable once InconsistentlySynchronizedField
                if (!Subscribers.TryGetValue(args.Subject, out var subscribers))
                    return; // ignore.

                NotifySubscribers(subscribers?.ToArray(), args.Subject, args.Json);

                // Forward stream message event to Publisher Message event observers.
                OnMessage(args);
            }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Failed processing JSON message.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        /// <summary>
        /// Notify listeners, both <see cref="IJsonSubscriber"/> subscribers and message event handlers.
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="streamName"></param>
        /// <param name="json"></param>
        protected virtual void NotifySubscribers(IEnumerable<IJsonSubscriber> subscribers, string streamName, string json)
        {
            if (subscribers == null)
                return;

            foreach (var subscriber in subscribers)
            {
                try { subscriber.HandleMessage(streamName, json); }
                catch (Exception e)
                {
                    Logger?.LogWarning(e, $"{GetType().Name}.{nameof(NotifySubscribers)}: Unhandled {nameof(HandleMessage)} exception.");
                }
            }
        }

        protected virtual void OnPublishedStreamsChanged() { }

        #endregion Protected Methods

        #region Private Methods

        private void Unsubscribe(IJsonSubscriber observer)
        {
            if (observer == null)
            {
                UnsubscribeAll();
                return;
            }

            lock (Sync)
            {
                var streamsChanged = false;

                foreach (var streamAndSubscribers in Subscribers.ToArray())
                {
                    if (streamAndSubscribers.Value.Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: Removing observer of stream ({streamAndSubscribers.Key})  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        streamAndSubscribers.Value.Remove(observer);
                    }

                    // Unsubscribe stream if there are no callbacks.
                    // ReSharper disable once InvertIf
                    if (!streamAndSubscribers.Value.Any())
                    {
                        RemoveStream(streamAndSubscribers.Key);
                        streamsChanged = true;
                    }
                }

                if (streamsChanged)
                    OnPublishedStreamsChanged();
            }
        }

        private void UnsubscribeAll()
        {
            lock (Sync)
            {
                if (!Subscribers.Any())
                    return;

                Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Removing all streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                Subscribers.Clear();

                OnPublishedStreamsChanged();
            }
        }

        private void RemoveStream(string stream)
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(RemoveStream)}: Removing stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            Subscribers.Remove(stream);

            // NOTE: This is called after multiple removes are done.
            //OnPublishedStreamsChanged();
        }

        #endregion Private Methods
    }
}
