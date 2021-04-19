using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace DPSSimulation.Classes
{
    public class InfraStructureData
    {
        public int GdpPerInfrastructure { get; set; }
        public Dictionary<string, Infrastructure> Infrastructures { get; set; } = new Dictionary<string, Infrastructure>();
        public Dictionary<string, Infrastructure> GmInfrastructures { get; set; } = new Dictionary<string, Infrastructure>();

        public InfraStructureData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Users\\thoma\\source\\repos\\DPSSimulation\\DPSSimulation\\Data\\EmpireDistribution.xml");
            XmlNode infrastructureData = doc.DocumentElement.SelectSingleNode("/space_industry_data");
            GdpPerInfrastructure = int.Parse(infrastructureData.Attributes["weight"].InnerText);

            foreach (XmlNode infrastructure in infrastructureData.ChildNodes)
            {
               Infrastructures.Add(infrastructure.Name, new Infrastructure(infrastructure));
            }

            //Temporary for testing
            XmlDocument docGM = new XmlDocument();
            docGM.Load("C:\\Users\\thoma\\source\\repos\\DPSSimulation\\DPSSimulation\\Data\\EmpireDistributionGM.xml");
            XmlNode infrastructureDataGM = docGM.DocumentElement.SelectSingleNode("/space_industry_data");
            GdpPerInfrastructure = int.Parse(infrastructureData.Attributes["weight"].InnerText);

            foreach (XmlNode infrastructure in infrastructureDataGM.ChildNodes)
            {
                GmInfrastructures.Add(infrastructure.Name, new Infrastructure(infrastructure));
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
