using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        #region Public Events

        public event EventHandler<EventArgs> ProvidedStreamsChanged;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<string> ProvidedStreams
        {
            get { lock (Sync) { return StreamNames.ToArray(); } }
        }

        public bool IsStreaming { get; private set; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IDictionary<string, ICollection<IJsonStreamObserver>> Subscribers;

        protected readonly ICollection<string> StreamNames;

        protected readonly object Sync = new object();

        #endregion Protected Fields

        #region Private Fields

        private volatile bool _isStreamingPaused;

        private CancellationTokenSource _cts;

        private readonly Stopwatch _stopwatch;

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

            _stopwatch = Stopwatch.StartNew();
        }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(IJsonStreamObserver observer, params string[] streamNames)
        {
            if (streamNames == null || !streamNames.Any())
            {
                throw new ArgumentException($"{GetType().Name}.{nameof(Subscribe)}: A a stream name must be specified.");
            }

            lock (Sync)
            {
                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding stream ({streamName}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        Subscribers[streamName] = new List<IJsonStreamObserver>();
                        StreamNames.Add(streamName);

                        AbortStreaming();
                        OnProvidedStreamsChanged();
                    }

                    if (observer == null || Subscribers[streamName].Contains(observer))
                        continue;

                    Logger?.LogDebug($"{GetType().Name}.{nameof(Subscribe)}: Adding observer of stream ({streamName})  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    Subscribers[streamName].Add(observer);
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

            lock (Sync)
            {
                foreach (var streamName in streamNames)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: \"{streamName}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    // NOTE: Allow unsubscribe even if IJsonClient is still observing stream to support unlink functionality.

                    if (!Subscribers.ContainsKey(streamName))
                    {
                        Logger?.LogError($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream ({streamName}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        throw new InvalidOperationException($"{GetType().Name}.{nameof(Unsubscribe)}: Not subscribed to stream ({streamName}).");
                    }

                    if (observer != null && Subscribers[streamName].Contains(observer))
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(Unsubscribe)}: Removing callback for stream ({streamName}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
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

            Logger?.LogDebug($"{GetType().Name}.{nameof(StreamAsync)}: Streaming initiated.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            IsStreaming = true;

            try
            {
                using (token.Register(AbortStreaming))
                {
                    while (!token.IsCancellationRequested)
                    {
                        // Wait while paused or in transistion.
                        await Task.Delay(100, token)
                            .ConfigureAwait(false);

                        lock (Sync)
                        {
                            if (_isStreamingPaused || !StreamNames.Any() || _stopwatch.ElapsedMilliseconds < 500)
                                continue;

                            _cts = new CancellationTokenSource();

                            InitalizeBuffer(_cts.Token);
                        }

                        try
                        {
                            await StreamProviderAsync(_cts.Token)
                                .ConfigureAwait(false);
                        }
                        catch (OperationCanceledException) { /* ignore */ }
                        finally
                        {
                            lock (Sync)
                            {
                                FinalizeBuffer();

                                _cts.Dispose();
                                _cts = null;
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogWarning(e, $"{GetType().Name}.{nameof(StreamAsync)}: Unhandled exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
                throw;
            }
            finally
            {
                IsStreaming = false;

                _isStreamingPaused = false;

                Logger?.LogDebug($"{GetType().Name}.{nameof(StreamAsync)}: Streaming complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        public void Pause()
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(Pause)}: Pause streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            _isStreamingPaused = true;
            AbortStreaming();
        }

        public void Resume()
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(Resume)}: Resume streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            _isStreamingPaused = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract Task StreamProviderAsync(CancellationToken token = default);

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

            OnMessage(json, streamName);
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

            OnMessage(json, streamName);

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Provided streams changed event.
        /// </summary>
        protected void OnProvidedStreamsChanged()
        {
            try { ProvidedStreamsChanged?.Invoke(this, EventArgs.Empty); }
            catch (Exception) { /* ignore */ }
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

            lock (Sync)
            {
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
                    }
                }
            }
        }

        private void UnsubscribeAll()
        {
            lock (Sync)
            {
                if (!Subscribers.Any())
                    return;

                AbortStreaming();

                Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Removing all streams.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                Subscribers.Clear();
                StreamNames.Clear();

                OnProvidedStreamsChanged();
            }
        }

        private void RemoveStream(string stream)
        {
            Logger?.LogDebug($"{GetType().Name}.{nameof(RemoveStream)}: Removing stream ({stream}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            AbortStreaming();

            Subscribers.Remove(stream);
            StreamNames.Remove(stream);

            OnProvidedStreamsChanged();
        }

        private void AbortStreaming()
        {
            _stopwatch.Restart();

            try
            {
                lock (Sync)
                {
                    if (!_cts?.IsCancellationRequested ?? false)
                    {
                        _cts.Cancel();
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{GetType().Name}.{nameof(AbortStreaming)}: Ignored exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Private Methods
    }
}
