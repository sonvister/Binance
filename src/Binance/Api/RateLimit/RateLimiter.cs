﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    public class RateLimiter : IRateLimiter
    {
        #region Public Properties

        public int Count
        {
            get => _count;
            set
            {
                if (value <= 0)
                    throw new ArgumentException($"{nameof(IApiRateLimiter)} configured count must be greater than 0.", nameof(Count));

                _count = value;

                while (_timestamps.Count > _count)
                {
                    _timestamps.Dequeue();
                }
            }
        }

        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(1);

        #endregion Public Properties

        #region Private Fields

        private int _count;

        private readonly Queue<long> _timestamps = new Queue<long>();

        private readonly ILogger<RateLimiter> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public RateLimiter(ILogger<RateLimiter> logger = null)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public async Task DelayAsync(CancellationToken token = default)
        {
            if (_count == 0)
                return;

            token.ThrowIfCancellationRequested();

            var millisecondsDelay = 0;

            // Create the current timestamp.
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // If the maximum count has not been reached.
            if (_timestamps.Count < _count)
            {
                // Queue the current timestammp.
                _timestamps.Enqueue(now);
                return;
            }

            // Remove the oldest timestamp.
            var then = _timestamps.Dequeue();

            try
            {
                // How long has it been?
                var time = Convert.ToInt32(now - then);

                // If elapsed time is less than allowed time...
                if (time < Duration.TotalMilliseconds)
                {
                    // ...set the delay as the time difference.
                    millisecondsDelay = Convert.ToInt32(Duration.TotalMilliseconds) - time;
                }
            }
            catch (OverflowException) { /* ignore */  }

            // Add the current/future timestammp.
            _timestamps.Enqueue(now + millisecondsDelay);

            // Delay if required.
            if (millisecondsDelay > 0)
            {
                _logger?.LogDebug($"{nameof(RateLimiter)} delaying for {millisecondsDelay} msec.");

                await Task.Delay(millisecondsDelay, token)
                    .ConfigureAwait(false);
            }
        }

        #endregion Public Methods
    }
}
