using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    public class BinanceWebSocketStreamController : WebSocketStreamController
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
        public BinanceWebSocketStreamController(IBinanceApi api, IWebSocketStream stream, ILogger<BinanceWebSocketStreamController> logger = null)
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
