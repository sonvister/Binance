using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binance.Client
{
    /// <summary>
    /// An abstract <see cref="IJsonClient"/> base class.
    /// </summary>
    /// <typeparam name="TDefaultEventArgs"></typeparam>
    public abstract class JsonClient<TDefaultEventArgs> : IJsonClient
        where TDefaultEventArgs : EventArgs
    {
        #region Public Properties

        public virtual IEnumerable<string> ObservedStreams
        {
            get { lock (_sync) { return _subscribers.Keys.ToArray(); } }
        }

        #endregion Public Properties

        #region Protected Fields

        protected readonly ILogger<IJsonClient> Logger;

        #endregion Protected Fields

        #region Private Fields

        private readonly IDictionary<string, IList<Action<TDefaultEventArgs>>> _subscribers;

        private readonly object _sync = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        protected JsonClient(ILogger<IJsonClient> logger = null)
        {
            Logger = logger;

            _subscribers = new Dictionary<string, IList<Action<TDefaultEventArgs>>>();
        }

        #endregion Constructors

        #region Public Methods

        public virtual IJsonClient Unsubscribe()
        {
            lock (_sync) { _subscribers.Clear(); }

            return this;
        }

        public virtual async Task HandleMessageAsync(string stream, string json, CancellationToken token)
        {
            try
            {
                // ReSharper disable once InconsistentlySynchronizedField
                _subscribers.TryGetValue(stream, out var callbacks);

                await HandleMessageAsync(callbacks, stream, json, token)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(JsonClient<TDefaultEventArgs>)}.{nameof(HandleMessageAsync)}: Unhandled exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract Task HandleMessageAsync(IEnumerable<Action<TDefaultEventArgs>> callbacks, string stream, string json, CancellationToken token = default);

        protected void SubscribeStream(string stream, Action<TDefaultEventArgs> callback)
        {
            lock (_sync)
            {
                SubscribeStream(stream, callback, _subscribers);
            }
        }

        protected virtual void SubscribeStream<TEventArgs>(string stream, Action<TEventArgs> callback, IDictionary<string, IList<Action<TEventArgs>>> subscribers)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            if (!subscribers.ContainsKey(stream))
            {
                Logger?.LogDebug($"{nameof(JsonClient<TDefaultEventArgs>)}.{nameof(SubscribeStream)}: Adding stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                subscribers[stream] = new List<Action<TEventArgs>>();
            }

            // ReSharper disable once InvertIf
            if (callback != null && !subscribers[stream].Contains(callback))
            {
                Logger?.LogDebug($"{nameof(JsonClient<TDefaultEventArgs>)}.{nameof(SubscribeStream)}: Adding callback for stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                subscribers[stream].Add(callback);
            }
        }

        protected void UnsubscribeStream(string stream, Action<TDefaultEventArgs> callback)
        {
            lock (_sync)
            {
                UnsubscribeStream(stream, callback, _subscribers);
            }
        }

        protected virtual void UnsubscribeStream<TEventArgs>(string stream, Action<TEventArgs> callback, IDictionary<string, IList<Action<TEventArgs>>> subscribers)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            if (callback != null && subscribers.ContainsKey(stream))
            {
                if (subscribers[stream].Contains(callback))
                {
                    Logger?.LogDebug($"{nameof(JsonClient<TDefaultEventArgs>)}.{nameof(UnsubscribeStream)}: Removing callback for stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    subscribers[stream].Remove(callback);
                }
            }

            // ReSharper disable once InvertIf
            // ReSharper disable once ArrangeRedundantParentheses
            if (callback == null || (subscribers.ContainsKey(stream) && !subscribers[stream].Any()))
            {
                Logger?.LogDebug($"{nameof(JsonClient<TDefaultEventArgs>)}.{nameof(UnsubscribeStream)}: Removing stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                subscribers.Remove(stream);
            }
        }

        protected virtual void ReplaceStreamName(string oldStreamName, string newStreamName)
        {
            lock (_sync)
            {
                if (!_subscribers.TryGetValue(oldStreamName, out var callbacks))
                    return;

                _subscribers[newStreamName] = callbacks;
                _subscribers.Remove(oldStreamName);
            }
        }

        #endregion Protected Methods
    }
}
