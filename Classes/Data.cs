﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace DPSSimulation.Classes
{
    public class Data
    {
        public List<Strata> Stratas { get; set; } = new List<Strata>();
        public List<Strata> GmData { get; set; } = new List<Strata>();

        public int BaseGdpPerPop { get; set; }
        public Data()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("C:\\Users\\thoma\\source\\repos\\DPSSimulation\\DPSSimulation\\Data\\PopDistribution.xml");
            XmlNode popdata = doc.DocumentElement.SelectSingleNode("/popdata");
            BaseGdpPerPop = int.Parse(popdata.Attributes["weight"].InnerText);
            foreach (XmlNode strata in popdata.ChildNodes)
            {
                Stratas.Add(new Strata(strata));
            }

            //temporary for testing
            XmlDocument docGM = new XmlDocument();
            docGM.Load("C:\\Users\\thoma\\source\\repos\\DPSSimulation\\DPSSimulation\\Data\\PopDistributionGM.xml");
            XmlNode popdataGM = docGM.DocumentElement.SelectSingleNode("/popdata");
            //BaseGdpPerPopGM = int.Parse(popdata.Attributes["weight"].InnerText);
            foreach (XmlNode strata in popdataGM.ChildNodes)
            {
                GmData.Add(new Strata(strata));
            }
        }
    }

    public class Strata
    {
        public int StrataWeight { get; set; }
        public Dictionary<string, float> StrataIndustries { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, Job> StrataJobs { get; set; } = new Dictionary<string, Job>();

        public Strata(XmlNode StrataData)
        {
            StrataWeight = int.Parse(StrataData.Attributes["weight"].InnerText);
            SetStrataIndustry(StrataData.SelectSingleNode("strata_industries"));
            SetStrataJobs(StrataData.SelectSingleNode("jobs"));
        }

        public void SetStrataIndustry(XmlNode DataStrataIndustries)
        {
            foreach (XmlNode industry in DataStrataIndustries.ChildNodes)
            {
                StrataIndustries.Add(industry.Name, float.Parse(industry.InnerText, CultureInfo.InvariantCulture));
            }
        }

        public void SetStrataJobs (XmlNode DataJobs)
        {
            foreach (XmlNode job in DataJobs.ChildNodes)
            {
                StrataJobs.Add(job.Name, new Job(job));
            }
        }
    }

    public class Job
    {
        public float JobWeight { get; set; }
        public Dictionary<string, float> JobIndustries { get; set; } = new Dictionary<string, float>();

        public Job (XmlNode JobData)
        {
            JobWeight = float.Parse(JobData.Attributes["weight"].InnerText, CultureInfo.InvariantCulture);

            foreach ( XmlNode jobIndustry in JobData)
            {
                JobIndustries.Add(jobIndustry.Name, float.Parse(jobIndustry.InnerText, CultureInfo.InvariantCulture));
            }
        }
    }
}
