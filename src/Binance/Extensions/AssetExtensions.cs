using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class AssetExtensions
    {
        /// <summary>
        /// Determine if amount is valid for <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool IsAmountValid(this Asset asset, decimal amount)
        {
            var precision = (decimal)Math.Pow(10, -asset.Precision);

            return amount >= 0
                && amount % precision == 0;
        }

        /// <summary>
        /// Validate amount for <see cref="Asset"/>.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="amount"></param>
        public static void ValidateAmount(this Asset asset, decimal amount, string paramName = null)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(amount), $"Asset amount ({amount}) must be greater than or equal to 0.");

            var precision = (decimal)Math.Pow(10, -asset.Precision);

            if (amount % precision > 0)
                throw new ArgumentOutOfRangeException(paramName ?? nameof(amount), $"Asset amount ({amount}) is too precise ({precision}).");
        }
    }
}
