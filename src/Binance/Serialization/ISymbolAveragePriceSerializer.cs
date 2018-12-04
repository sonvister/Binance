namespace Binance.Serialization
{
    public interface ISymbolAveragePriceSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="SymbolAveragePrice"/>.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        SymbolAveragePrice Deserialize(string symbol, string json);

        /// <summary>
        /// Serialize a <see cref="SymbolAveragePrice"/> to JSON.
        /// </summary>
        /// <param name="symbolAveragePrice"></param>
        /// <returns></returns>
        string Serialize(SymbolAveragePrice symbolAveragePrice);
    }
}
