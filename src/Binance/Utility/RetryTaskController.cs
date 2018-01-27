using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class RetryTaskController : IDisposable
    {
        #region Public Properties

        public Task Task { get; private set; }

        public int RetryDelayMilliseconds { get; set; } = 5000;

        #endregion Public Properties

        #region Private Fields

        private CancellationTokenSource _cts;

        #endregion Private Fields

        #region Public Methods

        public void Begin(Func<CancellationToken, Task> action, Action<Exception> onError = null)
        {
            ThrowIfDisposed();

            if (_cts != null)
                throw new InvalidOperationException($"{nameof(RetryTaskController)} - Task already running, use {nameof(CancelAsync)} to abort.");

            _cts = new CancellationTokenSource();

            Task = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try { await action(_cts.Token); }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!_cts.IsCancellationRequested)
                        {
                            onError?.Invoke(e);
                            OnError(e);
                        }
                    }

                    if (!_cts.IsCancellationRequested)
                    {
                        await Task.Delay(RetryDelayMilliseconds, _cts.Token);
                    }
                }
            });
        }

        public async Task CancelAsync()
        {
            ThrowIfDisposed();

            if (_cts == null)
                throw new InvalidOperationException($"{nameof(RetryTaskController)} - Task is not running, use {nameof(Begin)} to start.");

            _cts.Cancel();

            await Task;

            _cts.Dispose();
            _cts = null;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnError(Exception e) { }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RetryTaskController));
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _cts?.Cancel();
                Task?.GetAwaiter().GetResult();
                _cts?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
