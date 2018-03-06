using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Microsoft.Extensions.Logging;

namespace Binance.Utility
{
    public class BinanceWebSocketStreamController : WebSocketStreamController<IBinanceWebSocketStream>, IBinanceWebSocketStreamController
    {
        #region Public Constants

        public const int SystemMaintenanceCheckDelayMilliseconds = 30 * 1000; // 30 seconds.

        #endregion Public Constants

        #region Private Fields

        private readonly IBinanceApi _api;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// The default constructor.
        /// </summary>
        public BinanceWebSocketStreamController()
            : this(new BinanceApi(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        public BinanceWebSocketStreamController(IBinanceApi api, IBinanceWebSocketStream stream, ILogger<BinanceWebSocketStreamController> logger = null)
            : base(stream, logger)
        {
            Throw.IfNull(api, nameof(api));

            _api = api;
        }

        #endregion Constructors

        #region Protected Methods

        protected override async Task DelayAsync(CancellationToken token)
        {
            var status = BinanceStatus.Normal;

            try
            {
                status = await _api.GetSystemStatusAsync(token)
                    .ConfigureAwait(false);
            }
            catch { /* ignore */ }

            Logger?.LogDebug($"{nameof(BinanceWebSocketStreamController)}: System status ({status}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            if (status == BinanceStatus.Normal)
            {
                await base.DelayAsync(token)
                    .ConfigureAwait(false);

                return;
            }

            while (status == BinanceStatus.Maintenance)
            {
                // Notify listeners.
                OnPausing(TimeSpan.FromMilliseconds(SystemMaintenanceCheckDelayMilliseconds));

                Logger?.LogDebug($"{nameof(BinanceWebSocketStreamController)}: Delaying for {SystemMaintenanceCheckDelayMilliseconds} msec.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                await Task.Delay(SystemMaintenanceCheckDelayMilliseconds, token)
                    .ConfigureAwait(false);

                try
                {
                    status = await _api.GetSystemStatusAsync(token)
                        .ConfigureAwait(false);
                }
                catch { /* ignore */ }
            }
        }

        #endregion Protected Methods
    }
}
