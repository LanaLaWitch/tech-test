using System;
using System.Collections.Generic;
using System.Reflection;
using HmxLabs.TechTest.Models;

namespace HmxLabs.TechTest.RiskSystem
{
    public class SerialPricer
    {
        private const string PluginFolderPath = @".\plugins\";
       
        public SerialPricer()
        {
            LoadPricers();
        }

        public void Price(IEnumerable<IEnumerable<ITrade>> tradeContainters_, IScalarResultReceiver resultReceiver_)
        {
            foreach (var tradeContainter in tradeContainters_)
            {
                foreach (var trade in tradeContainter)
                {
                    Price(trade, resultReceiver_);
                }
            }
        }

        public void Price(ITrade trade, IScalarResultReceiver resultReceiver_)
        {
            if (!_pricers.ContainsKey(trade.TradeType))
            {
                resultReceiver_.AddError(trade.TradeId, "No Pricing Engines available for this trade type");
                return;
            }

            var pricer = _pricers[trade.TradeType];
            pricer.Price(trade, resultReceiver_);
        }

        private void LoadPricers()
        {
            var pricingConfigLoader = new PricingConfigLoader { ConfigFile = @".\PricingConfig\PricingEngines.xml" };
            var pricerConfig = pricingConfigLoader.LoadConfig();

            foreach (var configItem in pricerConfig)
            {
                try
                {
                    var pricingEngine = LoadPricingEngineFromAssembly(configItem);
                    _pricers.Add(configItem.TradeType, pricingEngine);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Could not load pricing engine for config with trade type {configItem.TradeType}. {ex.ToString()}");
                    continue;
                }
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