using System;
using Xunit;

namespace Binance.Tests
{
    public class InclusiveRangeTest
    {
        [Fact]
        public void IsValid()
        {
            const decimal minimum = 0.01m;
            const decimal maximum = 10.0m;
            const decimal increment = 0.01m;

            var range = new InclusiveRange(minimum, maximum, increment);

            Assert.True(range.IsValid(minimum));
            Assert.True(range.IsValid(maximum));
            Assert.True(range.IsValid(minimum + increment));
            Assert.True(range.IsValid(maximum - increment));

            Assert.False(range.IsValid(minimum - increment));
            Assert.False(range.IsValid(maximum + increment));
            Assert.False(range.IsValid(minimum + increment / 2));
            Assert.False(range.IsValid(maximum - increment / 2));
        }

        [Fact]
        public void Validate()
        {
            const decimal minimum = 0.01m;
            const decimal maximum = 10.0m;
            const decimal increment = 0.01m;

            var range = new InclusiveRange(minimum, maximum, increment);

            range.Validate(minimum);
            range.Validate(maximum);
            range.Validate(minimum + increment);
            range.Validate(maximum - increment);

            Assert.Throws<ArgumentOutOfRangeException>("value", () => range.Validate(minimum - increment));
            Assert.Throws<ArgumentOutOfRangeException>("value", () => range.Validate(maximum + increment));
            Assert.Throws<ArgumentOutOfRangeException>("value", () => range.Validate(minimum + increment / 2));
            Assert.Throws<ArgumentOutOfRangeException>("value", () => range.Validate(maximum - increment / 2));
        }

        [Fact]
        public void GetUpperValidValue()
        {
            const decimal minimum = 0.01m;
            const decimal maximum = 10.0m;
            const decimal increment = 0.01m;

            var range = new InclusiveRange(minimum, maximum, increment);

            Assert.Equal(maximum, range.GetUpperValidValue(maximum));
            Assert.Equal(minimum, range.GetUpperValidValue(minimum));
            Assert.Equal(minimum + increment, range.GetUpperValidValue(minimum + increment));
            Assert.Equal(maximum - increment, range.GetUpperValidValue(maximum - increment));

            Assert.Equal(1.11m, range.GetUpperValidValue(1.110m));
            Assert.Equal(1.12m, range.GetUpperValidValue(1.112m));
            Assert.Equal(1.12m, range.GetUpperValidValue(1.115m));
            Assert.Equal(1.12m, range.GetUpperValidValue(1.118m));
            Assert.Equal(1.12m, range.GetUpperValidValue(1.120m));
            Assert.Equal(1.13m, range.GetUpperValidValue(1.122m));
            Assert.Equal(1.13m, range.GetUpperValidValue(1.125m));
            Assert.Equal(1.13m, range.GetUpperValidValue(1.128m));

            // 1.234 => 1.240 given range of [0.01 - 10.00] with increment of 0.01.
            Assert.Equal(1.240m, range.GetUpperValidValue(1.234m));
        }

        [Fact]
        public void GetLowerValidValue()
        {
            const decimal minimum = 0.01m;
            const decimal maximum = 10.0m;
            const decimal increment = 0.01m;

            var range = new InclusiveRange(minimum, maximum, increment);

            Assert.Equal(maximum, range.GetLowerValidValue(maximum));
            Assert.Equal(minimum, range.GetLowerValidValue(minimum));
            Assert.Equal(minimum + increment, range.GetLowerValidValue(minimum + increment));
            Assert.Equal(maximum - increment, range.GetLowerValidValue(maximum - increment));

            Assert.Equal(1.11m, range.GetLowerValidValue(1.110m));
            Assert.Equal(1.11m, range.GetLowerValidValue(1.112m));
            Assert.Equal(1.11m, range.GetLowerValidValue(1.115m));
            Assert.Equal(1.11m, range.GetLowerValidValue(1.118m));
            Assert.Equal(1.12m, range.GetLowerValidValue(1.120m));
            Assert.Equal(1.12m, range.GetLowerValidValue(1.122m));
            Assert.Equal(1.12m, range.GetLowerValidValue(1.125m));
            Assert.Equal(1.12m, range.GetLowerValidValue(1.128m));

            // 9.876 => 9.870 given range of [0.01 - 10.00] with increment of 0.01.
            Assert.Equal(9.870m, range.GetLowerValidValue(9.876m));
        }

        [Fact]
        public void GetValidValue()
        {
            const decimal minimum = 0.01m;
            const decimal maximum = 10.0m;
            const decimal increment = 0.01m;

            var range = new InclusiveRange(minimum, maximum, increment);

            Assert.Equal(maximum, range.GetValidValue(maximum));
            Assert.Equal(minimum, range.GetValidValue(minimum));
            Assert.Equal(minimum + increment, range.GetValidValue(minimum + increment));
            Assert.Equal(maximum - increment, range.GetValidValue(maximum - increment));

            var midpointRounding = MidpointRounding.ToEven;

            Assert.Equal(1.11m, range.GetValidValue(1.110m, midpointRounding));
            Assert.Equal(1.11m, range.GetValidValue(1.112m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.115m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.118m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.120m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.122m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.125m, midpointRounding));
            Assert.Equal(1.13m, range.GetValidValue(1.128m, midpointRounding));

            midpointRounding = MidpointRounding.AwayFromZero;

            Assert.Equal(1.11m, range.GetValidValue(1.110m, midpointRounding));
            Assert.Equal(1.11m, range.GetValidValue(1.112m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.115m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.118m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.120m, midpointRounding));
            Assert.Equal(1.12m, range.GetValidValue(1.122m, midpointRounding));
            Assert.Equal(1.13m, range.GetValidValue(1.125m, midpointRounding));
            Assert.Equal(1.13m, range.GetValidValue(1.128m, midpointRounding));

            // 1.234 => 1.230 given range of [0.01 - 10.00] with increment of 0.01.
            Assert.Equal(1.230m, range.GetValidValue(1.234m));
            // 2.345 => 2.340 given range of [0.01 - 10.00] with increment of 0.01 (midpoint rounding to even).
            // ReSharper disable once RedundantArgumentDefaultValue
            Assert.Equal(2.340m, range.GetValidValue(2.345m, MidpointRounding.ToEven));
            // 2.345 => 2.350 given range of [0.01 - 10.00] with increment of 0.01 (midpoint rounding away from 0).
            Assert.Equal(2.350m, range.GetValidValue(2.345m, MidpointRounding.AwayFromZero));
            // 9.876 => 9.880 given range of [0.01 - 10.00] with increment of 0.01.
            Assert.Equal(9.880m, range.GetValidValue(9.876m));
        }
    }
}
