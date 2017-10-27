using Binance.Api.WebSocket.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public interface ITradesWebSocketClient : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The aggregate trade event.
        /// </summary>
        event EventHandler<AggregateTradeEventArgs> AggregateTrade;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving aggregate
        /// trade events. Awaiting this method will not return until the token
        /// is canceled, this <see cref="ITradesWebSocketClient"> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving aggregate
        /// trade events. Awaiting this method will not return until the token
        /// is canceled, this <see cref="ITradesWebSocketClient"> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="callback"/>An event callback.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, Action<AggregateTradeEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
