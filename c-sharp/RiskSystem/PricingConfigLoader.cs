using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace HmxLabs.TechTest.RiskSystem
{
    public class PricingConfigLoader
    {
        public string? ConfigFile { get; set; }

        public PricingEngineConfig LoadConfig()
        {
            if (string.IsNullOrEmpty(ConfigFile))
                throw new ArgumentException("ConfigFile property is not set.");

            PricingEngineConfig configList = new PricingEngineConfig();

            // Get the XmlRoot attribute. This means we only need to change XmlRoot attribute value to control deserialization
            var targetXmlRoot = typeof(PricingEngineConfigItem).GetCustomAttribute<XmlRootAttribute>();

            var deserializer = new XmlSerializer(typeof(PricingEngineConfigItem));
            using (var reader = XmlReader.Create(ConfigFile))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement(targetXmlRoot?.ElementName ?? ""))
                    {
                        var config = (PricingEngineConfigItem)deserializer.Deserialize(reader);

                        if (null == config)
                            throw new InvalidOperationException("Failed to deserialize pricing engine configuration.");

                        configList.Add(config);
                    }
                }
            }

            return configList;
        }
    }
}