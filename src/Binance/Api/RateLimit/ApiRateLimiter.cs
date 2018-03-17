using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class ApiRateLimiter : IApiRateLimiter
    {
        #region Public Constants

        public static readonly bool EnabledDefault = true;

        #endregion Public Constants

        #region Public Properties

        public bool IsEnabled { get => _isEnabled; set => _isEnabled = value; }

        #endregion Public Properties

        #region Private Fields

        private readonly IRateLimiterProvider _rateLimiterProvider;

        private volatile bool _isEnabled = EnabledDefault;

        private readonly IDictionary<TimeSpan, IRateLimiter> _limiters
            = new Dictionary<TimeSpan, IRateLimiter>();

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// The default constructor with default <see cref="IRateLimiterProvider"/>.
        /// </summary>
        public ApiRateLimiter()
            : this(new RateLimiterProvider())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="rateLimiterProvider">The rate limiter provider (required).</param>
        public ApiRateLimiter(IRateLimiterProvider rateLimiterProvider)
        {
            Throw.IfNull(rateLimiterProvider, nameof(rateLimiterProvider));

            _rateLimiterProvider = rateLimiterProvider;
        }

        #endregion Constructors

        #region Public Methods

        public void Configure(TimeSpan duration, int count)
        {
            if (count < 0)
                throw new ArgumentException($"{nameof(IApiRateLimiter)} count must be greater than 0 (or equal to 0 to disable).", nameof(count));

            ThrowIfDisposed();

            lock (_sync)
            {
                if (count == 0)
                {
                    if (_limiters.ContainsKey(duration))
                    {
                        _limiters.Remove(duration);
                    }
                    return;
                }

                if (_limiters.ContainsKey(duration))
                {
                    _limiters[duration].Count = count;
                    return;
                }

                var limiter = _rateLimiterProvider.CreateRateLimiter();

                limiter.Duration = duration;
                limiter.Count = count;

                _limiters[duration] = limiter;
            }
        }

        public async Task DelayAsync(int count = 1, CancellationToken token = default)
        {
            if (!IsEnabled)
                return;

            ThrowIfDisposed();

            IEnumerable<IRateLimiter> limiters;

            lock (_sync)
            {
                limiters = _limiters.Values.ToArray();
            }

            foreach (var limiter in limiters)
            {
                await limiter.DelayAsync(count, token)
                    .ConfigureAwait(false);
            }
        }

        #endregion Public Methods

        #region IDisposable

        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ApiRateLimiter));
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                lock (_sync)
                {
                    foreach (var limiter in _limiters.Values)
                    {
                        limiter.Dispose();
                    }
                }
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
