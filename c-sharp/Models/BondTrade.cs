namespace HmxLabs.TechTest.Models
{
    public class BondTrade : BaseTrade
    {
        public BondTrade(string tradeId_) : base(tradeId_) { }
    
        public enum BondTradeTypes
        {
            GovBond,
            CorpBond
        }

        public const string GovBondTradeType = "GovBond";
        public const string CorpBondTradeType = "CorpBond";

        public BondTradeTypes BondTradeType { get; set; }

        public override string TradeType { 
            get 
            {
                return BondTradeType switch
                {
                    BondTradeTypes.GovBond => GovBondTradeType,
                    BondTradeTypes.CorpBond => CorpBondTradeType,
                    _ => throw new ArgumentException("Unknown bond trade type", BondTradeType.ToString())
                };
            } 
        }
    }
}
