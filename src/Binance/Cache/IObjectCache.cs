using System.Collections.Generic;

namespace Binance.Cache
{
    /// <summary>
    /// A non-expiring object cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectCache<T>
    {
        /// <summary>
        /// Remove all items from the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Get an item with matching key or null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get(string key);

        /// <summary>
        /// Get all the items in the cache.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Add/Replace an item by key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        void Set(string key, T item);

        /// <summary>
        /// Add/Replace multiple items using the ToString() value as the key.
        /// </summary>
        /// <param name="items"></param>
        void Set(IEnumerable<T> items);
    }
}
