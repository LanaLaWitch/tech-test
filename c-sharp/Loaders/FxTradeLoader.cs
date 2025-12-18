using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public class FxTradeLoader : ITradeLoader
    {
        private const char Separator = '¬';
        private const int ExpectedDataStartIndex = 2; // This value may need to change if data file format changes in the future

        public IEnumerable<ITrade> LoadTrades()
        {
            var trades = LoadTradesFromFile(DataFile);
            return trades;
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

        private IEnumerable<ITrade> LoadTradesFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            var trades = new List<ITrade>();

            var stream = new StreamReader(filePath);
            using (stream)
            {
                var currentReadIndex = 0;
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();

                    // Assert line has at least 3 characters for "END" check
                    if (line.Length < 3)
                    {
                        currentReadIndex++;
                        continue;
                    }

                    // End of data marker
                    if (line.Substring(0, 3) == "END")
                        break;

                    if (currentReadIndex >= ExpectedDataStartIndex)
                    {
                        var trade = CreateTradeFromLine(line);
                        if (trade != null)
                        {
                            trades.Add(trade);
                        }
                    }
                    currentReadIndex++;
                }
            }

            return trades;
        }

        private IEnumerable<FxTrade> LoadTradesFromFileIndividually(string? filename_)
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

        private FxTrade CreateTradeFromLine(string? line)
        {
            if (string.IsNullOrEmpty(line))
                return null;

            var tradeData = line.Split(Separator);
            if (9 != tradeData.Length)
                return null;

            var tradeId = tradeData[8];
            var trade = new FxTrade(tradeId);

            // Set fx trade type. This drives the TradeType (string) property of the FxTrade object.
            trade.FxTradeType = tradeData[0] switch
            {
                "FxSpot" => FxTrade.FxTradeTypes.Spot,
                "FxFwd" => FxTrade.FxTradeTypes.Forward,
                _ => throw new InvalidDataException($"Invalid trade type: {tradeData[0]}"),
            };

            trade.TradeDate = DateTime.Parse(tradeData[1]);
            trade.Instrument = string.Concat(tradeData[2], tradeData[3]);
            trade.Notional = double.Parse(tradeData[4]);
            trade.Rate = double.Parse(tradeData[5]);
            trade.ValueDate = DateTime.Parse(tradeData[6]);
            trade.Counterparty = tradeData[7];
            
            return trade;
        }
    }
}
