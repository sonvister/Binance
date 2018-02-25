using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Utility
{
    public class RetryTaskController : TaskController, IRetryTaskController
    {
        #region Public Events

        public event EventHandler<PausingEventArgs> Pausing
        {
            add
            {
                if (_pausing == null || !_pausing.GetInvocationList().Contains(value))
                {
                    _pausing += value;
                }
            }
            remove => _pausing -= value;
        }
        private EventHandler<PausingEventArgs> _pausing;

        public event EventHandler<EventArgs> Resuming
        {
            add
            {
                if (_resuming == null || !_resuming.GetInvocationList().Contains(value))
                {
                    _resuming += value;
                }
            }
            remove => _resuming -= value;
        }
        private EventHandler<EventArgs> _resuming;

        #endregion Public Events

        #region Public Properties

        public int RetryDelayMilliseconds { get; set; } = 5000;

        #endregion Public Properties

        #region Constructors

        public RetryTaskController(Func<CancellationToken, Task> action)
            : base(action)
        { }

        #endregion Constructors

        #region Public Methods

        public override void Begin(Func<CancellationToken, Task> action = null)
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

            Task = Task.Run(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try
                    {
                        await Action(Cts.Token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        if (!Cts.IsCancellationRequested)
                        {
                            OnError(e);
                        }
                    }

                    try
                    {
                        if (!Cts.IsCancellationRequested)
                        {
                            await DelayAsync(Cts.Token)
                                .ConfigureAwait(false);
                        }
                    }
                    catch { /* ignored */ }

                    if (!Cts.IsCancellationRequested)
                    {
                        OnResuming();
                    }
                }
            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual Task DelayAsync(CancellationToken token)
        {
            // Notify listeners.
            OnPausing(TimeSpan.FromMilliseconds(RetryDelayMilliseconds));

            return Task.Delay(RetryDelayMilliseconds, token);
        }

        /// <summary>
        /// Raise a pausing event.
        /// </summary>
        /// <param name="timeSpan"></param>
        protected void OnPausing(TimeSpan timeSpan)
        {
            try { _pausing?.Invoke(this, new PausingEventArgs(timeSpan)); }
            catch { /* ignored */ }
        }

        /// <summary>
        /// Raise a resuming event.
        /// </summary>
        protected void OnResuming()
        {
            try { _resuming?.Invoke(this, EventArgs.Empty); }
            catch { /* ignored */ }
        }

        #endregion Protected Methods
    }
}
