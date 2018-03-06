using System.Collections.Generic;

namespace Binance.Serialization
{
    public interface IAccountTradeSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="AccountTrade"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        AccountTrade Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="AccountTrade"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IEnumerable<AccountTrade> DeserializeMany(string json, string symbol);

        /// <summary>
        /// Serialize a <see cref="AccountTrade"/> to JSON.
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        string Serialize(AccountTrade trade);
    }
}
