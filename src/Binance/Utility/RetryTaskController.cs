using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class RetryTaskController : TaskController, IRetryTaskController
    {
        #region Public Properties

        public int RetryDelayMilliseconds { get; set; } = 5000;

        #endregion Public Properties

        #region Constructors

        public RetryTaskController(Func<CancellationToken, Task> action, Action<Exception> onError = null)
            : base(action, onError)
        { }

        #endregion Constructors

        #region Public Methods

        public override void Begin(Func<CancellationToken, Task> action = null, Action<Exception> onError = null)
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

            if (action != null)
                Action = action;

            if (onError != null)
                ErrorAction = onError;

            Task = Task.Run(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try { await Action(Cts.Token).ConfigureAwait(false); }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        if (!Cts.IsCancellationRequested)
                        {
                            try { ErrorAction?.Invoke(e); }
                            catch { /* ignored */ }

                            OnError(e);
                        }
                    }

                    try
                    {
                        if (!Cts.IsCancellationRequested)
                        {
                            await DelayAsync(Cts.Token).ConfigureAwait(false);
                        }
                    }
                    catch { /* ignored */ }
                }
            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual Task DelayAsync(CancellationToken token)
        {
            return Task.Delay(RetryDelayMilliseconds, token);
        }

        #endregion Protected Methods
    }
}
