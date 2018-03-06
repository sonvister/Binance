// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface IStopOrder
    {
        /// <summary>
        /// Get or set the stop price.
        /// </summary>
        decimal StopPrice { get; set; }
    }
}
