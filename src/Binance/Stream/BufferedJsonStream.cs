using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binance.Stream
{
    /// <summary>
    /// An abstract buffered JSON provider stream that aborts the JSON
    /// provider when changes are made to the list of provided streams.
    /// </summary>
    public abstract class BufferedJsonStream<TProvider> : BufferedJsonProvider<TProvider>, IJsonStream
        where TProvider : IJsonProvider
    {
        #region Public Properties

        public IEnumerable<string> ProvidedStreams
        {
            get { lock (_sync) { return StreamNames.ToArray(); } }
        }

        public bool IsStreaming { get; private set; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IDictionary<string, ICollection<IJsonStreamObserver>> Subscribers;

        protected readonly ICollection<string> StreamNames;

        protected readonly object _sync = new object();

        #endregion Protected Fields

        #region Private Fields

        private volatile bool _isStreamingPaused;

        private CancellationTokenSource _cts;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonProvider">The JSON provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BufferedJsonStream(TProvider jsonProvider, ILogger<BufferedJsonStream<TProvider>> logger = null)
            : base(jsonProvider, logger)
        {
            Subscribers = new Dictionary<string, ICollection<IJsonStreamObserver>>();

            StreamNames = new List<string>();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(IJsonStreamObserver observer, params string[] streamNames)
        {
            if (observer == null && (streamNames == null || !streamNames.Any()))
            {
                throw new ArgumentException($"{GetType().Name}.{nameof(Subscribe)}: An observer and/or a stream name must be specified.");
            }

            if (streamNames == null || !streamNames.Any())
            {
                SubscribeAll(observer);
                return;
            }

            lock (_sync)
            {
                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding stream: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        Subscribers[streamName] = new List<IJsonStreamObserver>();
                        StreamNames.Add(streamName);

                        AbortWebSocket();
                    }

                    if (observer != null && !Subscribers[streamName].Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding observer of stream: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        Subscribers[streamName].Add(observer);
                    }
                }
            }
        }

        public void Unsubscribe(IJsonStreamObserver observer, params string[] streamNames)
        {
            if (streamNames == null || !streamNames.Any())
            {
                Unsubscribe(observer);
                return;
            }

            lock (_sync)
            {
                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    // NOTE: Allow unsubscribe even if IJsonClient is still observing stream to support unlink functionality.

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogError($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        throw new InvalidOperationException($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream: \"{streamName}\"");
                    }

                    if (observer != null && Subscribers[streamName].Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: Removing callback for stream: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        Subscribers[streamName].Remove(observer);
                    }

                    // Unsubscribe stream if there are no callbacks.
                    // ReSharper disable once InvertIf
                    if (!Subscribers[streamName].Any())
                    {
                        RemoveStream(streamName);
                    }
                }
            }
        }

        public async Task StreamAsync(CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (IsStreaming)
                throw new InvalidOperationException($"{GetType().Name}.{nameof(StreamAsync)}: Already streaming (Task is not completed).");

            if (!Subscribers.Any())
                throw new InvalidOperationException($"{GetType().Name}.{nameof(StreamAsync)}: Not subscribed to any streams.");

            IsStreaming = true;

            try
            {
                InitalizeBuffer(token);

                token.Register(AbortWebSocket);

                while (!token.IsCancellationRequested)
                {
                    // Wait while paused or in transistion.
                    await Task.Delay(300)
                        .ConfigureAwait(false);

                    lock (_sync)
                    {
                        if (_isStreamingPaused || StreamNames.Count() == 0)
                            continue;

                        _cts = new CancellationTokenSource();
                    }

                    try
                    {
                        await StreamAsync(JsonProvider, _cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    finally
                    {
                        lock (_sync)
                        {
                            _cts.Dispose();
                            _cts = null;
                        }
                    }

                    token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(StreamAsync)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
                throw;
            }
            finally
            {
                IsStreaming = false;

                _isStreamingPaused = false;

                FinalizeBuffer();

                Logger?.LogDebug($"{GetType().Name}.{nameof(StreamAsync)}: Task complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        public void Pause()
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(Pause)}: Pause streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            _isStreamingPaused = true;
            AbortWebSocket();
        }

        public void Resume()
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(Resume)}: Resume streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            _isStreamingPaused = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract Task StreamAsync(IJsonProvider jsonProvider, CancellationToken token = default);

        /// <summary>
        /// Notify listeners, both <see cref="IJsonStreamObserver"/> subscribers and message event handlers.
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="streamName"></param>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task NotifyListenersAsync(IEnumerable<IJsonStreamObserver> subscribers, string streamName, string json, CancellationToken token = default)
        {
            if (subscribers != null)
            {
                foreach (var subscriber in subscribers)
                {
                    await subscriber.HandleMessageAsync(streamName, json, token)
                        .ConfigureAwait(false);
                }
            }

            RaiseMessageEvent(json, streamName);
        }

        /// <summary>
        /// Notify all listeners in parallel.
        /// 
        /// NOTE: If using this, callbacks and event handlers need to handle
        ///       multi-thread synchronization of access to any shared resources.
        /// </summary>
        /// <param name="subscribers"></param>
        /// <param name="streamName"></param>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected Task NotifyListenersInParallelAsync(IEnumerable<IJsonStreamObserver> subscribers, string streamName, string json, CancellationToken token = default)
        {
            var tasks = new List<Task>(subscribers?.Count() ?? 0 + 1);

            if (subscribers != null)
            {
                foreach (var subscriber in subscribers)
                {
                    tasks.Add(
                        Task.Run(() => subscriber.HandleMessageAsync(streamName, json, token),
                        token));
                }
            }

            RaiseMessageEvent(json, streamName);

            return Task.WhenAll(tasks);
        }

        #endregion Protected Methods

        #region Private Methods

        private void Unsubscribe(IJsonStreamObserver observer)
        {
            if (observer == null)
            {
                UnsubscribeAll();
                return;
            }

            lock (_sync)
            {
                foreach (var streamAndSubscribers in Subscribers.ToArray())
                {
                    if (streamAndSubscribers.Value.Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: Removing observer of stream: \"{streamAndSubscribers.Key}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        streamAndSubscribers.Value.Remove(observer);
                    }

                    // Unsubscribe stream if there are no callbacks.
                    // ReSharper disable once InvertIf
                    if (!streamAndSubscribers.Value.Any())
                    {
                        RemoveStream(streamAndSubscribers.Key);
                    }
                }
            }
        }

        private void SubscribeAll(IJsonStreamObserver observer)
        {
            lock (_sync)
            {
                if (!Subscribers.Any())
                    throw new InvalidOperationException($"{GetType().Name}.{nameof(SubscribeAll)}: Not subscribed to any streams.");

                foreach (var streamAndSubscribers in Subscribers.ToArray())
                {
                    if (!streamAndSubscribers.Value.Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(SubscribeAll)}: Adding observer of stream: \"{streamAndSubscribers.Key}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        streamAndSubscribers.Value.Add(observer);
                    }
                }
            }
        }

        private void UnsubscribeAll()
        {
            lock (_sync)
            {
                if (!Subscribers.Any())
                    return;

                Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Removing all streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                Subscribers.Clear();
                StreamNames.Clear();

                AbortWebSocket();
            }
        }

        private void RemoveStream(string stream)
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(RemoveStream)}: Removing stream: \"{stream}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            Subscribers.Remove(stream);
            StreamNames.Remove(stream);

            AbortWebSocket();
        }

        private void AbortWebSocket()
        {
            try
            {
                if (!_cts?.IsCancellationRequested ?? false)
                {
                    _cts.Cancel();
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}.{nameof(AbortWebSocket)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Private Methods
    }
}
