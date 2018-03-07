// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace Binance
{
    public enum TimeInForce
    {
        /// <summary>
        /// Good 'til canceled.
        /// </summary>
        GTC,

        /// <summary>
        /// Immediate or cancel.
        /// </summary>
        IOC,

        /// <summary>
        /// Fill or kill.
        /// </summary>
        FOK
    }
}
