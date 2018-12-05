using System;

namespace Binance
{
    public sealed class PriceRange : InclusiveRange
    {
        #region Public Properties

        public decimal MultiplierUp { get; }

        public decimal MultiplierDown { get; }

        public override decimal Minimum => Math.Floor(GetAveragePrice() * MultiplierDown / Increment) * Increment;

        public override decimal Maximum => Math.Ceiling(GetAveragePrice() * MultiplierUp / Increment) * Increment;

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
            if (_averagePrice == 0 || DateTime.UtcNow - _lastUpdate > TimeSpan.FromSeconds(1))
            {
                _averagePrice = _api.GetAvgPriceAsync(_symbol).GetAwaiter().GetResult().Value;
                _lastUpdate = DateTime.UtcNow;
            }

            return _averagePrice;
        }

        #endregion Private Methods
    }
}
