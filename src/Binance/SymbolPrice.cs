using System;

namespace Binance
{
    /// <summary>
    /// A symbol/price value object.
    /// </summary>
    public sealed class SymbolPrice
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// Get the price value.
        /// </summary>
        public decimal Value { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="value">The price value.</param>
        public SymbolPrice(string symbol, decimal value)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (value < 0)
                throw new ArgumentException($"{nameof(SymbolPrice)} price must not be less than 0.", nameof(value));

            Symbol = symbol;
            Value = value;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Display price value as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString("0.00000000");
        }

        #endregion Public Methods
    }
}
