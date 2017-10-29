using Binance.Account.Orders;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ClientOrderExtensions
    {
        /// <summary>
        /// Determine if the client order has NOT been placed.
        /// 
        /// NOTE: After successful order placement the 'Timestamp' will be set,
        ///       however if order placement fails or the state is UNKNOWN then
        ///       the timestamp remains 0.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <returns></returns>
        public static bool IsNotPlaced(this ClientOrder clientOrder)
        {
            return clientOrder.Timestamp == 0;
        }
    }
}
