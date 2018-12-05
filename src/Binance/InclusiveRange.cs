using System;

namespace Binance
{
    public class InclusiveRange
    {
        #region Public Properties

        /// <summary>
        /// Get the miniumum value.
        /// </summary>
        public virtual decimal Minimum => _minimum;

        /// <summary>
        /// Get the maximum value.
        /// </summary>
        public virtual decimal Maximum => _maximum;

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

        #region Private Fields

        private decimal _minimum;
        private decimal _maximum;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="increment"></param>
        public InclusiveRange(decimal minimum, decimal maximum, decimal increment)
        {
            if (minimum < 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must not be less than 0.", nameof(minimum));
            if (maximum < 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must not be less than 0.", nameof(maximum));
            if (increment <= 0)
                throw new ArgumentException($"{nameof(InclusiveRange)}: value must be greater than 0.", nameof(increment));

            _minimum = minimum;
            _maximum = maximum;

            Increment = increment;
        }

        #endregion Constructors
    }
}
