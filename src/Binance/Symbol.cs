// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using Binance.Account.Orders;

namespace Binance
{
    /// <summary>
    /// Defined symbols (for convenience/reference only).
    /// </summary>
    public sealed class Symbol : IComparable<Symbol>, IEquatable<Symbol>
    {
        #region Public Constants

        /// <summary>
        /// When the symbols (currency pairs) were last updated.
        /// </summary>
        public static readonly long LastUpdateAt = 1516052328797;

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Symbol BCH_USDT;
        public static readonly Symbol BCH_BNB;
        public static readonly Symbol BCH_BTC;
        public static readonly Symbol BCH_ETH;

        // BTC
        public static readonly Symbol ADA_BTC = new Symbol(SymbolStatus.Trading, Asset.ADA, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ADX_BTC = new Symbol(SymbolStatus.Trading, Asset.ADX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AION_BTC = new Symbol(SymbolStatus.Trading, Asset.AION, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AMB_BTC = new Symbol(SymbolStatus.Trading, Asset.AMB, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol APPC_BTC = new Symbol(SymbolStatus.Trading, Asset.APPC, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ARK_BTC = new Symbol(SymbolStatus.Trading, Asset.ARK, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ARN_BTC = new Symbol(SymbolStatus.Trading, Asset.ARN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AST_BTC = new Symbol(SymbolStatus.Trading, Asset.AST, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BAT_BTC = new Symbol(SymbolStatus.Trading, Asset.BAT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCC_BTC = new Symbol(SymbolStatus.Trading, Asset.BCC, Asset.BTC, (0.00100000m, 100000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCD_BTC = new Symbol(SymbolStatus.Trading, Asset.BCD, Asset.BTC, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCPT_BTC = new Symbol(SymbolStatus.Trading, Asset.BCPT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BNB_BTC = new Symbol(SymbolStatus.Trading, Asset.BNB, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BNT_BTC = new Symbol(SymbolStatus.Trading, Asset.BNT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BQX_BTC = new Symbol(SymbolStatus.Trading, Asset.BQX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BRD_BTC = new Symbol(SymbolStatus.Trading, Asset.BRD, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTG_BTC = new Symbol(SymbolStatus.Trading, Asset.BTG, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTS_BTC = new Symbol(SymbolStatus.Trading, Asset.BTS, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CDT_BTC = new Symbol(SymbolStatus.Trading, Asset.CDT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CMT_BTC = new Symbol(SymbolStatus.Trading, Asset.CMT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CND_BTC = new Symbol(SymbolStatus.Trading, Asset.CND, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CTR_BTC = new Symbol(SymbolStatus.Trading, Asset.CTR, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DASH_BTC = new Symbol(SymbolStatus.Trading, Asset.DASH, Asset.BTC, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DGD_BTC = new Symbol(SymbolStatus.Trading, Asset.DGD, Asset.BTC, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DLT_BTC = new Symbol(SymbolStatus.Trading, Asset.DLT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DNT_BTC = new Symbol(SymbolStatus.Trading, Asset.DNT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EDO_BTC = new Symbol(SymbolStatus.Trading, Asset.EDO, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ELF_BTC = new Symbol(SymbolStatus.Trading, Asset.ELF, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ENG_BTC = new Symbol(SymbolStatus.Trading, Asset.ENG, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ENJ_BTC = new Symbol(SymbolStatus.Trading, Asset.ENJ, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EOS_BTC = new Symbol(SymbolStatus.Trading, Asset.EOS, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ETC_BTC = new Symbol(SymbolStatus.Trading, Asset.ETC, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ETH_BTC = new Symbol(SymbolStatus.Trading, Asset.ETH, Asset.BTC, (0.00100000m, 100000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EVX_BTC = new Symbol(SymbolStatus.Trading, Asset.EVX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol FUEL_BTC = new Symbol(SymbolStatus.Trading, Asset.FUEL, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol FUN_BTC = new Symbol(SymbolStatus.Trading, Asset.FUN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GAS_BTC = new Symbol(SymbolStatus.Trading, Asset.GAS, Asset.BTC, (0.01000000m, 100000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GTO_BTC = new Symbol(SymbolStatus.Trading, Asset.GTO, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GVT_BTC = new Symbol(SymbolStatus.Trading, Asset.GVT, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GXS_BTC = new Symbol(SymbolStatus.Trading, Asset.GXS, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol HSR_BTC = new Symbol(SymbolStatus.Trading, Asset.HSR, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ICN_BTC = new Symbol(SymbolStatus.Trading, Asset.ICN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ICX_BTC = new Symbol(SymbolStatus.Trading, Asset.ICX, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol INS_BTC = new Symbol(SymbolStatus.Trading, Asset.INS, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol IOTA_BTC = new Symbol(SymbolStatus.Trading, Asset.IOTA, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol KMD_BTC = new Symbol(SymbolStatus.Trading, Asset.KMD, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol KNC_BTC = new Symbol(SymbolStatus.Trading, Asset.KNC, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LEND_BTC = new Symbol(SymbolStatus.Trading, Asset.LEND, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LINK_BTC = new Symbol(SymbolStatus.Trading, Asset.LINK, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LRC_BTC = new Symbol(SymbolStatus.Trading, Asset.LRC, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LSK_BTC = new Symbol(SymbolStatus.Trading, Asset.LSK, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LTC_BTC = new Symbol(SymbolStatus.Trading, Asset.LTC, Asset.BTC, (0.01000000m, 100000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LUN_BTC = new Symbol(SymbolStatus.Trading, Asset.LUN, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MANA_BTC = new Symbol(SymbolStatus.Trading, Asset.MANA, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MCO_BTC = new Symbol(SymbolStatus.Trading, Asset.MCO, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MDA_BTC = new Symbol(SymbolStatus.Trading, Asset.MDA, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MOD_BTC = new Symbol(SymbolStatus.Trading, Asset.MOD, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MTH_BTC = new Symbol(SymbolStatus.Trading, Asset.MTH, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MTL_BTC = new Symbol(SymbolStatus.Trading, Asset.MTL, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NAV_BTC = new Symbol(SymbolStatus.Trading, Asset.NAV, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEBL_BTC = new Symbol(SymbolStatus.Trading, Asset.NEBL, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEO_BTC = new Symbol(SymbolStatus.Trading, Asset.NEO, Asset.BTC, (0.01000000m, 100000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NULS_BTC = new Symbol(SymbolStatus.Trading, Asset.NULS, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OAX_BTC = new Symbol(SymbolStatus.Trading, Asset.OAX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OMG_BTC = new Symbol(SymbolStatus.Trading, Asset.OMG, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OST_BTC = new Symbol(SymbolStatus.Trading, Asset.OST, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol POE_BTC = new Symbol(SymbolStatus.Trading, Asset.POE, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol POWR_BTC = new Symbol(SymbolStatus.Trading, Asset.POWR, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol PPT_BTC = new Symbol(SymbolStatus.Trading, Asset.PPT, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol QSP_BTC = new Symbol(SymbolStatus.Trading, Asset.QSP, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol QTUM_BTC = new Symbol(SymbolStatus.Trading, Asset.QTUM, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RCN_BTC = new Symbol(SymbolStatus.Trading, Asset.RCN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RDN_BTC = new Symbol(SymbolStatus.Trading, Asset.RDN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol REQ_BTC = new Symbol(SymbolStatus.Trading, Asset.REQ, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RLC_BTC = new Symbol(SymbolStatus.Trading, Asset.RLC, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SALT_BTC = new Symbol(SymbolStatus.Trading, Asset.SALT, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNGLS_BTC = new Symbol(SymbolStatus.Trading, Asset.SNGLS, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNM_BTC = new Symbol(SymbolStatus.Trading, Asset.SNM, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNT_BTC = new Symbol(SymbolStatus.Trading, Asset.SNT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol STORJ_BTC = new Symbol(SymbolStatus.Trading, Asset.STORJ, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol STRAT_BTC = new Symbol(SymbolStatus.Trading, Asset.STRAT, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SUB_BTC = new Symbol(SymbolStatus.Trading, Asset.SUB, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TNB_BTC = new Symbol(SymbolStatus.Trading, Asset.TNB, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TNT_BTC = new Symbol(SymbolStatus.Trading, Asset.TNT, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TRIG_BTC = new Symbol(SymbolStatus.Trading, Asset.TRIG, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TRX_BTC = new Symbol(SymbolStatus.Trading, Asset.TRX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VEN_BTC = new Symbol(SymbolStatus.Trading, Asset.VEN, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VIB_BTC = new Symbol(SymbolStatus.Trading, Asset.VIB, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VIBE_BTC = new Symbol(SymbolStatus.Trading, Asset.VIBE, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WABI_BTC = new Symbol(SymbolStatus.Trading, Asset.WABI, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WAVES_BTC = new Symbol(SymbolStatus.Trading, Asset.WAVES, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WINGS_BTC = new Symbol(SymbolStatus.Trading, Asset.WINGS, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WTC_BTC = new Symbol(SymbolStatus.Trading, Asset.WTC, Asset.BTC, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XLM_BTC = new Symbol(SymbolStatus.Trading, Asset.XLM, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XMR_BTC = new Symbol(SymbolStatus.Trading, Asset.XMR, Asset.BTC, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XRP_BTC = new Symbol(SymbolStatus.Trading, Asset.XRP, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XVG_BTC = new Symbol(SymbolStatus.Trading, Asset.XVG, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XZC_BTC = new Symbol(SymbolStatus.Trading, Asset.XZC, Asset.BTC, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol YOYO_BTC = new Symbol(SymbolStatus.Trading, Asset.YOYO, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ZEC_BTC = new Symbol(SymbolStatus.Trading, Asset.ZEC, Asset.BTC, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.00100000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ZRX_BTC = new Symbol(SymbolStatus.Trading, Asset.ZRX, Asset.BTC, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.00200000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});

        // ETH
        public static readonly Symbol ADA_ETH = new Symbol(SymbolStatus.Trading, Asset.ADA, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ADX_ETH = new Symbol(SymbolStatus.Trading, Asset.ADX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AION_ETH = new Symbol(SymbolStatus.Trading, Asset.AION, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AMB_ETH = new Symbol(SymbolStatus.Trading, Asset.AMB, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol APPC_ETH = new Symbol(SymbolStatus.Trading, Asset.APPC, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ARK_ETH = new Symbol(SymbolStatus.Trading, Asset.ARK, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ARN_ETH = new Symbol(SymbolStatus.Trading, Asset.ARN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AST_ETH = new Symbol(SymbolStatus.Trading, Asset.AST, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BAT_ETH = new Symbol(SymbolStatus.Trading, Asset.BAT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCC_ETH = new Symbol(SymbolStatus.Trading, Asset.BCC, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCD_ETH = new Symbol(SymbolStatus.Trading, Asset.BCD, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCPT_ETH = new Symbol(SymbolStatus.Trading, Asset.BCPT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BNB_ETH = new Symbol(SymbolStatus.Trading, Asset.BNB, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BNT_ETH = new Symbol(SymbolStatus.Trading, Asset.BNT, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BQX_ETH = new Symbol(SymbolStatus.Trading, Asset.BQX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BRD_ETH = new Symbol(SymbolStatus.Trading, Asset.BRD, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTG_ETH = new Symbol(SymbolStatus.Trading, Asset.BTG, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTS_ETH = new Symbol(SymbolStatus.Trading, Asset.BTS, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CDT_ETH = new Symbol(SymbolStatus.Trading, Asset.CDT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CMT_ETH = new Symbol(SymbolStatus.Trading, Asset.CMT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CND_ETH = new Symbol(SymbolStatus.Trading, Asset.CND, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CTR_ETH = new Symbol(SymbolStatus.Trading, Asset.CTR, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DASH_ETH = new Symbol(SymbolStatus.Trading, Asset.DASH, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DGD_ETH = new Symbol(SymbolStatus.Trading, Asset.DGD, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DLT_ETH = new Symbol(SymbolStatus.Trading, Asset.DLT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DNT_ETH = new Symbol(SymbolStatus.Trading, Asset.DNT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EDO_ETH = new Symbol(SymbolStatus.Trading, Asset.EDO, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ELF_ETH = new Symbol(SymbolStatus.Trading, Asset.ELF, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ENG_ETH = new Symbol(SymbolStatus.Trading, Asset.ENG, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ENJ_ETH = new Symbol(SymbolStatus.Trading, Asset.ENJ, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EOS_ETH = new Symbol(SymbolStatus.Trading, Asset.EOS, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ETC_ETH = new Symbol(SymbolStatus.Trading, Asset.ETC, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol EVX_ETH = new Symbol(SymbolStatus.Trading, Asset.EVX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol FUEL_ETH = new Symbol(SymbolStatus.Trading, Asset.FUEL, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol FUN_ETH = new Symbol(SymbolStatus.Trading, Asset.FUN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GTO_ETH = new Symbol(SymbolStatus.Trading, Asset.GTO, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GVT_ETH = new Symbol(SymbolStatus.Trading, Asset.GVT, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GXS_ETH = new Symbol(SymbolStatus.Trading, Asset.GXS, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol HSR_ETH = new Symbol(SymbolStatus.Trading, Asset.HSR, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ICN_ETH = new Symbol(SymbolStatus.Trading, Asset.ICN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ICX_ETH = new Symbol(SymbolStatus.Trading, Asset.ICX, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol INS_ETH = new Symbol(SymbolStatus.Trading, Asset.INS, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol IOTA_ETH = new Symbol(SymbolStatus.Trading, Asset.IOTA, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol KMD_ETH = new Symbol(SymbolStatus.Trading, Asset.KMD, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol KNC_ETH = new Symbol(SymbolStatus.Trading, Asset.KNC, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LEND_ETH = new Symbol(SymbolStatus.Trading, Asset.LEND, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LINK_ETH = new Symbol(SymbolStatus.Trading, Asset.LINK, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LRC_ETH = new Symbol(SymbolStatus.Trading, Asset.LRC, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LSK_ETH = new Symbol(SymbolStatus.Trading, Asset.LSK, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LTC_ETH = new Symbol(SymbolStatus.Trading, Asset.LTC, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LUN_ETH = new Symbol(SymbolStatus.Trading, Asset.LUN, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MANA_ETH = new Symbol(SymbolStatus.Trading, Asset.MANA, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MCO_ETH = new Symbol(SymbolStatus.Trading, Asset.MCO, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MDA_ETH = new Symbol(SymbolStatus.Trading, Asset.MDA, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MOD_ETH = new Symbol(SymbolStatus.Trading, Asset.MOD, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MTH_ETH = new Symbol(SymbolStatus.Trading, Asset.MTH, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MTL_ETH = new Symbol(SymbolStatus.Trading, Asset.MTL, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NAV_ETH = new Symbol(SymbolStatus.Trading, Asset.NAV, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEBL_ETH = new Symbol(SymbolStatus.Trading, Asset.NEBL, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEO_ETH = new Symbol(SymbolStatus.Trading, Asset.NEO, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NULS_ETH = new Symbol(SymbolStatus.Trading, Asset.NULS, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OAX_ETH = new Symbol(SymbolStatus.Trading, Asset.OAX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OMG_ETH = new Symbol(SymbolStatus.Trading, Asset.OMG, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OST_ETH = new Symbol(SymbolStatus.Trading, Asset.OST, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol POE_ETH = new Symbol(SymbolStatus.Trading, Asset.POE, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol POWR_ETH = new Symbol(SymbolStatus.Trading, Asset.POWR, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol PPT_ETH = new Symbol(SymbolStatus.Trading, Asset.PPT, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol QSP_ETH = new Symbol(SymbolStatus.Trading, Asset.QSP, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol QTUM_ETH = new Symbol(SymbolStatus.Trading, Asset.QTUM, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RCN_ETH = new Symbol(SymbolStatus.Trading, Asset.RCN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RDN_ETH = new Symbol(SymbolStatus.Trading, Asset.RDN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol REQ_ETH = new Symbol(SymbolStatus.Trading, Asset.REQ, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RLC_ETH = new Symbol(SymbolStatus.Trading, Asset.RLC, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SALT_ETH = new Symbol(SymbolStatus.Trading, Asset.SALT, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNGLS_ETH = new Symbol(SymbolStatus.Trading, Asset.SNGLS, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNM_ETH = new Symbol(SymbolStatus.Trading, Asset.SNM, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SNT_ETH = new Symbol(SymbolStatus.Trading, Asset.SNT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol STORJ_ETH = new Symbol(SymbolStatus.Trading, Asset.STORJ, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol STRAT_ETH = new Symbol(SymbolStatus.Trading, Asset.STRAT, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol SUB_ETH = new Symbol(SymbolStatus.Trading, Asset.SUB, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TNB_ETH = new Symbol(SymbolStatus.Trading, Asset.TNB, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TNT_ETH = new Symbol(SymbolStatus.Trading, Asset.TNT, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TRIG_ETH = new Symbol(SymbolStatus.Trading, Asset.TRIG, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TRX_ETH = new Symbol(SymbolStatus.Trading, Asset.TRX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VEN_ETH = new Symbol(SymbolStatus.Trading, Asset.VEN, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VIBE_ETH = new Symbol(SymbolStatus.Trading, Asset.VIBE, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VIB_ETH = new Symbol(SymbolStatus.Trading, Asset.VIB, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WABI_ETH = new Symbol(SymbolStatus.Trading, Asset.WABI, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WAVES_ETH = new Symbol(SymbolStatus.Trading, Asset.WAVES, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WINGS_ETH = new Symbol(SymbolStatus.Trading, Asset.WINGS, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000010m, 100000.00000000m, 0.00000010m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WTC_ETH = new Symbol(SymbolStatus.Trading, Asset.WTC, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XLM_ETH = new Symbol(SymbolStatus.Trading, Asset.XLM, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XMR_ETH = new Symbol(SymbolStatus.Trading, Asset.XMR, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XRP_ETH = new Symbol(SymbolStatus.Trading, Asset.XRP, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XVG_ETH = new Symbol(SymbolStatus.Trading, Asset.XVG, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XZC_ETH = new Symbol(SymbolStatus.Trading, Asset.XZC, Asset.ETH, (0.01000000m, 90000000.00000000m, 0.01000000m), (0.00000100m, 100000.00000000m, 0.00000100m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol YOYO_ETH = new Symbol(SymbolStatus.Trading, Asset.YOYO, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ZEC_ETH = new Symbol(SymbolStatus.Trading, Asset.ZEC, Asset.ETH, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00001000m, 100000.00000000m, 0.00001000m), 0.02000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ZRX_ETH = new Symbol(SymbolStatus.Trading, Asset.ZRX, Asset.ETH, (1.00000000m, 90000000.00000000m, 1.00000000m), (0.00000001m, 100000.00000000m, 0.00000001m), 0.01000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});

        // BNB
        public static readonly Symbol ADX_BNB = new Symbol(SymbolStatus.Trading, Asset.ADX, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AION_BNB = new Symbol(SymbolStatus.Trading, Asset.AION, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol AMB_BNB = new Symbol(SymbolStatus.Trading, Asset.AMB, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol APPC_BNB = new Symbol(SymbolStatus.Trading, Asset.APPC, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BAT_BNB = new Symbol(SymbolStatus.Trading, Asset.BAT, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCC_BNB = new Symbol(SymbolStatus.Trading, Asset.BCC, Asset.BNB, (0.00001000m, 10000000.00000000m, 0.00001000m), (0.01000000m, 100000.00000000m, 0.01000000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BCPT_BNB = new Symbol(SymbolStatus.Trading, Asset.BCPT, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BRD_BNB = new Symbol(SymbolStatus.Trading, Asset.BRD, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTS_BNB = new Symbol(SymbolStatus.Trading, Asset.BTS, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CMT_BNB = new Symbol(SymbolStatus.Trading, Asset.CMT, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol CND_BNB = new Symbol(SymbolStatus.Trading, Asset.CND, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol DLT_BNB = new Symbol(SymbolStatus.Trading, Asset.DLT, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol GTO_BNB = new Symbol(SymbolStatus.Trading, Asset.GTO, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ICX_BNB = new Symbol(SymbolStatus.Trading, Asset.ICX, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol IOTA_BNB = new Symbol(SymbolStatus.Trading, Asset.IOTA, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LSK_BNB = new Symbol(SymbolStatus.Trading, Asset.LSK, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00010000m, 100000.00000000m, 0.00010000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LTC_BNB = new Symbol(SymbolStatus.Trading, Asset.LTC, Asset.BNB, (0.00001000m, 10000000.00000000m, 0.00001000m), (0.01000000m, 100000.00000000m, 0.01000000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol MCO_BNB = new Symbol(SymbolStatus.Trading, Asset.MCO, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NAV_BNB = new Symbol(SymbolStatus.Trading, Asset.NAV, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEBL_BNB = new Symbol(SymbolStatus.Trading, Asset.NEBL, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEO_BNB = new Symbol(SymbolStatus.Trading, Asset.NEO, Asset.BNB, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00100000m, 10000000.00000000m, 0.00100000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NULS_BNB = new Symbol(SymbolStatus.Trading, Asset.NULS, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol OST_BNB = new Symbol(SymbolStatus.Trading, Asset.OST, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol POWR_BNB = new Symbol(SymbolStatus.Trading, Asset.POWR, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol QSP_BNB = new Symbol(SymbolStatus.Trading, Asset.QSP, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RCN_BNB = new Symbol(SymbolStatus.Trading, Asset.RCN, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RDN_BNB = new Symbol(SymbolStatus.Trading, Asset.RDN, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol RLC_BNB = new Symbol(SymbolStatus.Trading, Asset.RLC, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol TRIG_BNB = new Symbol(SymbolStatus.Trading, Asset.TRIG, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol VEN_BNB = new Symbol(SymbolStatus.Trading, Asset.VEN, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00010000m, 100000.00000000m, 0.00010000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WABI_BNB = new Symbol(SymbolStatus.Trading, Asset.WABI, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WAVES_BNB = new Symbol(SymbolStatus.Trading, Asset.WAVES, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00010000m, 100000.00000000m, 0.00010000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol WTC_BNB = new Symbol(SymbolStatus.Trading, Asset.WTC, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00010000m, 100000.00000000m, 0.00010000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XLM_BNB = new Symbol(SymbolStatus.Trading, Asset.XLM, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol XZC_BNB = new Symbol(SymbolStatus.Trading, Asset.XZC, Asset.BNB, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00100000m, 10000000.00000000m, 0.00100000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol YOYO_BNB = new Symbol(SymbolStatus.Trading, Asset.YOYO, Asset.BNB, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00001000m, 10000.00000000m, 0.00001000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});

        // USDT
        public static readonly Symbol BCC_USDT = new Symbol(SymbolStatus.Trading, Asset.BCC, Asset.USDT, (0.00001000m, 10000000.00000000m, 0.00001000m), (0.01000000m, 10000000.00000000m, 0.01000000m), 20.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BNB_USDT = new Symbol(SymbolStatus.Trading, Asset.BNB, Asset.USDT, (0.01000000m, 10000000.00000000m, 0.01000000m), (0.00010000m, 100000.00000000m, 0.00010000m), 10.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol BTC_USDT = new Symbol(SymbolStatus.Trading, Asset.BTC, Asset.USDT, (0.00000100m, 10000000.00000000m, 0.00000100m), (0.01000000m, 10000000.00000000m, 0.01000000m), 1.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol ETH_USDT = new Symbol(SymbolStatus.Trading, Asset.ETH, Asset.USDT, (0.00001000m, 10000000.00000000m, 0.00001000m), (0.01000000m, 10000000.00000000m, 0.01000000m), 20.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol LTC_USDT = new Symbol(SymbolStatus.Trading, Asset.LTC, Asset.USDT, (0.00001000m, 10000000.00000000m, 0.00001000m), (0.01000000m, 10000000.00000000m, 0.01000000m), 20.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});
        public static readonly Symbol NEO_USDT = new Symbol(SymbolStatus.Trading, Asset.NEO, Asset.USDT, (0.00100000m, 10000000.00000000m, 0.00100000m), (0.00100000m, 10000000.00000000m, 0.00100000m), 10.00000000m, true, new [] {OrderType.Limit,OrderType.LimitMaker,OrderType.Market,OrderType.StopLossLimit,OrderType.TakeProfitLimit});

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Symbol x, Symbol y) => Equals(x, y);

        public static bool operator !=(Symbol x, Symbol y) => !(x == y);

        public static implicit operator string(Symbol symbol) => symbol.ToString();

        public static implicit operator Symbol(string s)
        {
            var _s = s.FormatSymbol();
            lock (_sync)
            {
                return Cache.ContainsKey(_s) ? Cache[_s] : null;
            }
        }

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static readonly IDictionary<string, Symbol> Cache = new Dictionary<string, Symbol>
        {
            { "ADABTC", ADA_BTC },
            { "ADAETH", ADA_ETH },
            { "ADXBNB", ADX_BNB },
            { "ADXBTC", ADX_BTC },
            { "ADXETH", ADX_ETH },
            { "AIONBNB", AION_BNB },
            { "AIONBTC", AION_BTC },
            { "AIONETH", AION_ETH },
            { "AMBBNB", AMB_BNB },
            { "AMBBTC", AMB_BTC },
            { "AMBETH", AMB_ETH },
            { "APPCBNB", APPC_BNB },
            { "APPCBTC", APPC_BTC },
            { "APPCETH", APPC_ETH },
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
            { "BRDBNB", BRD_BNB },
            { "BRDBTC", BRD_BTC },
            { "BRDETH", BRD_ETH },
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
            { "CNDBNB", CND_BNB },
            { "CNDBTC", CND_BTC },
            { "CNDETH", CND_ETH },
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
            { "EDOBTC", EDO_BTC },
            { "EDOETH", EDO_ETH },
            { "ELFBTC", ELF_BTC },
            { "ELFETH", ELF_ETH },
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
            { "GTOBNB", GTO_BNB },
            { "GTOBTC", GTO_BTC },
            { "GTOETH", GTO_ETH },
            { "GVTBTC", GVT_BTC },
            { "GVTETH", GVT_ETH },
            { "GXSBTC", GXS_BTC },
            { "GXSETH", GXS_ETH },
            { "HSRBTC", HSR_BTC },
            { "HSRETH", HSR_ETH },
            { "ICNBTC", ICN_BTC },
            { "ICNETH", ICN_ETH },
            { "ICXBNB", ICX_BNB },
            { "ICXBTC", ICX_BTC },
            { "ICXETH", ICX_ETH },
            { "INSBTC", INS_BTC },
            { "INSETH", INS_ETH },
            { "IOTABNB", IOTA_BNB },
            { "IOTABTC", IOTA_BTC },
            { "IOTAETH", IOTA_ETH },
            { "KMDBTC", KMD_BTC },
            { "KMDETH", KMD_ETH },
            { "KNCBTC", KNC_BTC },
            { "KNCETH", KNC_ETH },
            { "LENDBTC", LEND_BTC },
            { "LENDETH", LEND_ETH },
            { "LINKBTC", LINK_BTC },
            { "LINKETH", LINK_ETH },
            { "LRCBTC", LRC_BTC },
            { "LRCETH", LRC_ETH },
            { "LSKBNB", LSK_BNB },
            { "LSKBTC", LSK_BTC },
            { "LSKETH", LSK_ETH },
            { "LTCBNB", LTC_BNB },
            { "LTCBTC", LTC_BTC },
            { "LTCETH", LTC_ETH },
            { "LTCUSDT", LTC_USDT },
            { "LUNBTC", LUN_BTC },
            { "LUNETH", LUN_ETH },
            { "MANABTC", MANA_BTC },
            { "MANAETH", MANA_ETH },
            { "MCOBNB", MCO_BNB },
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
            { "NAVBNB", NAV_BNB },
            { "NAVBTC", NAV_BTC },
            { "NAVETH", NAV_ETH },
            { "NEBLBNB", NEBL_BNB },
            { "NEBLBTC", NEBL_BTC },
            { "NEBLETH", NEBL_ETH },
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
            { "OSTBNB", OST_BNB },
            { "OSTBTC", OST_BTC },
            { "OSTETH", OST_ETH },
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
            { "RLCBNB", RLC_BNB },
            { "RLCBTC", RLC_BTC },
            { "RLCETH", RLC_ETH },
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
            { "TNBBTC", TNB_BTC },
            { "TNBETH", TNB_ETH },
            { "TNTBTC", TNT_BTC },
            { "TNTETH", TNT_ETH },
            { "TRIGBNB", TRIG_BNB },
            { "TRIGBTC", TRIG_BTC },
            { "TRIGETH", TRIG_ETH },
            { "TRXBTC", TRX_BTC },
            { "TRXETH", TRX_ETH },
            { "VENBNB", VEN_BNB },
            { "VENBTC", VEN_BTC },
            { "VENETH", VEN_ETH },
            { "VIBBTC", VIB_BTC },
            { "VIBEBTC", VIBE_BTC },
            { "VIBEETH", VIBE_ETH },
            { "VIBETH", VIB_ETH },
            { "WABIBNB", WABI_BNB },
            { "WABIBTC", WABI_BTC },
            { "WABIETH", WABI_ETH },
            { "WAVESBNB", WAVES_BNB },
            { "WAVESBTC", WAVES_BTC },
            { "WAVESETH", WAVES_ETH },
            { "WINGSBTC", WINGS_BTC },
            { "WINGSETH", WINGS_ETH },
            { "WTCBNB", WTC_BNB },
            { "WTCBTC", WTC_BTC },
            { "WTCETH", WTC_ETH },
            { "XLMBNB", XLM_BNB },
            { "XLMBTC", XLM_BTC },
            { "XLMETH", XLM_ETH },
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

            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            { "BCHUSDT", BCC_USDT },
            { "BCHBNB", BCC_BNB },
            { "BCHBTC", BCC_BTC },
            { "BCHETH", BCC_ETH }
        };

        /// <summary>
        /// Get the symbol status.
        /// </summary>
        public SymbolStatus Status { get; }

        /// <summary>
        /// Get the base asset symbol.
        /// </summary>
        public Asset BaseAsset { get; }

        /// <summary>
        /// Get the quote asset symbol.
        /// </summary>
        public Asset QuoteAsset { get; }

        /// <summary>
        /// Get base asset range (min/max quantity and step size).
        /// </summary>
        public InclusiveRange Quantity { get; }

        /// <summary>
        /// Get the quote asset range (min/max price and tick size).
        /// </summary>
        public InclusiveRange Price { get; }

        /// <summary>
        /// Get the minimum notional value.
        /// </summary>
        public decimal NotionalMinimumValue { get; }

        /// <summary>
        /// Get the allowed order types.
        /// </summary>
        public IEnumerable<OrderType> OrderTypes { get; }

        /// <summary>
        /// Get the flag indicating if iceberg orders are allowed.
        /// </summary>
        public bool IsIcebergAllowed { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly string _symbol;

        private static readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        static Symbol()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            BCH_USDT = BCC_USDT;
            BCH_BNB = BCC_BNB;
            BCH_BTC = BCC_BTC;
            BCH_ETH = BCC_ETH;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="status">The symbol status.</param>
        /// <param name="baseAsset">The symbol base asset.</param>
        /// <param name="quoteAsset">The symbol quote asset.</param>
        /// <param name="quantity">The minimum, maximum, and incremental quantity values.</param>
        /// <param name="price">The minimum, maximum, and incremental price values.</param>
        /// <param name="notionalMinimumValue">The minimum notional value.</param>
        /// <param name="isIcebergAllowed">The flag indicating if iceberg orders are allowed.</param>
        /// <param name="orderTypes">The list of allowed order types.</param>
        public Symbol(SymbolStatus status, Asset baseAsset, Asset quoteAsset, InclusiveRange quantity, InclusiveRange price, decimal notionalMinimumValue, bool isIcebergAllowed, IEnumerable<OrderType> orderTypes)
        {
            Throw.IfNull(baseAsset, nameof(baseAsset));
            Throw.IfNull(quoteAsset, nameof(quoteAsset));
            Throw.IfNull(quantity, nameof(quantity));
            Throw.IfNull(price, nameof(price));
            Throw.IfNull(orderTypes, nameof(orderTypes));

            Status = status;

            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;

            Quantity = quantity;
            Price = price;

            NotionalMinimumValue = notionalMinimumValue;
            IsIcebergAllowed = isIcebergAllowed;
            OrderTypes = orderTypes;

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

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Remove any old symbols (preserves redirections).
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in Cache.Values.ToArray())
                {
                    if (!symbols.Contains(symbol))
                    {
                        Cache.Remove(symbol);
                    }
                }

                // Update existing and add any new symbols.
                // ReSharper disable once PossibleMultipleEnumeration
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
            return other == null ? 1 : string.Compare(_symbol, other._symbol, StringComparison.Ordinal);
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
