using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace DPSSimulation.Classes
{
    public class InfraStructureData
    {
        public ulong GdpPerInfrastructure { get; set; }
        public Dictionary<string, Infrastructure> Infrastructures { get; set; } = new Dictionary<string, Infrastructure>();
        

        public InfraStructureData(XmlDocument InfrastructureData)
        {
            
            XmlNode infrastructureData = InfrastructureData.DocumentElement.SelectSingleNode("/space_industry_data");
            GdpPerInfrastructure = ulong.Parse(infrastructureData.Attributes["weight"].InnerText);

            foreach (XmlNode infrastructure in infrastructureData.ChildNodes)
            {
               Infrastructures.Add(infrastructure.Name, new Infrastructure(infrastructure));
            }

            
        }
    }

    public class Infrastructure
    {
        public float InfrastructureWeight { get; set; }
        public Dictionary<string, float> InfrastructureIndustries { get; set; } = new Dictionary<string, float>();

        public Infrastructure(XmlNode infrastructureData)
        {
            InfrastructureWeight = float.Parse(infrastructureData.Attributes["weight"].InnerText, CultureInfo.InvariantCulture);
            foreach (XmlNode industry in infrastructureData.ChildNodes)
            {
                InfrastructureIndustries.Add(industry.Name, float.Parse(industry.InnerText, CultureInfo.InvariantCulture));
            }
        }
    }
}
