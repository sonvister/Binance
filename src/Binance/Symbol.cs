// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;

namespace Binance
{
    /// <summary>
    /// Defined symbols (for convienience/reference only).
    /// </summary>
    public sealed class Symbol : IComparable<Symbol>, IEquatable<Symbol>
    {
        #region Public Constants

        /// <summary>
        /// When the symbols (currency pairs) were last updated.
        /// </summary>
        public static readonly long LastUpdateAt = 1511217569515;

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Symbol BCH_USDT = BCC_USDT;
        public static readonly Symbol BCH_BNB = BCC_BNB;
        public static readonly Symbol BCH_BTC = BCC_BTC;
        public static readonly Symbol BCH_ETH = BCC_ETH;

        // BNB
        public static readonly Symbol AMB_BNB = new Symbol(Asset.AMB, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BAT_BNB = new Symbol(Asset.BAT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BCC_BNB = new Symbol(Asset.BCC, Asset.BNB, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol BCPT_BNB = new Symbol(Asset.BCPT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol DLT_BNB = new Symbol(Asset.DLT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol NEO_BNB = new Symbol(Asset.NEO, Asset.BNB, 0.00100000m, 10000000.00000000m, 0.00100000m);
        public static readonly Symbol NULS_BNB = new Symbol(Asset.NULS, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol POWR_BNB = new Symbol(Asset.POWR, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol RCN_BNB = new Symbol(Asset.RCN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol RDN_BNB = new Symbol(Asset.RDN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol VEN_BNB = new Symbol(Asset.VEN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol WTC_BNB = new Symbol(Asset.WTC, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol YOYO_BNB = new Symbol(Asset.YOYO, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);

        // BTC
        public static readonly Symbol AMB_BTC = new Symbol(Asset.AMB, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ARK_BTC = new Symbol(Asset.ARK, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol ARN_BTC = new Symbol(Asset.ARN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol AST_BTC = new Symbol(Asset.AST, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BAT_BTC = new Symbol(Asset.BAT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BCC_BTC = new Symbol(Asset.BCC, Asset.BTC, 0.00100000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol BCPT_BTC = new Symbol(Asset.BCPT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNB_BTC = new Symbol(Asset.BNB, Asset.BTC, 1.00000000m, 100000.00000000m, 0.00000001m);
        public static readonly Symbol BNT_BTC = new Symbol(Asset.BNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BQX_BTC = new Symbol(Asset.BQX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BTG_BTC = new Symbol(Asset.BTG, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol CDT_BTC = new Symbol(Asset.CDT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CTR_BTC = new Symbol(Asset.CTR, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DASH_BTC = new Symbol(Asset.DASH, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol DLT_BTC = new Symbol(Asset.DLT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DNT_BTC = new Symbol(Asset.DNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENG_BTC = new Symbol(Asset.ENG, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENJ_BTC = new Symbol(Asset.ENJ, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol EOS_BTC = new Symbol(Asset.EOS, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ETC_BTC = new Symbol(Asset.ETC, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ETH_BTC = new Symbol(Asset.ETH, Asset.BTC, 0.00100000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol EVX_BTC = new Symbol(Asset.EVX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol FUN_BTC = new Symbol(Asset.FUN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol GAS_BTC = new Symbol(Asset.GAS, Asset.BTC, 0.01000000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol GVT_BTC = new Symbol(Asset.GVT, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol GXS_BTC = new Symbol(Asset.GXS, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol HSR_BTC = new Symbol(Asset.HSR, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ICN_BTC = new Symbol(Asset.ICN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol IOTA_BTC = new Symbol(Asset.IOTA, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol KMD_BTC = new Symbol(Asset.KMD, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol KNC_BTC = new Symbol(Asset.KNC, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol LINK_BTC = new Symbol(Asset.LINK, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol LRC_BTC = new Symbol(Asset.LRC, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol LTC_BTC = new Symbol(Asset.LTC, Asset.BTC, 0.01000000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol MCO_BTC = new Symbol(Asset.MCO, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol MDA_BTC = new Symbol(Asset.MDA, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol MOD_BTC = new Symbol(Asset.MOD, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol MTH_BTC = new Symbol(Asset.MTH, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol MTL_BTC = new Symbol(Asset.MTL, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol NEO_BTC = new Symbol(Asset.NEO, Asset.BTC, 0.01000000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol NULS_BTC = new Symbol(Asset.NULS, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol OAX_BTC = new Symbol(Asset.OAX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol OMG_BTC = new Symbol(Asset.OMG, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol POE_BTC = new Symbol(Asset.POE, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol POWR_BTC = new Symbol(Asset.POWR, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol QTUM_BTC = new Symbol(Asset.QTUM, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol RCN_BTC = new Symbol(Asset.RCN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol RDN_BTC = new Symbol(Asset.RDN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol REQ_BTC = new Symbol(Asset.REQ, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SALT_BTC = new Symbol(Asset.SALT, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol SNGLS_BTC = new Symbol(Asset.SNGLS, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SNM_BTC = new Symbol(Asset.SNM, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SNT_BTC = new Symbol(Asset.SNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol STORJ_BTC = new Symbol(Asset.STORJ, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol STRAT_BTC = new Symbol(Asset.STRAT, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol SUB_BTC = new Symbol(Asset.SUB, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol TRX_BTC = new Symbol(Asset.TRX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VEN_BTC = new Symbol(Asset.VEN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VIB_BTC = new Symbol(Asset.VIB, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol WTC_BTC = new Symbol(Asset.WTC, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XMR_BTC = new Symbol(Asset.XMR, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol XRP_BTC = new Symbol(Asset.XRP, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XVG_BTC = new Symbol(Asset.XVG, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol YOYO_BTC = new Symbol(Asset.YOYO, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ZEC_BTC = new Symbol(Asset.ZEC, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ZRX_BTC = new Symbol(Asset.ZRX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);

        // ETH
        public static readonly Symbol AMB_ETH = new Symbol(Asset.AMB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ARK_ETH = new Symbol(Asset.ARK, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ARN_ETH = new Symbol(Asset.ARN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol AST_ETH = new Symbol(Asset.AST, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol BAT_ETH = new Symbol(Asset.BAT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BCC_ETH = new Symbol(Asset.BCC, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BCPT_ETH = new Symbol(Asset.BCPT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNB_ETH = new Symbol(Asset.BNB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNT_ETH = new Symbol(Asset.BNT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol BQX_ETH = new Symbol(Asset.BQX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol BTG_ETH = new Symbol(Asset.BTG, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol CDT_ETH = new Symbol(Asset.CDT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CTR_ETH = new Symbol(Asset.CTR, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol DASH_ETH = new Symbol(Asset.DASH, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol DLT_ETH = new Symbol(Asset.DLT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DNT_ETH = new Symbol(Asset.DNT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENG_ETH = new Symbol(Asset.ENG, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol ENJ_ETH = new Symbol(Asset.ENJ, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol EOS_ETH = new Symbol(Asset.EOS, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ETC_ETH = new Symbol(Asset.ETC, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol EVX_ETH = new Symbol(Asset.EVX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol FUN_ETH = new Symbol(Asset.FUN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol GVT_ETH = new Symbol(Asset.GVT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol GXS_ETH = new Symbol(Asset.GXS, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol HSR_ETH = new Symbol(Asset.HSR, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ICN_ETH = new Symbol(Asset.ICN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol IOTA_ETH = new Symbol(Asset.IOTA, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol KMD_ETH = new Symbol(Asset.KMD, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol KNC_ETH = new Symbol(Asset.KNC, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol LINK_ETH = new Symbol(Asset.LINK, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol LRC_ETH = new Symbol(Asset.LRC, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol MCO_ETH = new Symbol(Asset.MCO, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol MDA_ETH = new Symbol(Asset.MDA, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol MOD_ETH = new Symbol(Asset.MOD, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol MTH_ETH = new Symbol(Asset.MTH, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol MTL_ETH = new Symbol(Asset.MTL, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol NEO_ETH = new Symbol(Asset.NEO, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol NULS_ETH = new Symbol(Asset.NULS, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol OAX_ETH = new Symbol(Asset.OAX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol OMG_ETH = new Symbol(Asset.OMG, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol POE_ETH = new Symbol(Asset.POE, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol POWR_ETH = new Symbol(Asset.POWR, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol QTUM_ETH = new Symbol(Asset.QTUM, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol RCN_ETH = new Symbol(Asset.RCN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol RDN_ETH = new Symbol(Asset.RDN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol REQ_ETH = new Symbol(Asset.REQ, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SALT_ETH = new Symbol(Asset.SALT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol SNGLS_ETH = new Symbol(Asset.SNGLS, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SNM_ETH = new Symbol(Asset.SNM, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol SNT_ETH = new Symbol(Asset.SNT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol STORJ_ETH = new Symbol(Asset.STORJ, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol STRAT_ETH = new Symbol(Asset.STRAT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol SUB_ETH = new Symbol(Asset.SUB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol TRX_ETH = new Symbol(Asset.TRX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VEN_ETH = new Symbol(Asset.VEN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VIB_ETH = new Symbol(Asset.VIB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol WTC_ETH = new Symbol(Asset.WTC, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol XMR_ETH = new Symbol(Asset.XMR, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol XRP_ETH = new Symbol(Asset.XRP, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XVG_ETH = new Symbol(Asset.XVG, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol YOYO_ETH = new Symbol(Asset.YOYO, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ZEC_ETH = new Symbol(Asset.ZEC, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol ZRX_ETH = new Symbol(Asset.ZRX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);

        // USDT
        public static readonly Symbol BCC_USDT = new Symbol(Asset.BCC, Asset.USDT, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol BNB_USDT = new Symbol(Asset.BNB, Asset.USDT, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol BTC_USDT = new Symbol(Asset.BTC, Asset.USDT, 0.00000100m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol ETH_USDT = new Symbol(Asset.ETH, Asset.USDT, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol NEO_USDT = new Symbol(Asset.NEO, Asset.USDT, 0.00100000m, 10000000.00000000m, 0.00100000m);

        #endregion Public Constants

        #region Implicit Operators

        public static implicit operator string(Symbol symbol) => symbol.ToString();

        public static implicit operator Symbol(string s) => Cache.ContainsKey(s) ? Cache[s] : null;

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static readonly IDictionary<string, Symbol> Cache = new Dictionary<string, Symbol>()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            { "BCH_USDT", BCC_USDT },
            { "BCH_BNB", BCC_BNB },
            { "BCH_BTC", BCC_BTC },
            { "BCH_ETH", BCC_ETH },

            { "AMB_BNB", AMB_BNB },
            { "AMB_BTC", AMB_BTC },
            { "AMB_ETH", AMB_ETH },
            { "ARK_BTC", ARK_BTC },
            { "ARK_ETH", ARK_ETH },
            { "ARN_BTC", ARN_BTC },
            { "ARN_ETH", ARN_ETH },
            { "AST_BTC", AST_BTC },
            { "AST_ETH", AST_ETH },
            { "BAT_BNB", BAT_BNB },
            { "BAT_BTC", BAT_BTC },
            { "BAT_ETH", BAT_ETH },
            { "BCC_BNB", BCC_BNB },
            { "BCC_BTC", BCC_BTC },
            { "BCC_ETH", BCC_ETH },
            { "BCC_USDT", BCC_USDT },
            { "BCPT_BNB", BCPT_BNB },
            { "BCPT_BTC", BCPT_BTC },
            { "BCPT_ETH", BCPT_ETH },
            { "BNB_BTC", BNB_BTC },
            { "BNB_ETH", BNB_ETH },
            { "BNB_USDT", BNB_USDT },
            { "BNT_BTC", BNT_BTC },
            { "BNT_ETH", BNT_ETH },
            { "BQX_BTC", BQX_BTC },
            { "BQX_ETH", BQX_ETH },
            { "BTC_USDT", BTC_USDT },
            { "BTG_BTC", BTG_BTC },
            { "BTG_ETH", BTG_ETH },
            { "CDT_BTC", CDT_BTC },
            { "CDT_ETH", CDT_ETH },
            { "CTR_BTC", CTR_BTC },
            { "CTR_ETH", CTR_ETH },
            { "DASH_BTC", DASH_BTC },
            { "DASH_ETH", DASH_ETH },
            { "DLT_BNB", DLT_BNB },
            { "DLT_BTC", DLT_BTC },
            { "DLT_ETH", DLT_ETH },
            { "DNT_BTC", DNT_BTC },
            { "DNT_ETH", DNT_ETH },
            { "ENG_BTC", ENG_BTC },
            { "ENG_ETH", ENG_ETH },
            { "ENJ_BTC", ENJ_BTC },
            { "ENJ_ETH", ENJ_ETH },
            { "EOS_BTC", EOS_BTC },
            { "EOS_ETH", EOS_ETH },
            { "ETC_BTC", ETC_BTC },
            { "ETC_ETH", ETC_ETH },
            { "ETH_BTC", ETH_BTC },
            { "ETH_USDT", ETH_USDT },
            { "EVX_BTC", EVX_BTC },
            { "EVX_ETH", EVX_ETH },
            { "FUN_BTC", FUN_BTC },
            { "FUN_ETH", FUN_ETH },
            { "GAS_BTC", GAS_BTC },
            { "GVT_BTC", GVT_BTC },
            { "GVT_ETH", GVT_ETH },
            { "GXS_BTC", GXS_BTC },
            { "GXS_ETH", GXS_ETH },
            { "HSR_BTC", HSR_BTC },
            { "HSR_ETH", HSR_ETH },
            { "ICN_BTC", ICN_BTC },
            { "ICN_ETH", ICN_ETH },
            { "IOTA_BTC", IOTA_BTC },
            { "IOTA_ETH", IOTA_ETH },
            { "KMD_BTC", KMD_BTC },
            { "KMD_ETH", KMD_ETH },
            { "KNC_BTC", KNC_BTC },
            { "KNC_ETH", KNC_ETH },
            { "LINK_BTC", LINK_BTC },
            { "LINK_ETH", LINK_ETH },
            { "LRC_BTC", LRC_BTC },
            { "LRC_ETH", LRC_ETH },
            { "LTC_BTC", LTC_BTC },
            { "MCO_BTC", MCO_BTC },
            { "MCO_ETH", MCO_ETH },
            { "MDA_BTC", MDA_BTC },
            { "MDA_ETH", MDA_ETH },
            { "MOD_BTC", MOD_BTC },
            { "MOD_ETH", MOD_ETH },
            { "MTH_BTC", MTH_BTC },
            { "MTH_ETH", MTH_ETH },
            { "MTL_BTC", MTL_BTC },
            { "MTL_ETH", MTL_ETH },
            { "NEO_BNB", NEO_BNB },
            { "NEO_BTC", NEO_BTC },
            { "NEO_ETH", NEO_ETH },
            { "NEO_USDT", NEO_USDT },
            { "NULS_BNB", NULS_BNB },
            { "NULS_BTC", NULS_BTC },
            { "NULS_ETH", NULS_ETH },
            { "OAX_BTC", OAX_BTC },
            { "OAX_ETH", OAX_ETH },
            { "OMG_BTC", OMG_BTC },
            { "OMG_ETH", OMG_ETH },
            { "POE_BTC", POE_BTC },
            { "POE_ETH", POE_ETH },
            { "POWR_BNB", POWR_BNB },
            { "POWR_BTC", POWR_BTC },
            { "POWR_ETH", POWR_ETH },
            { "QTUM_BTC", QTUM_BTC },
            { "QTUM_ETH", QTUM_ETH },
            { "RCN_BNB", RCN_BNB },
            { "RCN_BTC", RCN_BTC },
            { "RCN_ETH", RCN_ETH },
            { "RDN_BNB", RDN_BNB },
            { "RDN_BTC", RDN_BTC },
            { "RDN_ETH", RDN_ETH },
            { "REQ_BTC", REQ_BTC },
            { "REQ_ETH", REQ_ETH },
            { "SALT_BTC", SALT_BTC },
            { "SALT_ETH", SALT_ETH },
            { "SNGLS_BTC", SNGLS_BTC },
            { "SNGLS_ETH", SNGLS_ETH },
            { "SNM_BTC", SNM_BTC },
            { "SNM_ETH", SNM_ETH },
            { "SNT_BTC", SNT_BTC },
            { "SNT_ETH", SNT_ETH },
            { "STORJ_BTC", STORJ_BTC },
            { "STORJ_ETH", STORJ_ETH },
            { "STRAT_BTC", STRAT_BTC },
            { "STRAT_ETH", STRAT_ETH },
            { "SUB_BTC", SUB_BTC },
            { "SUB_ETH", SUB_ETH },
            { "TRX_BTC", TRX_BTC },
            { "TRX_ETH", TRX_ETH },
            { "VEN_BNB", VEN_BNB },
            { "VEN_BTC", VEN_BTC },
            { "VEN_ETH", VEN_ETH },
            { "VIB_BTC", VIB_BTC },
            { "VIB_ETH", VIB_ETH },
            { "WTC_BNB", WTC_BNB },
            { "WTC_BTC", WTC_BTC },
            { "WTC_ETH", WTC_ETH },
            { "XMR_BTC", XMR_BTC },
            { "XMR_ETH", XMR_ETH },
            { "XRP_BTC", XRP_BTC },
            { "XRP_ETH", XRP_ETH },
            { "XVG_BTC", XVG_BTC },
            { "XVG_ETH", XVG_ETH },
            { "YOYO_BNB", YOYO_BNB },
            { "YOYO_BTC", YOYO_BTC },
            { "YOYO_ETH", YOYO_ETH },
            { "ZEC_BTC", ZEC_BTC },
            { "ZEC_ETH", ZEC_ETH },
            { "ZRX_BTC", ZRX_BTC },
            { "ZRX_ETH", ZRX_ETH },
        };

        /// <summary>
        /// Get the base asset symbol.
        /// </summary>
        public Asset BaseAsset { get; }

        /// <summary>
        /// Get the quote asset symbol.
        /// </summary>
        public Asset QuoteAsset { get; }

        /// <summary>
        /// Get the minimum order quantity.
        /// </summary>
        public decimal BaseMinQuantity { get; }

        /// <summary>
        /// Get the maximum order quantity.
        /// </summary>
        public decimal BaseMaxQuantity { get; }

        /// <summary>
        /// Get the minimum order price as well as the price increment.
        /// NOTE: The order price must be a multiple of this increment.
        /// </summary>
        public decimal QuoteIncrement { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly string _symbol;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseAsset">The symbol base asset.</param>
        /// <param name="quoteAsset">The symbol quote asset.</param>
        /// <param name="baseMaxQty">The base asset maximum quantity.</param>
        /// <param name="baseMinQty">The base asset minimum quantity.</param>
        /// <param name="quoteIncrement">The quote asset increment price.</param>
        public Symbol(Asset baseAsset, Asset quoteAsset, decimal baseMinQty, decimal baseMaxQty, decimal quoteIncrement)
        {
            Throw.IfNull(baseAsset, nameof(baseAsset));
            Throw.IfNull(quoteAsset, nameof(quoteAsset));

            if (baseMinQty <= 0)
                throw new ArgumentException($"{nameof(Symbol)} quantity must be greater than 0.", nameof(baseMinQty));
            if (baseMaxQty <= 0)
                throw new ArgumentException($"{nameof(Symbol)} quantity must be greater than 0.", nameof(baseMaxQty));
            if (quoteIncrement <= 0)
                throw new ArgumentException($"{nameof(Symbol)} increment must be greater than 0.", nameof(quoteIncrement));

            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;
            BaseMinQuantity = baseMinQty;
            BaseMaxQuantity = baseMaxQty;
            QuoteIncrement = quoteIncrement;

            _symbol = $"{baseAsset}{quoteAsset}";
        }

        #endregion Constructors

        #region Public Methods

        public override string ToString()
        {
            return _symbol;
        }

        public override bool Equals(object obj)
        {
            return _symbol.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _symbol.GetHashCode();
        }

        #endregion Public Methods

        #region IComparable<Symbol>

        public int CompareTo(Symbol other)
        {
            if (other == null)
                return 1;

            return _symbol.CompareTo(other._symbol);
        }

        #endregion IComparable<Symbol>

        #region IEquatable<Symbol>

        public bool Equals(Symbol other)
        {
            return CompareTo(other) == 0;
        }

        #endregion IEquatable<Symbol>

        // File generated by BinanceCodeGenerator tool.
    }
}
