using System;
using System.Diagnostics;
using System.Threading;

namespace Binance.Utility
{
    /// <summary>
    /// The default <see cref="IWatchdogTimer"/> implementation.
    /// </summary>
    public sealed class WatchdogTimer : IWatchdogTimer
    {
        #region Public Properties

        public bool IsEnabled { get; set; } = true;

        public TimeSpan Interval { get; set; }

        #endregion Public Properties

        #region Private Fields

        private Timer _timer;

        private readonly Stopwatch _stopwatch;

        private readonly Action _onTimeout;

        private readonly int _timerResolutionMilliseconds;

        private readonly object _sync = new object();

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
            _timerResolutionMilliseconds = timerResolutionMilliseconds;

            _stopwatch = new Stopwatch();
        }

        #endregion Constructors

        #region Public Methods

        public void Kick()
        {
            if (!IsEnabled)
                return;

            if (_timer == null)
            {
                lock (_sync)
                {
                    if (_timer == null)
                    {
                        _timer = new Timer(OnTimer, null, _timerResolutionMilliseconds, _timerResolutionMilliseconds);
                    }
                }
            }

            _stopwatch.Restart();
        }

        #endregion Public Methods

        #region Private Methods

        private void OnTimer(object state)
        {
            if (_stopwatch.Elapsed < Interval)
                return;

            _stopwatch.Reset();

            if (IsEnabled)
            {
                try { _onTimeout(); }
                catch { /* ignore */  }
            }

            lock (_sync)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion Private Methods
    }
}
