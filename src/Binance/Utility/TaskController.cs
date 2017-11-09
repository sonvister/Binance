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

        #region Constructors

        public TaskController()
        {
            _cts = new CancellationTokenSource();
        }

        #endregion Constructors

        #region Public Methods

        public virtual void Run(Func<CancellationToken, Task> action, Action<Exception> onError = null)
        {
            Task = Task.Run(async () =>
            {
                try { await action(_cts.Token); }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    onError?.Invoke(e);
                    OnError(e);
                }
            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnError(Exception e) { }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed = false;

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
