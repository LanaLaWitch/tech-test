namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        public BondTrade(string tradeId_) : base(tradeId_) { }

        public const string GovBondTradeType = "GovBond";
        public const string CorpBondTradeType = "CorpBond";

        public override string TradeType { get { return GovBondTradeType; } }
    }
}
