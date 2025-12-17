using System.Xml.Serialization;

namespace HmxLabs.TechTest.RiskSystem
{
    [XmlRoot("Engine")]
    public class PricingEngineConfigItem
    {
        [XmlAttribute("tradeType")]
        public string? TradeType { get; set; }

        [XmlAttribute("assembly")]
        public string? Assembly { get; set; }

        [XmlAttribute("pricingEngine")]
        public string? TypeName { get; set; }
    }
}
