// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance
{
    /// <summary>
    /// Defined assets (for convenience/reference only).
    /// </summary>
    public sealed class Asset : IComparable<Asset>, IEquatable<Asset>
    {
        #region Public Constants

        /// <summary>
        /// When the assets were last updated.
        /// </summary>
        public static readonly long LastUpdateAt = 1518638933816;

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Asset BCH;

        public static readonly Asset ADA;
        public static readonly Asset ADX;
        public static readonly Asset AE;
        public static readonly Asset AION;
        public static readonly Asset AMB;
        public static readonly Asset APPC;
        public static readonly Asset ARK;
        public static readonly Asset ARN;
        public static readonly Asset AST;
        public static readonly Asset BAT;
        public static readonly Asset BCC;
        public static readonly Asset BCD;
        public static readonly Asset BCPT;
        public static readonly Asset BLZ;
        public static readonly Asset BNB;
        public static readonly Asset BNT;
        public static readonly Asset BQX;
        public static readonly Asset BRD;
        public static readonly Asset BTC;
        public static readonly Asset BTG;
        public static readonly Asset BTS;
        public static readonly Asset CDT;
        public static readonly Asset CHAT;
        public static readonly Asset CMT;
        public static readonly Asset CND;
        public static readonly Asset CTR;
        public static readonly Asset DASH;
        public static readonly Asset DGD;
        public static readonly Asset DLT;
        public static readonly Asset DNT;
        public static readonly Asset EDO;
        public static readonly Asset ELF;
        public static readonly Asset ENG;
        public static readonly Asset ENJ;
        public static readonly Asset EOS;
        public static readonly Asset ETC;
        public static readonly Asset ETH;
        public static readonly Asset EVX;
        public static readonly Asset FUEL;
        public static readonly Asset FUN;
        public static readonly Asset GAS;
        public static readonly Asset GTO;
        public static readonly Asset GVT;
        public static readonly Asset GXS;
        public static readonly Asset HSR;
        public static readonly Asset ICN;
        public static readonly Asset ICX;
        public static readonly Asset INS;
        public static readonly Asset IOST;
        public static readonly Asset IOTA;
        public static readonly Asset KMD;
        public static readonly Asset KNC;
        public static readonly Asset LEND;
        public static readonly Asset LINK;
        public static readonly Asset LRC;
        public static readonly Asset LSK;
        public static readonly Asset LTC;
        public static readonly Asset LUN;
        public static readonly Asset MANA;
        public static readonly Asset MCO;
        public static readonly Asset MDA;
        public static readonly Asset MOD;
        public static readonly Asset MTH;
        public static readonly Asset MTL;
        public static readonly Asset NANO;
        public static readonly Asset NAV;
        public static readonly Asset NEBL;
        public static readonly Asset NEO;
        public static readonly Asset NULS;
        public static readonly Asset OAX;
        public static readonly Asset OMG;
        public static readonly Asset OST;
        public static readonly Asset PIVX;
        public static readonly Asset POE;
        public static readonly Asset POWR;
        public static readonly Asset PPT;
        public static readonly Asset QSP;
        public static readonly Asset QTUM;
        public static readonly Asset RCN;
        public static readonly Asset RDN;
        public static readonly Asset REQ;
        public static readonly Asset RLC;
        public static readonly Asset RPX;
        public static readonly Asset SALT;
        public static readonly Asset SNGLS;
        public static readonly Asset SNM;
        public static readonly Asset SNT;
        public static readonly Asset STEEM;
        public static readonly Asset STORJ;
        public static readonly Asset STRAT;
        public static readonly Asset SUB;
        public static readonly Asset TNB;
        public static readonly Asset TNT;
        public static readonly Asset TRIG;
        public static readonly Asset TRX;
        public static readonly Asset USDT;
        public static readonly Asset VEN;
        public static readonly Asset VIA;
        public static readonly Asset VIB;
        public static readonly Asset VIBE;
        public static readonly Asset WABI;
        public static readonly Asset WAVES;
        public static readonly Asset WINGS;
        public static readonly Asset WTC;
        public static readonly Asset XLM;
        public static readonly Asset XMR;
        public static readonly Asset XRP;
        public static readonly Asset XVG;
        public static readonly Asset XZC;
        public static readonly Asset YOYO;
        public static readonly Asset ZEC;
        public static readonly Asset ZRX;

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Asset x, Asset y) => Equals(x, y);

        public static bool operator !=(Asset x, Asset y) => !(x == y);

        public static implicit operator string(Asset asset) => asset?.ToString();

        public static implicit operator Asset(string s)
        {
            if (s == null) return null;
            var _s = s.FormatSymbol();
            lock (_sync)
            {
                return Cache.ContainsKey(_s) ? Cache[_s] : null;
            }
        }

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Asset cache.
        /// </summary>
        public static IDictionary<string, Asset> Cache { get; }

        /// <summary>
        /// Get the asset symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the asset precision.
        /// </summary>
        public int Precision { get; }

        #endregion Public Properties

        #region Private Fields

        private static readonly object _sync;

        #endregion Private Fields

        #region Constructors

        static Asset()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            BCH = BCC;

            ADA = new Asset("ADA", 8);
            ADX = new Asset("ADX", 8);
            AE = new Asset("AE", 8);
            AION = new Asset("AION", 8);
            AMB = new Asset("AMB", 8);
            APPC = new Asset("APPC", 8);
            ARK = new Asset("ARK", 8);
            ARN = new Asset("ARN", 8);
            AST = new Asset("AST", 8);
            BAT = new Asset("BAT", 8);
            BCC = new Asset("BCC", 8);
            BCD = new Asset("BCD", 8);
            BCPT = new Asset("BCPT", 8);
            BLZ = new Asset("BLZ", 8);
            BNB = new Asset("BNB", 8);
            BNT = new Asset("BNT", 8);
            BQX = new Asset("BQX", 8);
            BRD = new Asset("BRD", 8);
            BTC = new Asset("BTC", 8);
            BTG = new Asset("BTG", 8);
            BTS = new Asset("BTS", 8);
            CDT = new Asset("CDT", 8);
            CHAT = new Asset("CHAT", 8);
            CMT = new Asset("CMT", 8);
            CND = new Asset("CND", 8);
            CTR = new Asset("CTR", 8);
            DASH = new Asset("DASH", 8);
            DGD = new Asset("DGD", 8);
            DLT = new Asset("DLT", 8);
            DNT = new Asset("DNT", 8);
            EDO = new Asset("EDO", 8);
            ELF = new Asset("ELF", 8);
            ENG = new Asset("ENG", 8);
            ENJ = new Asset("ENJ", 8);
            EOS = new Asset("EOS", 8);
            ETC = new Asset("ETC", 8);
            ETH = new Asset("ETH", 8);
            EVX = new Asset("EVX", 8);
            FUEL = new Asset("FUEL", 8);
            FUN = new Asset("FUN", 8);
            GAS = new Asset("GAS", 8);
            GTO = new Asset("GTO", 8);
            GVT = new Asset("GVT", 8);
            GXS = new Asset("GXS", 8);
            HSR = new Asset("HSR", 8);
            ICN = new Asset("ICN", 8);
            ICX = new Asset("ICX", 8);
            INS = new Asset("INS", 8);
            IOST = new Asset("IOST", 8);
            IOTA = new Asset("IOTA", 8);
            KMD = new Asset("KMD", 8);
            KNC = new Asset("KNC", 8);
            LEND = new Asset("LEND", 8);
            LINK = new Asset("LINK", 8);
            LRC = new Asset("LRC", 8);
            LSK = new Asset("LSK", 8);
            LTC = new Asset("LTC", 8);
            LUN = new Asset("LUN", 8);
            MANA = new Asset("MANA", 8);
            MCO = new Asset("MCO", 8);
            MDA = new Asset("MDA", 8);
            MOD = new Asset("MOD", 8);
            MTH = new Asset("MTH", 8);
            MTL = new Asset("MTL", 8);
            NANO = new Asset("NANO", 8);
            NAV = new Asset("NAV", 8);
            NEBL = new Asset("NEBL", 8);
            NEO = new Asset("NEO", 8);
            NULS = new Asset("NULS", 8);
            OAX = new Asset("OAX", 8);
            OMG = new Asset("OMG", 8);
            OST = new Asset("OST", 8);
            PIVX = new Asset("PIVX", 8);
            POE = new Asset("POE", 8);
            POWR = new Asset("POWR", 8);
            PPT = new Asset("PPT", 8);
            QSP = new Asset("QSP", 8);
            QTUM = new Asset("QTUM", 8);
            RCN = new Asset("RCN", 8);
            RDN = new Asset("RDN", 8);
            REQ = new Asset("REQ", 8);
            RLC = new Asset("RLC", 8);
            RPX = new Asset("RPX", 8);
            SALT = new Asset("SALT", 8);
            SNGLS = new Asset("SNGLS", 8);
            SNM = new Asset("SNM", 8);
            SNT = new Asset("SNT", 8);
            STEEM = new Asset("STEEM", 8);
            STORJ = new Asset("STORJ", 8);
            STRAT = new Asset("STRAT", 8);
            SUB = new Asset("SUB", 8);
            TNB = new Asset("TNB", 8);
            TNT = new Asset("TNT", 8);
            TRIG = new Asset("TRIG", 8);
            TRX = new Asset("TRX", 8);
            USDT = new Asset("USDT", 8);
            VEN = new Asset("VEN", 8);
            VIA = new Asset("VIA", 8);
            VIB = new Asset("VIB", 8);
            VIBE = new Asset("VIBE", 8);
            WABI = new Asset("WABI", 8);
            WAVES = new Asset("WAVES", 8);
            WINGS = new Asset("WINGS", 8);
            WTC = new Asset("WTC", 8);
            XLM = new Asset("XLM", 8);
            XMR = new Asset("XMR", 8);
            XRP = new Asset("XRP", 8);
            XVG = new Asset("XVG", 8);
            XZC = new Asset("XZC", 8);
            YOYO = new Asset("YOYO", 8);
            ZEC = new Asset("ZEC", 8);
            ZRX = new Asset("ZRX", 8);

            _sync = new object();

            Cache = new Dictionary<string, Asset>
            {
                { "ADA", ADA },
                { "ADX", ADX },
                { "AE", AE },
                { "AION", AION },
                { "AMB", AMB },
                { "APPC", APPC },
                { "ARK", ARK },
                { "ARN", ARN },
                { "AST", AST },
                { "BAT", BAT },
                { "BCC", BCC },
                { "BCD", BCD },
                { "BCPT", BCPT },
                { "BLZ", BLZ },
                { "BNB", BNB },
                { "BNT", BNT },
                { "BQX", BQX },
                { "BRD", BRD },
                { "BTC", BTC },
                { "BTG", BTG },
                { "BTS", BTS },
                { "CDT", CDT },
                { "CHAT", CHAT },
                { "CMT", CMT },
                { "CND", CND },
                { "CTR", CTR },
                { "DASH", DASH },
                { "DGD", DGD },
                { "DLT", DLT },
                { "DNT", DNT },
                { "EDO", EDO },
                { "ELF", ELF },
                { "ENG", ENG },
                { "ENJ", ENJ },
                { "EOS", EOS },
                { "ETC", ETC },
                { "ETH", ETH },
                { "EVX", EVX },
                { "FUEL", FUEL },
                { "FUN", FUN },
                { "GAS", GAS },
                { "GTO", GTO },
                { "GVT", GVT },
                { "GXS", GXS },
                { "HSR", HSR },
                { "ICN", ICN },
                { "ICX", ICX },
                { "INS", INS },
                { "IOST", IOST },
                { "IOTA", IOTA },
                { "KMD", KMD },
                { "KNC", KNC },
                { "LEND", LEND },
                { "LINK", LINK },
                { "LRC", LRC },
                { "LSK", LSK },
                { "LTC", LTC },
                { "LUN", LUN },
                { "MANA", MANA },
                { "MCO", MCO },
                { "MDA", MDA },
                { "MOD", MOD },
                { "MTH", MTH },
                { "MTL", MTL },
                { "NANO", NANO },
                { "NAV", NAV },
                { "NEBL", NEBL },
                { "NEO", NEO },
                { "NULS", NULS },
                { "OAX", OAX },
                { "OMG", OMG },
                { "OST", OST },
                { "PIVX", PIVX },
                { "POE", POE },
                { "POWR", POWR },
                { "PPT", PPT },
                { "QSP", QSP },
                { "QTUM", QTUM },
                { "RCN", RCN },
                { "RDN", RDN },
                { "REQ", REQ },
                { "RLC", RLC },
                { "RPX", RPX },
                { "SALT", SALT },
                { "SNGLS", SNGLS },
                { "SNM", SNM },
                { "SNT", SNT },
                { "STEEM", STEEM },
                { "STORJ", STORJ },
                { "STRAT", STRAT },
                { "SUB", SUB },
                { "TNB", TNB },
                { "TNT", TNT },
                { "TRIG", TRIG },
                { "TRX", TRX },
                { "USDT", USDT },
                { "VEN", VEN },
                { "VIA", VIA },
                { "VIB", VIB },
                { "VIBE", VIBE },
                { "WABI", WABI },
                { "WAVES", WAVES },
                { "WINGS", WINGS },
                { "WTC", WTC },
                { "XLM", XLM },
                { "XMR", XMR },
                { "XRP", XRP },
                { "XVG", XVG },
                { "XZC", XZC },
                { "YOYO", YOYO },
                { "ZEC", ZEC },
                { "ZRX", ZRX },
            
                // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
                { "BCH", BCC }
            };
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The asset symbol.</param>
        /// <param name="precision">The asset precision.</param>
        public Asset(string symbol, int precision)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentNullException(nameof(symbol));

            if (precision <= 0)
                throw new ArgumentException("Asset precision must be greater than 0.", nameof(precision));

            Symbol = symbol.ToUpperInvariant();
            Precision = precision;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Verify that asset is valid. If fails, but known to be valid,
        /// call Symbol.UpdateCacheAsync() to get the latest assets.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool IsValid(string asset)
        {
            if (string.IsNullOrWhiteSpace(asset))
                return false;

            asset = asset.FormatSymbol();

            lock (_sync)
            {
                return Cache.ContainsKey(asset)
                    && Cache[asset].ToString() == asset;
            }
        }

        public override bool Equals(object obj)
        {
            return Symbol.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Update the asset cache.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        internal static void UpdateCache(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            var assets = new List<Asset>();

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var symbol in symbols)
            {
                if (!assets.Contains(symbol.BaseAsset))
                    assets.Add(symbol.BaseAsset);

                if (!assets.Contains(symbol.QuoteAsset))
                    assets.Add(symbol.QuoteAsset);
            }

            lock (_sync)
            {
                // Remove any old assets (preserves redirections).
                foreach (var asset in Cache.Values.ToArray())
                {
                    if (!assets.Contains(asset))
                    {
                        Cache.Remove(asset);
                    }
                }

                // Update existing and add any new assets.
                foreach (var asset in assets)
                {
                    Cache[asset] = asset;
                }
            }
        }

        #endregion Internal Methods

        #region IComparable<Asset>

        public int CompareTo(Asset other)
        {
            return other == null ? 1 : string.Compare(Symbol, other.Symbol, StringComparison.Ordinal);
        }

        #endregion IComparable<Asset>

        #region IEquatable<Asset>

        public bool Equals(Asset other)
        {
            return CompareTo(other) == 0;
        }

        #endregion IEquatable<Asset>

        // File generated by BinanceCodeGenerator tool.
    }
}
