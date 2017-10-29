using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ExceptionExtensions
    {
        private const string IsLoggedKey = "Binance.IsLogged";

        /// <summary>
        /// Return true if this exception has been logged by this library already.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        internal static bool IsLogged (this Exception exception)
        {
            return exception.Data.Contains(IsLoggedKey) && (bool)exception.Data[IsLoggedKey];
        }

        /// <summary>
        /// Set a data property indication this exception has been logged.
        /// </summary>
        /// <param name="exception"></param>
        internal static void Logged(this Exception exception)
        {
            exception.Data[IsLoggedKey] = true;
        }
    }
}
