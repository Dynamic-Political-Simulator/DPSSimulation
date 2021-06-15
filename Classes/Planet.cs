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
        public ulong Population { get; set; }
        public Dictionary<string, ulong> Output { get; set; } = new Dictionary<string, ulong>();
        public Data Data { get; set; }
        public Dictionary<string, float> GmData { get; set; } = new Dictionary<string, float>();
        //Popsim
        public Dictionary<Group, float> PlanetGroups { get; set; } = new Dictionary<Group, float>(); //No idea how we will populate this but its propably fine
        public Dictionary<Faction, float> PlanetFactions { get; set; } = new Dictionary<Faction, float>();

        public void PlanetSim ()
        {
            CalculatePopulation();
        }
        public void CalculatePopulation ()
        {
            Population = (ulong)(Math.Floor(100000 * (decimal)Math.Pow(Pops.Count,3.5)));
        }

        public void ApplyPlanetaryData(Data data)
        {
            Data = data;
        }

        public void CalculateEconomy(Dictionary<string, float> EmpireModifiers)
        {
            CalculatePopulation();
           
            ulong PopulationPerPop = (Population / (ulong)Pops.Count);
            //High Strata
            
            List<ulong> StrataPopulations = new List<ulong>();
            StrataPopulations.Add((ulong)Pops.FindAll(p => p.Strata == "\"ruler\"").Count * PopulationPerPop);
            StrataPopulations.Add((ulong)Pops.FindAll(p => p.Strata == "\"specialist\"").Count * PopulationPerPop);
            StrataPopulations.Add((ulong)Pops.FindAll(p => p.Strata == "\"worker\"").Count * PopulationPerPop);
            CalculateStrataIndustriesOutput(StrataPopulations, EmpireModifiers);
            Dictionary<string, ulong> jobPopulations = new Dictionary<string, ulong>();
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
            CalculateStrataJobOutput(jobPopulations, EmpireModifiers);
            
        }

        public void CalculateStrataIndustriesOutput(List<ulong> StrataPopulations, Dictionary<string, float> EmpireModifiers)
        {
            for(int i = 0; i<StrataPopulations.Count; i++)
            {
                foreach (KeyValuePair<string, float> industry in Data.Stratas[i].StrataIndustries)
                {
                    float GmModifier = 1;
                    float EmpireModifier = 1;
                    if (EmpireModifiers.ContainsKey(industry.Key))
                    {
                        EmpireModifier = EmpireModifiers[industry.Key];
                    }
                    if (GmData.ContainsKey(industry.Key))
                    {
                        GmModifier = GmData[industry.Key];
                    }
                    
                    if (Output.ContainsKey(industry.Key))
                    {
                        Output[industry.Key] += (ulong)(StrataPopulations[i] * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[i].StrataWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[i].StrataIndustries.Count);
                    }
                    else
                    {
                        Output.Add(industry.Key, (ulong)(StrataPopulations[i] * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[i].StrataWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[i].StrataIndustries.Count));
                    }
                }
            }
        }

        public void CalculateStrataJobOutput(Dictionary<string,ulong> jobPopulations, Dictionary<string, float> EmpireModifiers)
        {
            for (int i = 0; i < Data.Stratas.Count; i++)
            {
                foreach (KeyValuePair<string, Job> job in Data.Stratas[i].StrataJobs)
                {
                    if (jobPopulations.ContainsKey(job.Key))
                    {
                        foreach (KeyValuePair<string, float> industry in Data.Stratas[i].StrataJobs[job.Key].JobIndustries)
                        {
                            float GmModifier = 1;
                            float EmpireModifier = 1;
                            if (EmpireModifiers.ContainsKey(industry.Key))
                            {
                                EmpireModifier = EmpireModifiers[industry.Key];
                            }
                            if (GmData.ContainsKey(industry.Key))
                            {
                                GmModifier = GmData[industry.Key];
                            }

                            if (Output.ContainsKey(industry.Key))
                            {
                                Output[industry.Key] += (ulong)(jobPopulations[job.Key] * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[i].StrataWeight * Data.Stratas[i].StrataJobs[job.Key].JobWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[i].StrataJobs[job.Key].JobIndustries.Count);
                            }
                            else
                            {
                                Output.Add(industry.Key, (ulong)(jobPopulations[job.Key] * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[i].StrataWeight * Data.Stratas[i].StrataJobs[job.Key].JobWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[i].StrataJobs[job.Key].JobIndustries.Count));
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
