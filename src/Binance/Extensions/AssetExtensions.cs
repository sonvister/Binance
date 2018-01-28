// ReSharper disable once CheckNamespace
using System;

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

            return amount > 0
                && amount % precision == 0;
        }
    }
}
