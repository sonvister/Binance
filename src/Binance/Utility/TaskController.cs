using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class TaskController : ITaskController
    {
        #region Public Events

        public event EventHandler<ErrorEventArgs> Error
        {
            add
            {
                if (_error == null || !_error.GetInvocationList().Contains(value))
                {
                    _error += value;
                }
            }
            remove => _error -= value;
        }
        private EventHandler<ErrorEventArgs> _error;

        #endregion Public Events

        #region Public Properties

        public bool IsActive { get; protected set; }

        public Task Task { get; protected set; }

        #endregion Public Properties

        #region Protected Fields

        protected Func<CancellationToken, Task> Action;
        protected Action<Exception> ErrorAction;
        protected CancellationTokenSource Cts;
        protected readonly object Sync = new object();

        #endregion Protected Fields

        #region Constructors

        public TaskController(Func<CancellationToken, Task> action, Action<Exception> onError = null)
        {
            Throw.IfNull(action, nameof(action));

            Action = action;
            ErrorAction = onError;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void Begin()
        {
            ThrowIfDisposed();

            lock (Sync)
            {
                if (IsActive)
                    return;

                IsActive = true;

                Cts?.Dispose();

                Cts = new CancellationTokenSource();
            }

            Task = Task.Run(async () =>
            {
                try { await Action(Cts.Token).ConfigureAwait(false); }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    if (!Cts.IsCancellationRequested)
                    {
                        try { ErrorAction?.Invoke(e); } 
                        catch { /* ignored */ }

                        OnError(e);
                    }
                }
            });
        }

        public virtual void Abort()
        {
            ThrowIfDisposed();

            lock (Sync)
            {
                if (!IsActive)
                    return;

                IsActive = false;

                Cts?.Cancel();
            }
        }

        public virtual async Task CancelAsync()
        {
            Abort();

            if (Task != null && !Task.IsCompleted)
            {
                await Task // wait for task to complete.
                    .ConfigureAwait(false);
            }
        }

        public virtual async Task RestartAsync()
        {
            await CancelAsync()
                .ConfigureAwait(false);

            Begin();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise an error event.
        /// </summary>
        /// <param name="exception"></param>
        protected void OnError(Exception exception)
        {
            try { _error?.Invoke(this, new ErrorEventArgs(exception)); }
            catch (Exception) { /* ignored */ }
        }

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

                Cts?.Dispose();
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
