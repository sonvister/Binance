using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class AccountInfoExtensions
    {
        /// <summary>
        /// Get the individual account balance for an asset.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static AccountBalance GetBalance(this AccountInfo account, string asset)
        {
            return account.Balances.FirstOrDefault(b => b.Asset.Equals(asset, StringComparison.OrdinalIgnoreCase));
        }
    }
}
