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

        public override void Begin()
        {
            ThrowIfDisposed();

            if (IsActive)
                throw new InvalidOperationException($"{nameof(RetryTaskController)} - Task already running, use {nameof(CancelAsync)} to abort.");

            IsActive = true;

            Cts = new CancellationTokenSource();

            Task = Task.Run(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try { await Action(Cts.Token).ConfigureAwait(false); }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!Cts.IsCancellationRequested)
                        {
                            try
                            {
                                ErrorAction?.Invoke(e);
                                OnError(e);
                            }
                            catch { /* ignored */}
                        }
                    }

                    if (!Cts.IsCancellationRequested)
                    {
                        await Task.Delay(RetryDelayMilliseconds, Cts.Token)
                            .ConfigureAwait(false);
                    }
                }
            });
        }

        #endregion Public Methods
    }
}
