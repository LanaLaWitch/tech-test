using System;

namespace HmxLabs.TechTest.Models
{
    public abstract class BaseTrade : ITrade
    {
        public BaseTrade(string tradeId_)
        {
            if (string.IsNullOrWhiteSpace(tradeId_))
            {
                throw new ArgumentException("A valid non null, non empty trade ID must be provided");
            }

            TradeId = tradeId_;
        }

        public DateTime TradeDate { get; set; }

        public string? Counterparty { get; set; }

        public string? Instrument { get; set; }

        public double Notional { get; set; }
            
        public double Rate { get; set; }

        public abstract string TradeType { get; }

        public string TradeId { get; protected set; }
    }
}
