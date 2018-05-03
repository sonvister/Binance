using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance.Cache
{
    public sealed class InMemoryAssetCache : IAssetCache
    {
        private readonly IDictionary<string, Asset> _cache;

        private readonly object _sync = new object();

        public InMemoryAssetCache()
        {
            _cache = new Dictionary<string, Asset>();
        }

        public Asset Get(string key)
        {
            if (key == null)
                return null;

            var _key = key.FormatSymbol();

            lock (_sync)
            {
                return _cache.ContainsKey(_key) ? _cache[_key] : null;
            }
        }

        public IEnumerable<Asset> GetAll()
        {
            lock (_sync)
            {
                return _cache.Values;
            }
        }

        public void Set(string key, Asset asset)
        {
            lock (_sync)
            {
                _cache[string.Intern(key)] = asset;
            }
        }

        public void Load(IEnumerable<Asset> assets)
        {
            Throw.IfNull(assets, nameof(assets));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!assets.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(assets));

            lock (_sync)
            {
                // Remove any old assets (preserves redirections).
                foreach (var asset in _cache.Values.ToArray())
                {
                    if (!assets.Contains(asset))
                    {
                        _cache.Remove(asset);
                    }
                }

                // Update existing and add any new assets.
                foreach (var asset in assets)
                {
                    _cache[string.Intern(asset)] = asset;
                }
            }
        }
    }
}
