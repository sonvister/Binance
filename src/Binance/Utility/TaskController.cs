using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class TaskController : ITaskController
    {
        #region Public Properties

        public bool IsActive { get; protected set; }

        public Task Task { get; protected set; }

        #endregion Public Properties

        #region Protected Fields

        protected Func<CancellationToken, Task> _action;
        protected Action<Exception> _onError;
        protected CancellationTokenSource _cts;

        #endregion Protected Fields

        #region Constructors

        public TaskController(Func<CancellationToken, Task> action, Action<Exception> onError = null)
        {
            Throw.IfNull(action, nameof(action));

            _action = action;
            _onError = onError;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void Begin()
        {
            ThrowIfDisposed();

            if (IsActive)
                throw new InvalidOperationException($"{nameof(TaskController)} - Task already running, use {nameof(CancelAsync)} to abort.");

            IsActive = true;

            _cts = new CancellationTokenSource();

            Task = Task.Run(async () =>
            {
                try { await _action(_cts.Token).ConfigureAwait(false); }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    if (!_cts.IsCancellationRequested)
                    {
                        try
                        {
                            _onError?.Invoke(e);
                            OnError(e);
                        }
                        catch { /* ignored */}
                    }
                }
            });
        }

        public virtual async Task CancelAsync()
        {
            ThrowIfDisposed();

            if (!IsActive)
                return;

            IsActive = false;

            _cts?.Cancel();

            if (Task != null && !Task.IsCompleted)
            {
                await Task // wait for task to complete.
                    .ConfigureAwait(false);
            }

            _cts?.Dispose();
            _cts = null;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnError(Exception e) { }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected void ThrowIfDisposed()
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
                CancelAsync().GetAwaiter().GetResult();
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
