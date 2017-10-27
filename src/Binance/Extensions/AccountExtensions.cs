using Binance.Accounts;
using System;
using System.Linq;

namespace Binance
{
    public static class AccountExtensions
    {
        /// <summary>
        /// Get the individual account balance for an asset.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static AccountBalance GetBalance(this Account account, string asset)
        {
            return account.Balances.FirstOrDefault(b => b.Asset.Equals(asset, StringComparison.OrdinalIgnoreCase));
        }
    }
}
