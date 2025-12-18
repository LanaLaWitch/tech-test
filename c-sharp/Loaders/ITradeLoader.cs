using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.Loaders
{
    public interface ITradeLoader
    {
        IEnumerable<ITrade> LoadTrades();
        IEnumerable<ITrade> LoadTradesIndividually();
        string? DataFile { get; set; }
    }
}