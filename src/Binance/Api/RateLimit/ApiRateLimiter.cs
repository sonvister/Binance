using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Binance.Api
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

        private IServiceProvider _services;

        private volatile bool _isEnabled = EnabledDefault;

        private readonly IDictionary<TimeSpan, IRateLimiter> _limiters
            = new Dictionary<TimeSpan, IRateLimiter>();

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services"></param>
        public ApiRateLimiter(IServiceProvider services = null)
        {
            _services = services;
        }

        #endregion Constructors

        #region Public Methods

        public void Configure(TimeSpan duration, int count)
        {
            if (count < 0)
                throw new ArgumentException($"{nameof(IApiRateLimiter)} count must be greater than 0 (or equal to 0 to disable).", nameof(count));

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

                var limiter = _services?.GetService<IRateLimiter>() ?? new RateLimiter();

                limiter.Duration = duration;
                limiter.Count = count;

                _limiters[duration] = limiter;
            }
        }

        public async Task DelayAsync(CancellationToken token = default)
        {
            if (!IsEnabled)
                return;

            IEnumerable<IRateLimiter> limiters;

            lock (_sync)
            {
                limiters = _limiters.Values
                    .Where(_ => _.Count > 0)
                    .ToArray();
            }

            foreach (var limiter in limiters)
                await limiter.DelayAsync(token)
                    .ConfigureAwait(false);
        }

        #endregion Public Methods
    }
}
