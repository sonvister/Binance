using System.Collections.Generic;

namespace Binance
{
    /// <summary>
    /// A non-expiring object cache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectCache<T>
    {
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
        /// Load the cache, adding new items, replacing existing items,
        /// and removing items that are not contained in the list.
        /// 
        /// NOTE: This does not clear the cache to preseve redirections.
        /// </summary>
        /// <param name="items"></param>
        void Load(IEnumerable<T> items);

        /// <summary>
        /// Add/Replace an item by key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        void Set(string key, T item);
    }
}
