using System;

namespace Binance
{
    public sealed class InclusiveRange
    {
        #region Public Properties

        /// <summary>
        /// Get the miniumum value.
        /// </summary>
        public decimal Minimum { get; }

        /// <summary>
        /// Get the maximum value.
        /// </summary>
        public decimal Maximum { get; }

        /// <summary>
        /// Get the increment value.
        /// </summary>
        public decimal Increment { get; }

        #endregion Public Properties

        #region Implicit Operators

        public static implicit operator (decimal, decimal, decimal) (InclusiveRange range)
            => (range.Minimum, range.Maximum, range.Increment);

        public static implicit operator InclusiveRange((decimal, decimal, decimal) range)
            => new InclusiveRange(range.Item1, range.Item2, range.Item3);

        #endregion Implicit Operators

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="increment"></param>
        public InclusiveRange(decimal minimum, decimal maximum, decimal increment)
        {
            if (minimum <= 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must be greater than 0.", nameof(minimum));
            if (maximum <= 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must be greater than 0.", nameof(maximum));
            if (increment <= 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must be greater than 0.", nameof(increment));

            Minimum = minimum;
            Maximum = maximum;
            Increment = increment;
        }

        #endregion Constructors

        #region Public Methods

        public bool IsValid(decimal value)
        {
            return value >= Minimum && value <= Maximum && (value - Minimum) % Increment == 0;
        }

        #endregion Public Methods
    }
}
