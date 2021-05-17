using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;


namespace DPSSimulation.Classes
{
    public class Planet
    {
        //Game Data
        public int PlanetGameId { get; set; }
        public string Name { get; set; }
        public string Planet_class { get; set; }
        public int Owner { get; set; }
        public int Controller { get; set; }
        public List<Pop> Pops { get; set; } = new List<Pop>();
        public List<Building> Buildings { get; set; } = new List<Building>();
        public List<District> Districts { get; set; } = new List<District>();
        public float Stability { get; set; }
        public float Crime { get; set; }
        public float Migration { get; set; }
        //Simulation Data
        public decimal Population { get; set; }
        public Dictionary<string, decimal> Output { get; set; } = new Dictionary<string, decimal>();
        public Data Data { get; set; }
        //Popsim
        public Dictionary<Group, float> PlanetGroups { get; set; } = new Dictionary<Group, float>(); //No idea how we will populate this but its propably fine
        public Dictionary<Faction, float> PlanetFactions { get; set; } = new Dictionary<Faction, float>();

        public void PlanetSim ()
        {
            CalculatePopulation();
        }
        public void CalculatePopulation ()
        {
            Population = (decimal)(Math.Floor(100000 * Math.Pow(Pops.Count,3.5)));
        }

        public void ApplyPlanetaryData(Data data)
        {
            Data = data;
        }

        public void CalculateEconomy()
        {
            CalculatePopulation();
           
            decimal PopulationPerPop = (decimal)(Population / Pops.Count);
            //High Strata
            
            List<decimal> StrataPopulations = new List<decimal>();
            StrataPopulations.Add(Pops.FindAll(p => p.Strata == "\"ruler\"").Count * PopulationPerPop);
            StrataPopulations.Add(Pops.FindAll(p => p.Strata == "\"specialist\"").Count * PopulationPerPop);
            StrataPopulations.Add(Pops.FindAll(p => p.Strata == "\"worker\"").Count * PopulationPerPop);
            CalculateStrataIndustriesOutput(StrataPopulations);
            Dictionary<string, decimal> jobPopulations = new Dictionary<string, decimal>();
            foreach(Pop pop in Pops)
            {
                if (pop.Job!= null)
                {
                    if (jobPopulations.ContainsKey(pop.Job))
                    {
                        jobPopulations[pop.Job] += PopulationPerPop;
                    }
                    else
                    {
                        jobPopulations.Add(pop.Job, PopulationPerPop);
                    }
                }
                
            }
            CalculateStrataJobOutput(jobPopulations);
            
        }

        public void CalculateStrataIndustriesOutput(List<decimal> StrataPopulations)
        {
            for(int i = 0; i<StrataPopulations.Count; i++)
            {
                foreach (KeyValuePair<string, float> industry in Data.Stratas[i].StrataIndustries)
                {
                    if (Output.ContainsKey(industry.Key))
                    {
                        Output[industry.Key] += (decimal)((long)StrataPopulations[i] * Data.BaseGdpPerPop * Data.Stratas[i].StrataWeight * industry.Value * Data.GmData[i].StrataIndustries[industry.Key] / Data.Stratas[i].StrataIndustries.Count);
                    }
                    else
                    {
                        Output.Add(industry.Key, (decimal)((long)StrataPopulations[i] * Data.BaseGdpPerPop * Data.Stratas[i].StrataWeight * industry.Value * Data.GmData[i].StrataIndustries[industry.Key] / Data.Stratas[i].StrataIndustries.Count));
                    }
                }
            }
        }

        public void CalculateStrataJobOutput(Dictionary<string,decimal> jobPopulations)
        {
            for (int i = 0; i < Data.Stratas.Count; i++)
            {
                foreach (KeyValuePair<string, Job> job in Data.Stratas[i].StrataJobs)
                {
                    if (jobPopulations.ContainsKey(job.Key))
                    {
                        foreach (KeyValuePair<string, float> industry in Data.Stratas[i].StrataJobs[job.Key].JobIndustries)
                        {
                            if (Output.ContainsKey(industry.Key))
                            {
                                Output[industry.Key] += (decimal)((long)jobPopulations[job.Key] * Data.BaseGdpPerPop * Data.Stratas[i].StrataWeight * Data.Stratas[i].StrataJobs[job.Key].JobWeight * industry.Value * Data.GmData[i].StrataJobs[job.Key].JobIndustries[industry.Key] / Data.Stratas[i].StrataJobs[job.Key].JobIndustries.Count);
                            }
                            else
                            {
                                Output.Add(industry.Key, (decimal)((long)jobPopulations[job.Key] * Data.BaseGdpPerPop * Data.Stratas[i].StrataWeight * Data.Stratas[i].StrataJobs[job.Key].JobWeight * industry.Value * Data.GmData[i].StrataJobs[job.Key].JobIndustries[industry.Key] / Data.Stratas[i].StrataJobs[job.Key].JobIndustries.Count));
                            }
                        }
                    }   
                }
            }
        }
        
        public void CalculatePopularity()
        {
            Dictionary<Group, Dictionary<Faction,float>> PopularityByGroup = new Dictionary<Group, Dictionary<Faction, float>>();
            foreach(KeyValuePair<Faction,float> Faction in PlanetFactions)
            {
                PlanetFactions[Faction.Key] = 0;
            }

            foreach (KeyValuePair<Group, float> Group in PlanetGroups)
            {
                PopularityByGroup.Add(Group.Key, Group.Key.CalculateGroupPopularity(PlanetFactions.Keys.ToList()));
                foreach(KeyValuePair<Faction,float> Faction in PopularityByGroup[Group.Key])
                {
                    PlanetFactions[Faction.Key] += Faction.Value * Group.Value;
                } 
            }
            
        }

        /*public Dictionary<Faction, float> CalculateGroupPopularity(Group group)
        {
            Dictionary<Faction, float> Popularity = new Dictionary<Faction, float>();
            Dictionary<Faction, float> Compatabilities = new Dictionary<Faction, float>();
            foreach (KeyValuePair<Faction, float> Faction in PlanetFactions)
            {
                Compatabilities.Add(Faction.Key,Faction.Key.CalculcateCompatability(group));
            }
            foreach (KeyValuePair<Faction,float> Faction in Compatabilities)
            {
                Popularity.Add(Faction.Key, Faction.Value / Compatabilities.Sum(c => c.Value));
            }
            return Popularity;
        }*/
    }


}
