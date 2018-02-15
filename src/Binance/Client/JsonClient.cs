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
    /// <typeparam name="TEventArgs"></typeparam>
    public abstract class JsonClient<TEventArgs> : IJsonClient
        where TEventArgs : EventArgs
    {
        #region Public Properties

        public IEnumerable<string> ObservedStreams
        {
            get { lock (_sync) { return _subscribers.Keys.ToArray(); } }
        }

        #endregion Public Properties

        #region Protected Fields

        protected readonly ILogger<IJsonClient> Logger;

        #endregion Protected Fields

        #region Private Fields

        private readonly IDictionary<string, IList<Action<TEventArgs>>> _subscribers;

        private readonly object _sync = new object();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public JsonClient(ILogger<IJsonClient> logger = null)
        {
            Logger = logger;

            _subscribers = new Dictionary<string, IList<Action<TEventArgs>>>();
        }

        #endregion Constructors

        #region Public Methods

        public virtual void Unsubscribe()
        {
            lock (_sync) { _subscribers.Clear(); }
        }

        public virtual async Task HandleMessageAsync(string stream, string json, CancellationToken token)
        {
            try
            {
                if (!_subscribers.ContainsKey(stream))
                {
                    Logger?.LogDebug($"{nameof(JsonClient<TEventArgs>)}.{nameof(HandleMessageAsync)} - Ignoring event for non-subscribed stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    return; // ignore.
                }

                _subscribers.TryGetValue(stream, out var callbacks);

                await HandleMessageAsync(callbacks, stream, json, token)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(JsonClient<TEventArgs>)}.{nameof(HandleMessageAsync)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract Task HandleMessageAsync(IEnumerable<Action<TEventArgs>> callbacks, string stream, string json, CancellationToken token = default);

        protected virtual void SubscribeStream(string stream, Action<TEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            lock (_sync)
            {
                if (!_subscribers.ContainsKey(stream))
                {
                    Logger?.LogDebug($"{nameof(JsonClient<TEventArgs>)}.{nameof(SubscribeStream)}: Adding stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    _subscribers[stream] = new List<Action<TEventArgs>>();
                }

                // ReSharper disable once InvertIf
                if (callback != null && !_subscribers[stream].Contains(callback))
                {
                    Logger?.LogDebug($"{nameof(JsonClient<TEventArgs>)}.{nameof(SubscribeStream)}: Adding callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    _subscribers[stream].Add(callback);
                }
            }
        }

        protected virtual void UnsubscribeStream(string stream, Action<TEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(stream, nameof(stream));

            lock (_sync)
            {
                if (callback != null && _subscribers.ContainsKey(stream))
                {
                    if (_subscribers[stream].Contains(callback))
                    {
                        Logger?.LogDebug($"{nameof(JsonClient<TEventArgs>)}.{nameof(UnsubscribeStream)}: Removing callback for stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        _subscribers[stream].Remove(callback);
                    }
                }

                // ReSharper disable once InvertIf
                if (callback == null || _subscribers.ContainsKey(stream) && !_subscribers[stream].Any())
                {
                    Logger?.LogDebug($"{nameof(JsonClient<TEventArgs>)}.{nameof(UnsubscribeStream)}: Removing stream (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    _subscribers.Remove(stream);
                }
            }
        }

        #endregion Protected Methods
    }
}
