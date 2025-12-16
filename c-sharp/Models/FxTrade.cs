using System;

namespace HmxLabs.TechTest.Models
{
    public class FxTrade : BaseTrade
    {
        public FxTrade(string tradeId_) : base(tradeId_) { }

        public const string FxSpotTradeType = "FxSpot";
        public const string FxForwardTradeType = "FxFwd";

        public enum FxTradeTypes
        {
            Spot,
            Forward
        }

        public FxTradeTypes FxTradeType { get; set; }
        public override string TradeType
        {
            get 
            { 
                return FxTradeType switch
                {
                    FxTradeTypes.Spot => FxSpotTradeType,
                    FxTradeTypes.Forward => FxForwardTradeType,
                    _ => throw new ArgumentException("Unknown FX trade type", FxTradeType.ToString())
                }; 
            }
        }

        public DateTime ValueDate { get; set; }
    }
}
