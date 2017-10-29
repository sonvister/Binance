using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// Exception throwing helpers.
    /// </summary>
    internal static class Throw
    {
        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if argument is null.
        /// </summary>
        /// <param name="argument"></param>
        public static void IfNull(object argument)
        {
            if (argument == null)
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if argument is null.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        public static void IfNull(object argument, string paramName)
        {
            if (argument == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if argument is null.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public static void IfNull(object argument, string message, Exception innerException)
        {
            if (argument == null)
                throw new ArgumentNullException(message, innerException);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if argument is null.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void IfNull(object argument, string paramName, string message)
        {
            if (argument == null)
                throw new ArgumentNullException(paramName, message);
        }


        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or empty.
        /// </summary>
        /// <param name="argument"></param>
        public static void IfNullOrEmpty(string argument)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or empty.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        public static void IfNullOrEmpty(string argument, string paramName)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or empty.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public static void IfNullOrEmpty(string argument, string message, Exception innerException)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(message, innerException);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or empty.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void IfNullOrEmpty(string argument, string paramName, string message)
        {
            if (string.IsNullOrEmpty(argument))
                throw new ArgumentNullException(paramName, message);
        }


        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or white space.
        /// </summary>
        /// <param name="argument"></param>
        public static void IfNullOrWhiteSpace(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentNullException();
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or white space.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        public static void IfNullOrWhiteSpace(string argument, string paramName)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or white space.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public static void IfNullOrWhiteSpace(string argument, string message, Exception innerException)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentNullException(message, innerException);
        }

        /// <summary>
        /// Throw an <see cref="ArgumentNullException"/> if string argument is null or white space.
        /// </summary>
        /// <param name="argument"></param>
        /// <param name="paramName"></param>
        /// <param name="message"></param>
        public static void IfNullOrWhiteSpace(string argument, string paramName, string message)
        {
            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentNullException(paramName, message);
        }
    }
}
