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
        public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
        //Popsim
        public Dictionary<Group, float> PlanetGroups { get; set; } = new Dictionary<Group, float>(); //No idea how we will populate this but its propably fine
        public Dictionary<Faction, float> PlanetFactions { get; set; } = new Dictionary<Faction, float>();
        public Dictionary<Group, Dictionary<Faction, float>> PopsimGmData = new Dictionary<Group, Dictionary<Faction, float>>();

        public void PlanetSim ()
        {
            CalculatePopulation();
        }
        public void CalculatePopulation ()
        {
            //Population = (ulong)(Math.Floor(100000 * (decimal)Math.Pow(Pops.Count,3.5))); FUCK YOU REV AND SKELLY THIS IS COOL!
            var rand = new Random();
            long ranndom = rand.Next(-10000000, 10000000);
            long population = Convert.ToInt64((250000000 * (decimal)Pops.Count()) + rand.Next(-10000000, 10000000));
            Population = (ulong)((250000000 * (decimal)Pops.Count())+rand.Next(-100000,10000000));
        }

        public void ApplyPlanetaryData(Data data)
        {
            Data = data;
        }

        public void CalculateEconomy(Dictionary<string, float> EmpireModifiers)
        {
            CalculatePopulation();
            Output = new Dictionary<string, ulong>();
            ulong PopulationPerPop = (Population / (ulong)Pops.Count);
            //High Strata
            Console.WriteLine(Name);
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
                    if (EconGmData.ContainsKey(industry.Key))
                    {
                        GmModifier = EconGmData[industry.Key];
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
                            if (EconGmData.ContainsKey(industry.Key))
                            {
                                GmModifier = EconGmData[industry.Key];
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

        
        
        public Dictionary<string, float> OutputStrataGDP(Dictionary<string, float> EmpireModifiers)
        {
            CalculatePopulation();

            ulong PopulationPerPop = (Population / (ulong)Pops.Count);

            Dictionary<string, float> StrataOutput = new Dictionary<string, float>();
            List<Pop> ruler = Pops.FindAll(p => p.Strata == "\"ruler\"");
            List<Pop> specialist = Pops.FindAll(p => p.Strata == "\"specialist\"");
            List<Pop> worker = Pops.FindAll(p => p.Strata == "\"worker\"");
            Dictionary<string,ulong> popAmount = new Dictionary<string, ulong>();
            popAmount.Add("ruler",(ulong)ruler.Count * PopulationPerPop);
            popAmount.Add("specialist",(ulong)specialist.Count * PopulationPerPop);
            popAmount.Add("worker",(ulong)worker.Count * PopulationPerPop);
            ulong Output1 = 0;
            int x = 0;
            foreach(KeyValuePair<string,ulong> size in popAmount)
            {
                Output1 = 0;
                
                    foreach (KeyValuePair<string, float> industry in Data.Stratas[x].StrataIndustries)
                    {
                        float GmModifier = 1;
                        float EmpireModifier = 1;
                        if (EmpireModifiers.ContainsKey(industry.Key))
                        {
                            EmpireModifier = EmpireModifiers[industry.Key];
                        }
                        if (EconGmData.ContainsKey(industry.Key))
                        {
                            GmModifier = EconGmData[industry.Key];
                        }

                        Output1 += (ulong)(size.Value * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[x].StrataWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[x].StrataIndustries.Count);
                    }
                x++;
                float OutputPerCapita = 0;
                if (size.Value != 0)
                {
                    OutputPerCapita = Output1 / size.Value;
                }
                
                StrataOutput.Add(size.Key, OutputPerCapita); 
            }
            if (popAmount["ruler"] != 0)
            {
                StrataOutput["ruler"] += PerStrataJobStuff(ruler, EmpireModifiers, PopulationPerPop, 0) / popAmount["ruler"];
            }
            if (popAmount["specialist"] != 0)
            {
                StrataOutput["specialist"] += PerStrataJobStuff(specialist, EmpireModifiers, PopulationPerPop, 1) / popAmount["specialist"];
            }
            if (popAmount["worker"] != 0)
            {
                StrataOutput["worker"] += PerStrataJobStuff(worker, EmpireModifiers, PopulationPerPop, 2) / popAmount["worker"];
            }
            
            
            

            return StrataOutput;
        }

        public ulong PerStrataJobStuff(List<Pop> Pops, Dictionary<string, float> EmpireModifiers, ulong PopulationPerPop, int strata)
        {
            Dictionary<string, ulong> jobPopulations = new Dictionary<string, ulong>();
            foreach (Pop pop in Pops)
            {
                if (pop.Job != null)
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
            ulong Ouput = 0;

            foreach (KeyValuePair<string, Job> job in Data.Stratas[strata].StrataJobs)
            {
                if (jobPopulations.ContainsKey(job.Key))
                {
                    foreach (KeyValuePair<string, float> industry in Data.Stratas[strata].StrataJobs[job.Key].JobIndustries)
                    {
                        float GmModifier = 1;
                        float EmpireModifier = 1;
                        if (EmpireModifiers.ContainsKey(industry.Key))
                        {
                            EmpireModifier = EmpireModifiers[industry.Key];
                        }
                        if (EconGmData.ContainsKey(industry.Key))
                        {
                            GmModifier = EconGmData[industry.Key];
                        }

                        Ouput += (ulong)(jobPopulations[job.Key] * (ulong)Data.BaseGdpPerPop * (ulong)Data.Stratas[strata].StrataWeight * Data.Stratas[strata].StrataJobs[job.Key].JobWeight * industry.Value * GmModifier * EmpireModifier / Data.Stratas[strata].StrataJobs[job.Key].JobIndustries.Count);
                    }
                }
            }
            return Ouput;
        }

        public void CalculatePopularity(Dictionary<Group, Dictionary<Faction, float>> EmpirePopsimGmData)
        {
            Dictionary<Group, Dictionary<Faction,float>> PopularityByGroup = new Dictionary<Group, Dictionary<Faction, float>>();
            for(int i = 0; i < PlanetFactions.Keys.Count; i++) {
                PlanetFactions[PlanetFactions.Keys.ElementAt(i)] = 0;
            }

            foreach (KeyValuePair<Group, float> Group in PlanetGroups)
            {
                Group DataKey = EmpirePopsimGmData.Keys.FirstOrDefault(g => g.Name == Group.Key.Name);
                Group DataKey2 = PopsimGmData.Keys.FirstOrDefault(g => g.Name == Group.Key.Name);
                Dictionary<Faction, float> CombinedGmData = new Dictionary<Faction, float>();
                if (DataKey != null && DataKey2 != null)
                {
                    CombinedGmData = EmpirePopsimGmData[DataKey].Concat(PopsimGmData[DataKey2])
                   .GroupBy(x => x.Key)
                   .ToDictionary(x => x.Key, x => x.Sum(y => y.Value));
                }
                
                PopularityByGroup.Add(Group.Key, Group.Key.CalculateGroupPopularity(PlanetFactions.Keys.ToList(),CombinedGmData));
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
