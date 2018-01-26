// ReSharper disable once CheckNamespace
namespace Binance.Account.Orders
{
    public static class ClientOrderExtensions
    {
        /// <summary>
        /// Determine if the client order has NOT been placed.
        /// 
        /// NOTE: After successful order placement the 'Time' will be set,
        ///       however if order placement fails or the state is UNKNOWN then
        ///       the time remains the default value.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <returns></returns>
        public static bool IsNotPlaced(this ClientOrder clientOrder)
        {
            return clientOrder.Time == default;
        }
    }
}
