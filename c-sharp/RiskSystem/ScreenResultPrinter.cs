using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ScreenResultPrinter
    {
        public void PrintResults(ScalarResults results_)
        {
            foreach (var result in results_)
                Console.WriteLine(GetOutputLine(result));
        }

        private string GetOutputLine(ScalarResult result_)
        {
            // By definition of scalar result it cannot have both null result and null error.
            if (null == result_.Result)
                return $"{result_.TradeId} : {result_.Error}";

            if (null == result_.Error)
                return $"{result_.TradeId} : {result_.Result}";

            return $"{result_.TradeId} : {result_.Result} : {result_.Error}";
        }
    }
}