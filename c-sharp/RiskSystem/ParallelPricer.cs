using HmxLabs.TechTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HmxLabs.TechTest.RiskSystem
{
    public class ParallelPricer
    {
        private const string PluginFolderPath = @".\plugins\";
        private readonly object ResultLock = new object();

        public ParallelPricer()
        {
            LoadPricers();
        }

        // Synchronous wrapper for async method
        public void PriceTrades(IEnumerable<IEnumerable<ITrade>> tradeContainers_, IScalarResultReceiver resultReceiver_)
        {
            Task.Run(() => PriceTradesInParallel(tradeContainers_, resultReceiver_)).GetAwaiter().GetResult();
        }

        // Asynchronous method to price trades in parallel
        private async Task PriceTradesInParallel(IEnumerable<IEnumerable<ITrade>> tradeContainers_, IScalarResultReceiver resultReceiver_)
        {
            var pricingTasks = tradeContainers_.SelectMany(tc => tc).Select(trade => PriceTradeAsync(trade, resultReceiver_));
            await Task.WhenAll(pricingTasks);
        }

        private async Task PriceTradeAsync(ITrade trade_, IScalarResultReceiver resultReceiver_)
        {
            if (!_pricers.ContainsKey(trade_.TradeType))
            {
                await UpdateResultReceiver(new ScalarResult(trade_.TradeId, null, "No Pricing Engines available for this trade type"), resultReceiver_);
                return;
            }

            var pricer = _pricers[trade_.TradeType];

            var resultsHolder = new ScalarResults();
            await Task.Run(() => pricer.Price(trade_, resultsHolder));

            await UpdateResultReceiver(resultsHolder.First(), resultReceiver_);
        }

        // Thread-safe method to update the result receiver
        private async Task UpdateResultReceiver(ScalarResult result_, IScalarResultReceiver resultReceiver_)
        {
            lock (ResultLock)
            {
                if (!string.IsNullOrEmpty(result_.Error))
                    resultReceiver_.AddError(result_.TradeId, result_.Error);
                if (result_.Result.HasValue)
                    resultReceiver_.AddResult(result_.TradeId, result_.Result.Value);
            }
        }

        private void LoadPricers()
        {
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @".\PricingConfig\PricingEngines.xml" };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                var pricingEngine = LoadPricingEngineFromAssembly(configItem);
                _pricers.Add(configItem.TradeType, pricingEngine);
            }
        }

        private IPricingEngine LoadPricingEngineFromAssembly(PricingEngineConfigItem configItem_)
        {
            if (string.IsNullOrEmpty(configItem_.Assembly) || string.IsNullOrEmpty(configItem_.TypeName))
                throw new ArgumentException($"PricingEngineConfig missing assembly/type name: Assembly {configItem_.Assembly}, TypeName {configItem_.TypeName}");

            // The only part we need to load the dll is after the final .
            var dllName = configItem_.Assembly.Split('.').Last();

            var assemblyAddress = PluginFolderPath + dllName + ".dll";
            var assembly = Assembly.LoadFrom(assemblyAddress);

            var pricerType = assembly.GetType(configItem_.TypeName);
            if (pricerType is null)
                throw new TypeLoadException($"Could not load {configItem_.TypeName} from {configItem_.Assembly}");

            // This should throw an error instead of returning null
            var instance = Activator.CreateInstance(pricerType);

            if (!(instance is IPricingEngine))
                throw new InvalidCastException($"Loaded object was not of type {nameof(IPricingEngine)}.");

            return instance as IPricingEngine;
        }

        private readonly Dictionary<string, IPricingEngine> _pricers = new Dictionary<string, IPricingEngine>();
    }


}
