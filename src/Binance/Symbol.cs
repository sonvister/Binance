// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static readonly long LastUpdateAt = 1512688771928;

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Symbol BCH_USDT = BCC_USDT;
        public static readonly Symbol BCH_BNB = BCC_BNB;
        public static readonly Symbol BCH_BTC = BCC_BTC;
        public static readonly Symbol BCH_ETH = BCC_ETH;

        // BTC
        public static readonly Symbol ADA_BTC = new Symbol(Asset.ADA, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ADX_BTC = new Symbol(Asset.ADX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol AMB_BTC = new Symbol(Asset.AMB, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ARK_BTC = new Symbol(Asset.ARK, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol ARN_BTC = new Symbol(Asset.ARN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol AST_BTC = new Symbol(Asset.AST, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BAT_BTC = new Symbol(Asset.BAT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BCC_BTC = new Symbol(Asset.BCC, Asset.BTC, 0.00100000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol BCD_BTC = new Symbol(Asset.BCD, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol BCPT_BTC = new Symbol(Asset.BCPT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNB_BTC = new Symbol(Asset.BNB, Asset.BTC, 1.00000000m, 100000.00000000m, 0.00000001m);
        public static readonly Symbol BNT_BTC = new Symbol(Asset.BNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BQX_BTC = new Symbol(Asset.BQX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BTG_BTC = new Symbol(Asset.BTG, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol BTS_BTC = new Symbol(Asset.BTS, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CDT_BTC = new Symbol(Asset.CDT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CMT_BTC = new Symbol(Asset.CMT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CTR_BTC = new Symbol(Asset.CTR, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DASH_BTC = new Symbol(Asset.DASH, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol DGD_BTC = new Symbol(Asset.DGD, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol DLT_BTC = new Symbol(Asset.DLT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DNT_BTC = new Symbol(Asset.DNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENG_BTC = new Symbol(Asset.ENG, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENJ_BTC = new Symbol(Asset.ENJ, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol EOS_BTC = new Symbol(Asset.EOS, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ETC_BTC = new Symbol(Asset.ETC, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ETH_BTC = new Symbol(Asset.ETH, Asset.BTC, 0.00100000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol EVX_BTC = new Symbol(Asset.EVX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol FUEL_BTC = new Symbol(Asset.FUEL, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol LSK_BTC = new Symbol(Asset.LSK, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol LTC_BTC = new Symbol(Asset.LTC, Asset.BTC, 0.01000000m, 100000.00000000m, 0.00000100m);
        public static readonly Symbol MANA_BTC = new Symbol(Asset.MANA, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol PPT_BTC = new Symbol(Asset.PPT, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol QSP_BTC = new Symbol(Asset.QSP, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol TNT_BTC = new Symbol(Asset.TNT, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol TRX_BTC = new Symbol(Asset.TRX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VEN_BTC = new Symbol(Asset.VEN, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VIB_BTC = new Symbol(Asset.VIB, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol WTC_BTC = new Symbol(Asset.WTC, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XMR_BTC = new Symbol(Asset.XMR, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol XRP_BTC = new Symbol(Asset.XRP, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XVG_BTC = new Symbol(Asset.XVG, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XZC_BTC = new Symbol(Asset.XZC, Asset.BTC, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol YOYO_BTC = new Symbol(Asset.YOYO, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ZEC_BTC = new Symbol(Asset.ZEC, Asset.BTC, 0.00100000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ZRX_BTC = new Symbol(Asset.ZRX, Asset.BTC, 1.00000000m, 10000000.00000000m, 0.00000001m);

        // ETH
        public static readonly Symbol ADA_ETH = new Symbol(Asset.ADA, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ADX_ETH = new Symbol(Asset.ADX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol AMB_ETH = new Symbol(Asset.AMB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ARK_ETH = new Symbol(Asset.ARK, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ARN_ETH = new Symbol(Asset.ARN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol AST_ETH = new Symbol(Asset.AST, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol BAT_ETH = new Symbol(Asset.BAT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BCC_ETH = new Symbol(Asset.BCC, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BCD_ETH = new Symbol(Asset.BCD, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BCPT_ETH = new Symbol(Asset.BCPT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNB_ETH = new Symbol(Asset.BNB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol BNT_ETH = new Symbol(Asset.BNT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol BQX_ETH = new Symbol(Asset.BQX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol BTG_ETH = new Symbol(Asset.BTG, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol BTS_ETH = new Symbol(Asset.BTS, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CDT_ETH = new Symbol(Asset.CDT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CMT_ETH = new Symbol(Asset.CMT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol CTR_ETH = new Symbol(Asset.CTR, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol DASH_ETH = new Symbol(Asset.DASH, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol DGD_ETH = new Symbol(Asset.DGD, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol DLT_ETH = new Symbol(Asset.DLT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol DNT_ETH = new Symbol(Asset.DNT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ENG_ETH = new Symbol(Asset.ENG, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol ENJ_ETH = new Symbol(Asset.ENJ, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol EOS_ETH = new Symbol(Asset.EOS, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol ETC_ETH = new Symbol(Asset.ETC, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol EVX_ETH = new Symbol(Asset.EVX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000010m);
        public static readonly Symbol FUEL_ETH = new Symbol(Asset.FUEL, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol LSK_ETH = new Symbol(Asset.LSK, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol MANA_ETH = new Symbol(Asset.MANA, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol PPT_ETH = new Symbol(Asset.PPT, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol QSP_ETH = new Symbol(Asset.QSP, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
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
        public static readonly Symbol TNT_ETH = new Symbol(Asset.TNT, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol TRX_ETH = new Symbol(Asset.TRX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VEN_ETH = new Symbol(Asset.VEN, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol VIB_ETH = new Symbol(Asset.VIB, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol WTC_ETH = new Symbol(Asset.WTC, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol XMR_ETH = new Symbol(Asset.XMR, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol XRP_ETH = new Symbol(Asset.XRP, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XVG_ETH = new Symbol(Asset.XVG, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol XZC_ETH = new Symbol(Asset.XZC, Asset.ETH, 0.01000000m, 10000000.00000000m, 0.00000100m);
        public static readonly Symbol YOYO_ETH = new Symbol(Asset.YOYO, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);
        public static readonly Symbol ZEC_ETH = new Symbol(Asset.ZEC, Asset.ETH, 0.00100000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol ZRX_ETH = new Symbol(Asset.ZRX, Asset.ETH, 1.00000000m, 10000000.00000000m, 0.00000001m);

        // BNB
        public static readonly Symbol ADX_BNB = new Symbol(Asset.ADX, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol AMB_BNB = new Symbol(Asset.AMB, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BAT_BNB = new Symbol(Asset.BAT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BCC_BNB = new Symbol(Asset.BCC, Asset.BNB, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol BCPT_BNB = new Symbol(Asset.BCPT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol BTS_BNB = new Symbol(Asset.BTS, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol CMT_BNB = new Symbol(Asset.CMT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol DLT_BNB = new Symbol(Asset.DLT, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol IOTA_BNB = new Symbol(Asset.IOTA, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol LSK_BNB = new Symbol(Asset.LSK, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol NEO_BNB = new Symbol(Asset.NEO, Asset.BNB, 0.00100000m, 10000000.00000000m, 0.00100000m);
        public static readonly Symbol NULS_BNB = new Symbol(Asset.NULS, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol POWR_BNB = new Symbol(Asset.POWR, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol QSP_BNB = new Symbol(Asset.QSP, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol RCN_BNB = new Symbol(Asset.RCN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol RDN_BNB = new Symbol(Asset.RDN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);
        public static readonly Symbol VEN_BNB = new Symbol(Asset.VEN, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol WTC_BNB = new Symbol(Asset.WTC, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol XZC_BNB = new Symbol(Asset.XZC, Asset.BNB, 0.00100000m, 10000000.00000000m, 0.00100000m);
        public static readonly Symbol YOYO_BNB = new Symbol(Asset.YOYO, Asset.BNB, 0.01000000m, 10000000.00000000m, 0.00001000m);

        // USDT
        public static readonly Symbol BCC_USDT = new Symbol(Asset.BCC, Asset.USDT, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol BNB_USDT = new Symbol(Asset.BNB, Asset.USDT, 0.01000000m, 10000000.00000000m, 0.00010000m);
        public static readonly Symbol BTC_USDT = new Symbol(Asset.BTC, Asset.USDT, 0.00000100m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol ETH_USDT = new Symbol(Asset.ETH, Asset.USDT, 0.00001000m, 10000000.00000000m, 0.01000000m);
        public static readonly Symbol NEO_USDT = new Symbol(Asset.NEO, Asset.USDT, 0.00100000m, 10000000.00000000m, 0.00100000m);

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Symbol x, Symbol y) => Equals(x, y);

        public static bool operator !=(Symbol x, Symbol y) => !(x == y);

        public static implicit operator string(Symbol symbol) => symbol.ToString();

        public static implicit operator Symbol(string s)
        {
            var _s = s.FormatSymbol();
            return Cache.ContainsKey(_s) ? Cache[_s] : null;
        }

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static readonly IDictionary<string, Symbol> Cache = new Dictionary<string, Symbol>()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            { "BCHUSDT", BCC_USDT },
            { "BCHBNB", BCC_BNB },
            { "BCHBTC", BCC_BTC },
            { "BCHETH", BCC_ETH },

            { "ADABTC", ADA_BTC },
            { "ADAETH", ADA_ETH },
            { "ADXBNB", ADX_BNB },
            { "ADXBTC", ADX_BTC },
            { "ADXETH", ADX_ETH },
            { "AMBBNB", AMB_BNB },
            { "AMBBTC", AMB_BTC },
            { "AMBETH", AMB_ETH },
            { "ARKBTC", ARK_BTC },
            { "ARKETH", ARK_ETH },
            { "ARNBTC", ARN_BTC },
            { "ARNETH", ARN_ETH },
            { "ASTBTC", AST_BTC },
            { "ASTETH", AST_ETH },
            { "BATBNB", BAT_BNB },
            { "BATBTC", BAT_BTC },
            { "BATETH", BAT_ETH },
            { "BCCBNB", BCC_BNB },
            { "BCCBTC", BCC_BTC },
            { "BCCETH", BCC_ETH },
            { "BCCUSDT", BCC_USDT },
            { "BCDBTC", BCD_BTC },
            { "BCDETH", BCD_ETH },
            { "BCPTBNB", BCPT_BNB },
            { "BCPTBTC", BCPT_BTC },
            { "BCPTETH", BCPT_ETH },
            { "BNBBTC", BNB_BTC },
            { "BNBETH", BNB_ETH },
            { "BNBUSDT", BNB_USDT },
            { "BNTBTC", BNT_BTC },
            { "BNTETH", BNT_ETH },
            { "BQXBTC", BQX_BTC },
            { "BQXETH", BQX_ETH },
            { "BTCUSDT", BTC_USDT },
            { "BTGBTC", BTG_BTC },
            { "BTGETH", BTG_ETH },
            { "BTSBNB", BTS_BNB },
            { "BTSBTC", BTS_BTC },
            { "BTSETH", BTS_ETH },
            { "CDTBTC", CDT_BTC },
            { "CDTETH", CDT_ETH },
            { "CMTBNB", CMT_BNB },
            { "CMTBTC", CMT_BTC },
            { "CMTETH", CMT_ETH },
            { "CTRBTC", CTR_BTC },
            { "CTRETH", CTR_ETH },
            { "DASHBTC", DASH_BTC },
            { "DASHETH", DASH_ETH },
            { "DGDBTC", DGD_BTC },
            { "DGDETH", DGD_ETH },
            { "DLTBNB", DLT_BNB },
            { "DLTBTC", DLT_BTC },
            { "DLTETH", DLT_ETH },
            { "DNTBTC", DNT_BTC },
            { "DNTETH", DNT_ETH },
            { "ENGBTC", ENG_BTC },
            { "ENGETH", ENG_ETH },
            { "ENJBTC", ENJ_BTC },
            { "ENJETH", ENJ_ETH },
            { "EOSBTC", EOS_BTC },
            { "EOSETH", EOS_ETH },
            { "ETCBTC", ETC_BTC },
            { "ETCETH", ETC_ETH },
            { "ETHBTC", ETH_BTC },
            { "ETHUSDT", ETH_USDT },
            { "EVXBTC", EVX_BTC },
            { "EVXETH", EVX_ETH },
            { "FUELBTC", FUEL_BTC },
            { "FUELETH", FUEL_ETH },
            { "FUNBTC", FUN_BTC },
            { "FUNETH", FUN_ETH },
            { "GASBTC", GAS_BTC },
            { "GVTBTC", GVT_BTC },
            { "GVTETH", GVT_ETH },
            { "GXSBTC", GXS_BTC },
            { "GXSETH", GXS_ETH },
            { "HSRBTC", HSR_BTC },
            { "HSRETH", HSR_ETH },
            { "ICNBTC", ICN_BTC },
            { "ICNETH", ICN_ETH },
            { "IOTABNB", IOTA_BNB },
            { "IOTABTC", IOTA_BTC },
            { "IOTAETH", IOTA_ETH },
            { "KMDBTC", KMD_BTC },
            { "KMDETH", KMD_ETH },
            { "KNCBTC", KNC_BTC },
            { "KNCETH", KNC_ETH },
            { "LINKBTC", LINK_BTC },
            { "LINKETH", LINK_ETH },
            { "LRCBTC", LRC_BTC },
            { "LRCETH", LRC_ETH },
            { "LSKBNB", LSK_BNB },
            { "LSKBTC", LSK_BTC },
            { "LSKETH", LSK_ETH },
            { "LTCBTC", LTC_BTC },
            { "MANABTC", MANA_BTC },
            { "MANAETH", MANA_ETH },
            { "MCOBTC", MCO_BTC },
            { "MCOETH", MCO_ETH },
            { "MDABTC", MDA_BTC },
            { "MDAETH", MDA_ETH },
            { "MODBTC", MOD_BTC },
            { "MODETH", MOD_ETH },
            { "MTHBTC", MTH_BTC },
            { "MTHETH", MTH_ETH },
            { "MTLBTC", MTL_BTC },
            { "MTLETH", MTL_ETH },
            { "NEOBNB", NEO_BNB },
            { "NEOBTC", NEO_BTC },
            { "NEOETH", NEO_ETH },
            { "NEOUSDT", NEO_USDT },
            { "NULSBNB", NULS_BNB },
            { "NULSBTC", NULS_BTC },
            { "NULSETH", NULS_ETH },
            { "OAXBTC", OAX_BTC },
            { "OAXETH", OAX_ETH },
            { "OMGBTC", OMG_BTC },
            { "OMGETH", OMG_ETH },
            { "POEBTC", POE_BTC },
            { "POEETH", POE_ETH },
            { "POWRBNB", POWR_BNB },
            { "POWRBTC", POWR_BTC },
            { "POWRETH", POWR_ETH },
            { "PPTBTC", PPT_BTC },
            { "PPTETH", PPT_ETH },
            { "QSPBNB", QSP_BNB },
            { "QSPBTC", QSP_BTC },
            { "QSPETH", QSP_ETH },
            { "QTUMBTC", QTUM_BTC },
            { "QTUMETH", QTUM_ETH },
            { "RCNBNB", RCN_BNB },
            { "RCNBTC", RCN_BTC },
            { "RCNETH", RCN_ETH },
            { "RDNBNB", RDN_BNB },
            { "RDNBTC", RDN_BTC },
            { "RDNETH", RDN_ETH },
            { "REQBTC", REQ_BTC },
            { "REQETH", REQ_ETH },
            { "SALTBTC", SALT_BTC },
            { "SALTETH", SALT_ETH },
            { "SNGLSBTC", SNGLS_BTC },
            { "SNGLSETH", SNGLS_ETH },
            { "SNMBTC", SNM_BTC },
            { "SNMETH", SNM_ETH },
            { "SNTBTC", SNT_BTC },
            { "SNTETH", SNT_ETH },
            { "STORJBTC", STORJ_BTC },
            { "STORJETH", STORJ_ETH },
            { "STRATBTC", STRAT_BTC },
            { "STRATETH", STRAT_ETH },
            { "SUBBTC", SUB_BTC },
            { "SUBETH", SUB_ETH },
            { "TNTBTC", TNT_BTC },
            { "TNTETH", TNT_ETH },
            { "TRXBTC", TRX_BTC },
            { "TRXETH", TRX_ETH },
            { "VENBNB", VEN_BNB },
            { "VENBTC", VEN_BTC },
            { "VENETH", VEN_ETH },
            { "VIBBTC", VIB_BTC },
            { "VIBETH", VIB_ETH },
            { "WTCBNB", WTC_BNB },
            { "WTCBTC", WTC_BTC },
            { "WTCETH", WTC_ETH },
            { "XMRBTC", XMR_BTC },
            { "XMRETH", XMR_ETH },
            { "XRPBTC", XRP_BTC },
            { "XRPETH", XRP_ETH },
            { "XVGBTC", XVG_BTC },
            { "XVGETH", XVG_ETH },
            { "XZCBNB", XZC_BNB },
            { "XZCBTC", XZC_BTC },
            { "XZCETH", XZC_ETH },
            { "YOYOBNB", YOYO_BNB },
            { "YOYOBTC", YOYO_BTC },
            { "YOYOETH", YOYO_ETH },
            { "ZECBTC", ZEC_BTC },
            { "ZECETH", ZEC_ETH },
            { "ZRXBTC", ZRX_BTC },
            { "ZRXETH", ZRX_ETH },
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

        private static readonly object _sync = new object();

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

        /// <summary>
        /// Update the symbol cache.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        public static void UpdateCache(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Remove any old symbols (preserves redirections).
                foreach (var symbol in Cache.Values.ToArray())
                {
                    if (!symbols.Contains(symbol))
                    {
                        Cache.Remove(symbol);
                    }
                }

                // Update existing and add any new symbols.
                foreach (var symbol in symbols)
                {
                    Cache[symbol] = symbol;
                }
            }
        }

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
