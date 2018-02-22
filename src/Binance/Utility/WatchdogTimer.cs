using System;
using System.Diagnostics;
using System.Threading;

namespace Binance.Utility
{
    /// <summary>
    /// The default <see cref="IWatchdogTimer"/> implementation.
    /// </summary>
    public sealed class WatchdogTimer : IWatchdogTimer, IDisposable
    {
        #region Public Properties

        public TimeSpan Interval { get; set; }

        #endregion Public Properties

        #region Private Fields

        private readonly Timer _timer;

        private readonly Stopwatch _stopwatch;

        private readonly Action _onTimeout;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="onTimeout">The timeout action (required).</param>
        /// <param name="timerResolutionMilliseconds">The internal timer resolution (optional).</param>
        public WatchdogTimer(Action onTimeout, int timerResolutionMilliseconds = 100)
        {
            Throw.IfNull(onTimeout, nameof(onTimeout));

            if (timerResolutionMilliseconds <= 0)
                throw new ArgumentException($"{nameof(WatchdogTimer)}: Timer resolution ({timerResolutionMilliseconds}) must be greater than 0 milliseconds.", nameof(timerResolutionMilliseconds));

            _onTimeout = onTimeout;

            _stopwatch = Stopwatch.StartNew();

            _timer = new Timer(OnTimer, null, timerResolutionMilliseconds, timerResolutionMilliseconds);
        }

        #endregion Constructors

        #region Public Methods

        public void Kick()
        {
            _stopwatch.Restart();
        }

        #endregion Public Methods

        #region Private Methods

        private void OnTimer(object state)
        {
            if (_stopwatch.Elapsed < Interval)
                return;

            _onTimeout();
            _stopwatch.Restart();
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _timer?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
