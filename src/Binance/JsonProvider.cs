using System;
using Microsoft.Extensions.Logging;

namespace Binance
{
    /// <summary>
    /// An abstract <see cref="IJsonProvider"/> implementation.
    /// </summary>
    public abstract class JsonProvider : IJsonProvider
    {
        #region Public Events

        public event EventHandler<JsonMessageEventArgs> Message;

        #endregion Public Events

        #region Protected Fields

        protected readonly ILogger<JsonProvider> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Optional <see cref="ILogger{JsonProvider}"/>.</param>
        protected JsonProvider(ILogger<JsonProvider> logger = null)
        {
            Logger = logger;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Raise a JSON message event.
        /// </summary>
        /// <param name="json">The JSON message (required).</param>
        /// <param name="subject">The JSON message context (optional).</param>
        protected void RaiseMessageEvent(string json, string subject = null)
        {
            try { Message?.Invoke(this, new JsonMessageEventArgs(json, subject)); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled {nameof(Message)} event handler exception.");
            }
        }

        #endregion Protected Methods
    }
}
