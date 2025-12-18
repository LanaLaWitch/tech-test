using HmxLabs.TechTest.Models;
using HmxLabs.TechTest.RiskSystem;

namespace HmxLabs.TechTest.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args_)
        {
            var tradeLoader = new SerialTradeLoader();
            var results = new ScalarResults();
            var pricer = new SerialPricer();

            tradeLoader.LoadAndPriceTrades(pricer, results);
            //var pricer = new ParallelPricer();

            var screenPrinter = new ScreenResultPrinter();
            screenPrinter.PrintResults(results);

            Console.WriteLine("Press any key to exit..");
            Console.ReadKey();
        }
    }
}