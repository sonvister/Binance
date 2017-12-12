namespace Binance.Account.Orders
{
    public interface IStopOrder
    {
        /// <summary>
        /// Get or set the stop price.
        /// </summary>
        decimal StopPrice { get; set; }
    }
}
