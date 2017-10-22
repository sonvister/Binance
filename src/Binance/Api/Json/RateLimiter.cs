using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.Json
{
    public sealed class RateLimiter : IRateLimiter
    {
        #region Public Constants

        public static readonly bool EnabledDefault = true;

        public static readonly int CountDefault = 3;

        public static readonly TimeSpan DurationDefault = TimeSpan.FromSeconds(1);

        #endregion Public Constants

        #region Public Properties

        public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

        public int Count { get; private set; } = CountDefault;

        public TimeSpan Duration { get; private set; } = DurationDefault;

        #endregion Public Properties

        #region Private Fields

        private Queue<long> _timestamps = new Queue<long>();

        private readonly object _sync = new object();

        private volatile bool _isEnabled = EnabledDefault;

        #endregion Private Fields

        #region Public Methods

        public void Configure(int count, TimeSpan duration)
        {
            if (count <= 0)
                throw new ArgumentException($"{nameof(IRateLimiter)} configured count must be greater than 0.", nameof(count));

            lock (_sync)
            {
                Count = count;
                Duration = duration;

                while (_timestamps.Count > Count)
                    _timestamps.Dequeue();
            }
        }

        public async Task DelayAsync(CancellationToken token = default)
        {
            if (!IsEnabled)
                return;

            var millisecondsDelay = 0;

            lock (_sync)
            {
                try
                {
                    // Create the current timestamp.
                    var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    // Queue the current timestammp.
                    _timestamps.Enqueue(now);

                    // If the maximum count has not been exceeded.
                    if (_timestamps.Count <= Count)
                        return;

                    // Get the oldest timestamp.
                    var then = _timestamps.Dequeue();

                    // How long has it been?
                    var time = Convert.ToInt32(now - then);

                    // If elapsed time is less than allowed time...
                    if (time < Duration.TotalMilliseconds)
                    {
                        // ...set the delay as the time difference.
                        millisecondsDelay = Convert.ToInt32(Duration.TotalMilliseconds) - time;
                    }
                }
                catch (OverflowException) { }
            }

            // Delay if required.
            if (millisecondsDelay > 0)
                await Task.Delay(millisecondsDelay, token)
                    .ConfigureAwait(false);
        }

        #endregion Public Methods
    }
}
