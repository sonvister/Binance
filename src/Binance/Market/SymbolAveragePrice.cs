using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// A symbol/average price value object.
    /// See https://github.com/binance-exchange/binance-official-api-docs/blob/master/CHANGELOG.md for an explanation of the average price calculation.
    /// </summary>
    public sealed class SymbolAveragePrice : IEquatable<SymbolAveragePrice>
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the moving average time interval minutes.
        /// </summary>
        public int Minutes { get; }

        /// <summary>
        /// Get the average price value.
        /// </summary>
        public decimal Value { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="minutes">The moving average time interval minutes.</param>
        /// <param name="value">The average price value.</param>
        public SymbolAveragePrice(string symbol, int minutes, decimal value)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (minutes <= 0)
                throw new ArgumentException($"{nameof(SymbolAveragePrice)} value must be greater than 0.", nameof(minutes));

            if (value < 0)
                throw new ArgumentException($"{nameof(SymbolAveragePrice)} value must not be less than 0.", nameof(value));

            Symbol = symbol.FormatSymbol();
            Minutes = minutes;
            Value = value;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Display average price value as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.ToString("0.00000000", CultureInfo.InvariantCulture);
        }

        #endregion Public Methods

        #region IEquatable

        public bool Equals(SymbolAveragePrice other)
        {
            if (other == null)
                return false;

            return other.Symbol == Symbol
                   && other.Minutes == Minutes
                   && other.Value == Value;
        }

        #endregion IEquatable
    }
}
