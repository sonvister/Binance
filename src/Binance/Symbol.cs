// ReSharper disable InconsistentNaming
namespace Binance
{
    /// <summary>
    /// Defined symbols (for convienience/reference only).
    /// </summary>
    public static class Symbol
    {
        #region Public Constants

        /// <summary>
        /// When the symbols (currency pairs) were last updated.
        /// </summary>
        public static readonly long LastUpdateAt = 1510613503259;

        // Redirect Bitcoin Cash (BCC = BitConnect)
        public static readonly string BCH_USDT = "BCCUSDT";
        public static readonly string BCH_BTC = "BCCBTC";
        public static readonly string BCH_ETH = "BCCETH";

        // USDT
        public static readonly string BCC_USDT = "BCCUSDT";
        public static readonly string BNB_USDT = "BNBUSDT";
        public static readonly string BTC_USDT = "BTCUSDT";
        public static readonly string ETH_USDT = "ETHUSDT";

        // BTC
        public static readonly string AMB_BTC = "AMBBTC";
        public static readonly string ARK_BTC = "ARKBTC";
        public static readonly string AST_BTC = "ASTBTC";
        public static readonly string BAT_BTC = "BATBTC";
        public static readonly string BCC_BTC = "BCCBTC";
        public static readonly string BNB_BTC = "BNBBTC";
        public static readonly string BNT_BTC = "BNTBTC";
        public static readonly string BQX_BTC = "BQXBTC";
        public static readonly string BTG_BTC = "BTGBTC";
        public static readonly string CTR_BTC = "CTRBTC";
        public static readonly string DASH_BTC = "DASHBTC";
        public static readonly string DLT_BTC = "DLTBTC";
        public static readonly string DNT_BTC = "DNTBTC";
        public static readonly string ELC_BTC = "ELCBTC";
        public static readonly string ENG_BTC = "ENGBTC";
        public static readonly string ENJ_BTC = "ENJBTC";
        public static readonly string EOS_BTC = "EOSBTC";
        public static readonly string ETC_BTC = "ETCBTC";
        public static readonly string ETH_BTC = "ETHBTC";
        public static readonly string EVX_BTC = "EVXBTC";
        public static readonly string FUN_BTC = "FUNBTC";
        public static readonly string GAS_BTC = "GASBTC";
        public static readonly string HCC_BTC = "HCCBTC";
        public static readonly string HSR_BTC = "HSRBTC";
        public static readonly string ICN_BTC = "ICNBTC";
        public static readonly string IOTA_BTC = "IOTABTC";
        public static readonly string KMD_BTC = "KMDBTC";
        public static readonly string KNC_BTC = "KNCBTC";
        public static readonly string LINK_BTC = "LINKBTC";
        public static readonly string LLT_BTC = "LLTBTC";
        public static readonly string LRC_BTC = "LRCBTC";
        public static readonly string LTC_BTC = "LTCBTC";
        public static readonly string MCO_BTC = "MCOBTC";
        public static readonly string MDA_BTC = "MDABTC";
        public static readonly string MOD_BTC = "MODBTC";
        public static readonly string MTH_BTC = "MTHBTC";
        public static readonly string MTL_BTC = "MTLBTC";
        public static readonly string NEO_BTC = "NEOBTC";
        public static readonly string NULS_BTC = "NULSBTC";
        public static readonly string OAX_BTC = "OAXBTC";
        public static readonly string OMG_BTC = "OMGBTC";
        public static readonly string POWR_BTC = "POWRBTC";
        public static readonly string QTUM_BTC = "QTUMBTC";
        public static readonly string RCN_BTC = "RCNBTC";
        public static readonly string RDN_BTC = "RDNBTC";
        public static readonly string REQ_BTC = "REQBTC";
        public static readonly string SALT_BTC = "SALTBTC";
        public static readonly string SNGLS_BTC = "SNGLSBTC";
        public static readonly string SNM_BTC = "SNMBTC";
        public static readonly string SNT_BTC = "SNTBTC";
        public static readonly string STORJ_BTC = "STORJBTC";
        public static readonly string STRAT_BTC = "STRATBTC";
        public static readonly string SUB_BTC = "SUBBTC";
        public static readonly string TRX_BTC = "TRXBTC";
        public static readonly string VEN_BTC = "VENBTC";
        public static readonly string VIB_BTC = "VIBBTC";
        public static readonly string WTC_BTC = "WTCBTC";
        public static readonly string XMR_BTC = "XMRBTC";
        public static readonly string XRP_BTC = "XRPBTC";
        public static readonly string XVG_BTC = "XVGBTC";
        public static readonly string YOYO_BTC = "YOYOBTC";
        public static readonly string ZEC_BTC = "ZECBTC";
        public static readonly string ZRX_BTC = "ZRXBTC";

