using System;

// ReSharper disable once CheckNamespace
namespace Binance
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
            Throw.IfNull(clientOrder, nameof(clientOrder));

            return clientOrder.Time == default;
        }

        /// <summary>
        /// Determine if client order is valid using cached symbol information.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <returns></returns>
        public static bool IsValid(this ClientOrder clientOrder)
        {
            Throw.IfNull(clientOrder, nameof(clientOrder));

            Symbol symbol = clientOrder.Symbol; // use implicit conversion.

            if (symbol == null)
                return false;

            return symbol.IsValid(clientOrder);
        }

        /// <summary>
        /// Determine if client order is valid using cached symbol information.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <returns></returns>
        public static void Validate(this ClientOrder clientOrder)
        {
            Throw.IfNull(clientOrder, nameof(clientOrder));

            Symbol symbol = clientOrder.Symbol; // use implicit conversion.

            if (symbol == null)
                throw new ArgumentException($"The symbol ({clientOrder.Symbol}) is not recognized.", nameof(clientOrder.Symbol));

            symbol.Validate(clientOrder);
        }
    }
}
