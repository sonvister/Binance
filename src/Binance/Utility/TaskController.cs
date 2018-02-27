using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        protected ILogger<TaskController> Logger;

        protected CancellationTokenSource Cts;

        protected readonly object Sync = new object();

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        public TaskController(Func<CancellationToken, Task> action = null, ILogger<TaskController> logger = null)
        {
            Action = action;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void Begin(Func<CancellationToken, Task> action = null)
        {
            ThrowIfDisposed();

            lock (Sync)
            {
                if (action != null)
                    Action = action;

                Throw.IfNull(Action, nameof(action));

                if (IsActive)
                    return;

                IsActive = true;

                Cts?.Dispose();

                Cts = new CancellationTokenSource();
            }

            Task = Task.Run(async () =>
            {
                Logger?.LogDebug($"{nameof(TaskController)}: Task beginning...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                // ReSharper disable once InconsistentlySynchronizedField
                try
                {
                    await Action(Cts.Token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { /* ignore */  }
                catch (Exception e)
                {
                    Logger?.LogWarning(e, $"{nameof(TaskController)}: Unhandled action exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    // ReSharper disable once InconsistentlySynchronizedField
                    if (!Cts.IsCancellationRequested)
                    {
                        OnError(e);
                    }
                }

                Logger?.LogDebug($"{nameof(TaskController)}: Task complete.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
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

            lock (Sync)
            {
                Cts?.Dispose();
                Cts = null;
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
            catch { /* ignore */ }
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TaskController));
        }

        protected virtual void Dispose(bool disposing)
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
