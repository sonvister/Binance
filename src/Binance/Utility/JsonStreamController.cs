using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Producer;
using Microsoft.Extensions.Logging;

namespace Binance.Utility
{
    public class JsonStreamController<TStream> : RetryTaskController, IJsonStreamController<TStream>
        where TStream : IJsonStream
    {
        #region Private Constants

        private readonly TimeSpan _watchdogTimerIntervalDefault = TimeSpan.FromHours(1);

        #endregion Private Constants

        #region Public Properties

        public TStream Stream { get; }

        public IWatchdogTimer Watchdog { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        public JsonStreamController(TStream stream, ILogger<JsonStreamController<TStream>> logger = null)
            : base(stream.StreamAsync, logger)
        {
            Throw.IfNull(stream, nameof(stream));

            Stream = stream;

            Watchdog = new WatchdogTimer(async () =>
            {
                Logger?.LogInformation($"{nameof(JsonStreamController<TStream>)}: Watchdog timer restarting stream controller (no data received in {Watchdog.Interval.TotalMinutes} minutes).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                await RestartAsync()
                    .ConfigureAwait(false);
            })
            {
                Interval = _watchdogTimerIntervalDefault
            };

            Stream.Message += (s, e) =>
            {
                Watchdog.Kick();
            };
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Begin streaming after verifying stream is not active.
        /// </summary>
        public override void Begin(Func<CancellationToken, Task> action = null)
        {
            if (!Stream.IsStreaming)
            {
                base.Begin(action);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Task ActionAsync()
        {
            Watchdog.Kick();

            return base.ActionAsync();
        }

        #endregion Protected Methods
    }
}
