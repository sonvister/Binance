using System;
using System.Diagnostics;
using System.Threading;

namespace Binance.Utility
{
    public sealed class WatchdogTimer : IWatchdogTimer, IDisposable
    {
        public TimeSpan Interval { get; set; }

        private Timer _timer;

        private Stopwatch _stopwatch;

        private Action _onTimeout;

        public WatchdogTimer(Action onTimeout)
        {
            Throw.IfNull(onTimeout, nameof(onTimeout));

            _onTimeout = onTimeout;

            _stopwatch = Stopwatch.StartNew();

            _timer = new Timer(OnTimer, null, 100, 100);
        }

        public void Kick()
        {
            _stopwatch.Restart();
        }

        private void OnTimer(object state)
        {
            if (_stopwatch.Elapsed >= Interval)
            {
                _onTimeout();
                _stopwatch.Restart();
            }
        }

        #region IDisposable

        private bool _disposed;

        void Dispose(bool disposing)
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
