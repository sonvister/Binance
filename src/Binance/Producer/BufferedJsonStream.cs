using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binance.Producer
{
    public abstract class BufferedJsonStream<TProvider> : BufferedJsonProducer<TProvider>, IBufferedJsonStream<TProvider>
        where TProvider : IJsonProducer
    {
        #region Public Properties

        public bool IsStreaming { get; private set; }

        #endregion Public Properties

        #region Protected Fields

        protected volatile bool IsStreamingPaused;

        #endregion Protected Fields

        #region Private Fields

        private CancellationTokenSource _cts;

        private readonly Stopwatch _stopwatch;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        protected BufferedJsonStream(TProvider jsonProvider, ILogger<BufferedJsonStream<TProvider>> logger = null)
            : base(jsonProvider, logger)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        #endregion Constructors

        #region Public Methods

        public async Task StreamAsync(CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (IsStreaming)
                throw new InvalidOperationException($"{GetType().Name}.{nameof(StreamAsync)}: Already streaming (Task is not completed).");

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

                        lock (_sync)
                        {
                            if (IsStreamingPaused || _stopwatch.ElapsedMilliseconds < 500)
                                continue;

                            _cts = new CancellationTokenSource();

                            InitalizeBuffer(_cts.Token);
                        }

                        try
                        {
                            await StreamActionAsync(_cts.Token)
                                .ConfigureAwait(false);
                        }
                        catch (OperationCanceledException) { /* ignore */ }
                        finally
                        {
                            lock (_sync)
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

                IsStreamingPaused = false;

                Logger?.LogDebug($"{GetType().Name}.{nameof(StreamAsync)}: Streaming complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected abstract Task StreamActionAsync(CancellationToken token = default);

        protected void Pause()
        {
            if (!IsStreaming)
                return;

            Logger?.LogDebug($"{GetType().Name}.{nameof(Pause)}: Pause streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            IsStreamingPaused = true;

            AbortStreaming();

            while (_cts != null) Thread.Sleep(10);
        }

        protected void Resume()
        {
            if (!IsStreaming)
                return;

            Logger?.LogDebug($"{GetType().Name}.{nameof(Resume)}: Resume streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            IsStreamingPaused = false;
        }

        #endregion Protected Methods

        #region Private Methods

        private void AbortStreaming()
        {
            _stopwatch.Restart();

            try
            {
                lock (_sync)
                {
                    if (!_cts?.IsCancellationRequested ?? false)
                    {
                        _cts.Cancel();
                    }
                }
            }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{GetType().Name}.{nameof(AbortStreaming)}: Exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }
        }

        #endregion Private Methods
    }
}