        // ETH
        public static readonly string AMB_ETH = "AMBETH";
        public static readonly string ARK_ETH = "ARKETH";
        public static readonly string AST_ETH = "ASTETH";
        public static readonly string BAT_ETH = "BATETH";
        public static readonly string BCC_ETH = "BCCETH";
        public static readonly string BNB_ETH = "BNBETH";
        public static readonly string BNT_ETH = "BNTETH";
        public static readonly string BQX_ETH = "BQXETH";
        public static readonly string BTG_ETH = "BTGETH";
        public static readonly string BTM_ETH = "BTMETH";
        public static readonly string CTR_ETH = "CTRETH";
        public static readonly string DASH_ETH = "DASHETH";
        public static readonly string DLT_ETH = "DLTETH";
        public static readonly string DNT_ETH = "DNTETH";
        public static readonly string ENG_ETH = "ENGETH";
        public static readonly string ENJ_ETH = "ENJETH";
        public static readonly string EOS_ETH = "EOSETH";
        public static readonly string ETC_ETH = "ETCETH";
        public static readonly string EVX_ETH = "EVXETH";
        public static readonly string FUN_ETH = "FUNETH";
        public static readonly string HSR_ETH = "HSRETH";
        public static readonly string ICN_ETH = "ICNETH";
        public static readonly string IOTA_ETH = "IOTAETH";
        public static readonly string KMD_ETH = "KMDETH";
        public static readonly string KNC_ETH = "KNCETH";
        public static readonly string LINK_ETH = "LINKETH";
        public static readonly string LRC_ETH = "LRCETH";
        public static readonly string MCO_ETH = "MCOETH";
        public static readonly string MDA_ETH = "MDAETH";
        public static readonly string MOD_ETH = "MODETH";
        public static readonly string MTH_ETH = "MTHETH";
        public static readonly string MTL_ETH = "MTLETH";
        public static readonly string NEO_ETH = "NEOETH";
        public static readonly string NULS_ETH = "NULSETH";
        public static readonly string OAX_ETH = "OAXETH";
        public static readonly string OMG_ETH = "OMGETH";
        public static readonly string POWR_ETH = "POWRETH";
        public static readonly string QTUM_ETH = "QTUMETH";
        public static readonly string RCN_ETH = "RCNETH";
        public static readonly string RDN_ETH = "RDNETH";
        public static readonly string REQ_ETH = "REQETH";
        public static readonly string SALT_ETH = "SALTETH";
        public static readonly string SNGLS_ETH = "SNGLSETH";
        public static readonly string SNM_ETH = "SNMETH";
        public static readonly string SNT_ETH = "SNTETH";
        public static readonly string STORJ_ETH = "STORJETH";
        public static readonly string STRAT_ETH = "STRATETH";
        public static readonly string SUB_ETH = "SUBETH";
        public static readonly string TRX_ETH = "TRXETH";
        public static readonly string VEN_ETH = "VENETH";
        public static readonly string VIB_ETH = "VIBETH";
        public static readonly string WTC_ETH = "WTCETH";
        public static readonly string XMR_ETH = "XMRETH";
        public static readonly string XRP_ETH = "XRPETH";
        public static readonly string XVG_ETH = "XVGETH";
        public static readonly string YOYO_ETH = "YOYOETH";
        public static readonly string ZEC_ETH = "ZECETH";
        public static readonly string ZRX_ETH = "ZRXETH";

        // BNB
        public static readonly string AMB_BNB = "AMBBNB";
        public static readonly string BAT_BNB = "BATBNB";
        public static readonly string BCC_BNB = "BCCBNB";
        public static readonly string DLT_BNB = "DLTBNB";
        public static readonly string NULS_BNB = "NULSBNB";
        public static readonly string POWR_BNB = "POWRBNB";
        public static readonly string RCN_BNB = "RCNBNB";
        public static readonly string RDN_BNB = "RDNBNB";
        public static readonly string VEN_BNB = "VENBNB";
        public static readonly string WTC_BNB = "WTCBNB";
        public static readonly string YOYO_BNB = "YOYOBNB";

        #endregion Public Constants

        // File generated by BinanceCodeGenerator tool.
    }
}
