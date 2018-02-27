using System;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default abstract <see cref="IManager"/> base class.
    /// </summary>
    public abstract class ControllerManager : ControllerManager<ErrorEventArgs>
    {
        #region Constructors

        protected ControllerManager(ILogger<ControllerManager> logger = null)
            : base(logger)
        { }

        #endregion Constructors
    }

    /// <summary>
    /// An abstract <see cref="IManager"/> base class.
    /// </summary>
    public abstract class ControllerManager<TEventArgs> : IManager<TEventArgs>
        where TEventArgs : ErrorEventArgs
    {
        #region Public Events

        public event EventHandler<TEventArgs> Error
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
        private EventHandler<TEventArgs> _error;

        #endregion Public Events

        #region Protected Fields

        protected ILogger<ControllerManager<TEventArgs>> Logger;

        #endregion Protected Fields

        #region Constructors

        protected ControllerManager(ILogger<ControllerManager<TEventArgs>> logger = null)
        {
            Logger = logger;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Raise an error event.
        /// </summary>
        /// <param name="exception"></param>
        protected virtual void OnError(TEventArgs args)
        {
            try { _error?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{GetType().Name}.{nameof(OnError)}: Unhandled {nameof(Error)} event handler exception.");
            }
        }

        #endregion Protected Methods

        #region IDisposable

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
