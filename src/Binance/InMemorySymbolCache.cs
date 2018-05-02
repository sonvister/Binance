using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance
{
    public sealed class InMemorySymbolCache : ISymbolCache
    {
        private readonly IDictionary<string, Symbol> _cache;
            
        private readonly object _sync = new object();

        public InMemorySymbolCache()
        {
            _cache = new Dictionary<string, Symbol>();
        }

        public IEnumerable<Symbol> GetAll()
        {
            lock (_sync)
            {
                return _cache.Values;
            }
        }

        public Symbol Get(string key)
        {
            if (key == null)
                return null;

            var _key = key.FormatSymbol();

            lock (_sync)
            {
                return _cache.ContainsKey(_key) ? _cache[_key] : null;
            }
        }

        public void Set(string key, Symbol symbol)
        {
            lock (_sync)
            {
                _cache[string.Intern(key)] = symbol;
            }
        }

        public void Load(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Remove any old symbols (preserves redirections).
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in _cache.Values.ToArray())
                {
                    if (!symbols.Contains(symbol))
                    {
                        _cache.Remove(symbol);
                    }
                }

                // Update existing and add any new symbols.
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in symbols)
                {
                    _cache[string.Intern(symbol)] = symbol;
                }
            }
        }
    }
}
