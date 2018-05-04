using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance.Cache
{
    public class InMemoryCache<T> : IObjectCache<T>
    {
        private readonly IDictionary<string, T> _cache;

        private readonly object _sync = new object();

        public InMemoryCache()
        {
            _cache = new Dictionary<string, T>();
        }

        public void Clear()
        {
            lock (_sync)
            {
                _cache.Clear();
            }
        }

        public IEnumerable<T> GetAll()
        {
            lock (_sync)
            {
                return _cache.Values;
            }
        }

        public T Get(string key)
        {
            if (key == null)
                return default;

            var _key = key.FormatSymbol();

            lock (_sync)
            {
                return _cache.ContainsKey(_key) ? _cache[_key] : default;
            }
        }

        public void Set(string key, T symbol)
        {
            lock (_sync)
            {
                _cache[string.Intern(key)] = symbol;
            }
        }

        public void Set(IEnumerable<T> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Update existing and add any new symbols.
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in symbols)
                {
                    _cache[string.Intern(symbol.ToString())] = symbol;
                }
            }
        }
    }
}
