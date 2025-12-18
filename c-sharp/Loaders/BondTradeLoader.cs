using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class BondTradeLoader : ITradeLoader
    {
        private const char Seperator = ',';

        public IEnumerable<ITrade> LoadTrades()
        {
            var tradeList = new BondTradeList();
            LoadTradesFromFile(DataFile, tradeList);

            return tradeList;
        }

        public IEnumerable<ITrade> LoadTradesIndividually()
        {
            var tradeList = new BondTradeList();

            foreach (var trade in LoadTradesFromFileIndividually(DataFile))
            {
                yield return trade;
            }
        }

        public string? DataFile { get; set; }

        private BondTrade CreateTradeFromLine(string line_)
        {
            
            var items = line_.Split(new[] {Seperator});
            var trade = new BondTrade(items[6]);

            // Set fx trade type. This drives the TradeType (string) property of the FxTrade object.
            trade.BondTradeType = items[0] switch
            {
                "GovBond" => BondTrade.BondTradeTypes.Government,
                "CorpBond" => BondTrade.BondTradeTypes.Corporate,
                "Supra" => BondTrade.BondTradeTypes.Supranational,
                _ => throw new InvalidDataException($"Invalid trade type: {items[0]}"),
            };

            trade.TradeDate = DateTime.Parse(items[1]);
            trade.Instrument = items[2];
            trade.Counterparty = items[3];
            trade.Notional = Double.Parse(items[4]);
            trade.Rate = Double.Parse(items[5]);

            return trade;
        }

        private void LoadTradesFromFile(string? filename_, BondTradeList tradeList_)
        {
            if (null == filename_)
                throw new ArgumentNullException(nameof(filename_));
            
            var stream = new StreamReader(filename_);

            using (stream)
            {
                var lineCount = 0;
                while (!stream.EndOfStream)
                {
                    if (0 == lineCount)
                    {
                        stream.ReadLine();
                    }
                    else
                    {
                        tradeList_.Add(CreateTradeFromLine(stream.ReadLine()!));    
                    }
                    lineCount++;
                }
            }
        }

        private IEnumerable<BondTrade> LoadTradesFromFileIndividually(string? filename_)
        {
            if (null == filename_)
                throw new ArgumentNullException(nameof(filename_));
            
            var stream = new StreamReader(filename_);
            using (stream)
            {
                var lineCount = 0;
                while (!stream.EndOfStream)
                {
                    if (0 == lineCount)
                    {
                        stream.ReadLine();
                    }
                    else
                    {
                        yield return CreateTradeFromLine(stream.ReadLine()!);    
                    }
                    lineCount++;
                }
            }
        }
    }
}
