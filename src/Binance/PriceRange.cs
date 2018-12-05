using System;

namespace Binance
{
    public sealed class PriceRange : InclusiveRange
    {
        #region Public Properties

        public decimal MultiplierUp { get; }

        public decimal MultiplierDown { get; }

        /// <summary>
        /// Get the realtime minimum price.
        /// 
        /// NOTE: Average price changes frequently; using the exact minimum value for an order price occassionaly fails.
        ///       So, a (10 * Increment) buffer is added to the minimum calculation to improve reliability.
        /// </summary>
        public override decimal Minimum => Math.Ceiling(GetAveragePrice() * MultiplierDown / Increment) * Increment + 10 * Increment;

        /// <summary>
        /// Get the realtime maximum price.
        /// 
        /// NOTE: Average price changes frequently; using the exact maximum value for an order price occassionaly fails.
        ///       So, a (10 * Increment) buffer is subtraced from the maximum calculation to improve reliability.
        /// </summary>
        public override decimal Maximum => Math.Floor(GetAveragePrice() * MultiplierUp / Increment) * Increment - 10 * Increment;

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private string _symbol;

        private decimal _averagePrice;

        private DateTime _lastUpdate = DateTime.MinValue;

        #endregion Private Fields

        #region Constructor

        public PriceRange(IBinanceApi api, string symbol, decimal multiplierUp, decimal multiplierDown, decimal increment)
            : base(0, 0, increment)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            _api = api;
            _symbol = symbol;

            MultiplierUp = multiplierUp;
            MultiplierDown = multiplierDown;
        }

        #endregion Constructor

        #region Private Methods

        private decimal GetAveragePrice()
        {
            // TODO: Add a BinanceApiOption property for this update delay seconds/milliseconds... ?
            if (_averagePrice == 0 || DateTime.UtcNow - _lastUpdate > TimeSpan.FromSeconds(1))
            {
                // TODO: Redesign price/quantity validation to use async methods rather than properties... ?
                _averagePrice = _api.GetAvgPriceAsync(_symbol).GetAwaiter().GetResult().Value; // HACK
                _lastUpdate = DateTime.UtcNow;
            }

            return _averagePrice;
        }

        #endregion Private Methods
    }
}
