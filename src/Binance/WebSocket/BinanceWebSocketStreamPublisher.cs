using System.Linq;
using Binance.Producer;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IBinanceWebSocketStreamPublisher"/> implementation.
    /// </summary>
    public class BinanceWebSocketStreamPublisher : AutoJsonStreamPublisher<IBinanceWebSocketStream>, IBinanceWebSocketStreamPublisher
    {
        #region Constructors

        /// <summary>
        /// The default constructor with default <see cref="IBinanceWebSocketStreamController"/> and no logging.
        /// </summary>
        public BinanceWebSocketStreamPublisher()
            : this(new BinanceWebSocketStreamController())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="logger"></param>
        public BinanceWebSocketStreamPublisher(IBinanceWebSocketStreamController controller, ILogger<BinanceWebSocketStreamPublisher> logger = null)
            : base(controller, logger)
        { }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Update web socket stream URI when published streams change.
        /// </summary>
        protected override void OnPublishedStreamsChanged()
        {
            Stream.Uri = BinanceWebSocketStream.CreateUri(PublishedStreams.ToArray());

            // Call base class to handle automatic streaming.
            base.OnPublishedStreamsChanged();
        }

        #endregion Protected Methods
    }
}
