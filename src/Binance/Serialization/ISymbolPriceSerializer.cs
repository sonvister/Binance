using System.Collections.Generic;

namespace Binance.Serialization
{
    public interface ISymbolPriceSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="SymbolPrice"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        SymbolPrice Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="SymbolPrice"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<SymbolPrice> DeserializeMany(string json);

        /// <summary>
        /// Serialize a <see cref="SymbolPrice"/> to JSON.
        /// </summary>
        /// <param name="symbolPrice"></param>
        /// <returns></returns>
        string Serialize(SymbolPrice symbolPrice);
    }
}
