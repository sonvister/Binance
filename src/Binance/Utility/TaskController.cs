using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class TaskController : IDisposable
    {
        #region Public Properties

        public Task Task { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private CancellationTokenSource _cts;

        #endregion Private Fields

        #region Public Methods

        public virtual void Begin(Func<CancellationToken, Task> action, Action<Exception> onError = null)
        {
            ThrowIfDisposed();

            if (_cts != null)
                throw new InvalidOperationException($"{nameof(TaskController)} - Task already running, use {nameof(CancelAsync)} to abort.");

            _cts = new CancellationTokenSource();

            Task = Task.Run(async () =>
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
            });
        }

        public async Task CancelAsync()
        {
            ThrowIfDisposed();

            if (_cts == null)
                throw new InvalidOperationException($"{nameof(TaskController)} - Task is not running, use {nameof(Begin)} to start.");

            _cts.Cancel();

            await Task; // wait for task to complete.

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
                throw new ObjectDisposedException(nameof(TaskController));
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
