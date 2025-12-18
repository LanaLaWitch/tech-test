using static HmxLabs.TechTest.Models.FxTrade;

namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        public BondTrade(string tradeId_) : base(tradeId_) { }

        public const string GovBondTradeType = "GovBond";
        public const string CorpBondTradeType = "CorpBond";
        public const string SupraBondTradeType = "SupraBond";

        public enum BondTradeTypes
        {
            Government,
            Corporate,
            Supranational
        }

        public BondTradeTypes BondTradeType { get; set; }

        public override string TradeType
        {
            get
            {
                return BondTradeType switch
                {
                    BondTradeTypes.Government => GovBondTradeType,
                    BondTradeTypes.Corporate => CorpBondTradeType,
                    BondTradeTypes.Supranational => SupraBondTradeType,
                    _ => throw new ArgumentException("Unknown Bond trade type", BondTradeType.ToString())
                };
            }
        }
    }
}
