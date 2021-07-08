using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Empire
    {
        public int EmpireID { get; set; }
        public List<string> Colors { get; set; } = new List<string>();
        public string Name { get; set; }
        public List<int> OwnedFleetIds { get; set; } = new List<int>();
        public List<int> OwnedArmyIds { get; set; } = new List<int>();
        public List<GalacticObject> GalacticObjects { get; set; } = new List<GalacticObject>();
        public List<Fleet> Fleets { get; set; } = new List<Fleet>();
        public List<Army> Armies { get; set; } = new List<Army>();
        public List<Fleet> MiningStations { get; set; } = new List<Fleet>();
        public List<Fleet> ResearchStations { get; set; } = new List<Fleet>();
        public Dictionary<string, ulong> NationalOutput { get; set; } = new Dictionary<string, ulong>();
        public InfraStructureData InfraStructureData { get; set; }
        public Dictionary<string, float> EconGmData { get; set; } = new Dictionary<string, float>();
        public Dictionary<Faction, int> GeneralAssembly { get; set; } = new Dictionary<Faction, int>();
        public Military Military { get; set; }
        public Dictionary<Group, Dictionary<Faction, float>> PopsimGmData { get; set; } = new Dictionary<Group, Dictionary<Faction, float>>();


        public void OrganiseFleets()
        {
            List<Fleet> starbases = new List<Fleet>();
           
            foreach (Fleet fleet in Fleets)
            {

                if (fleet.Ships.FirstOrDefault(s => s.Type == "\"research_station\"") != null)
                {
                    
                    ResearchStations.Add(fleet);
                    
                }

                if (fleet.Ships.FirstOrDefault(s => s.Type == "\"mining_station\"") != null)
                {
                    MiningStations.Add(fleet);
                    
                }

                
                if (fleet.Ships.FirstOrDefault(s => s.Type.StartsWith("\"starbase")) != null)
                {
                    if (GalacticObjects.FirstOrDefault(g => g.GalacticObjectGameId == fleet.System)!= null) //if this is null it means the starbase is occupied rn. And I just am not gonna care YOLO
                    {
                        GalacticObjects.FirstOrDefault(g => g.GalacticObjectGameId == fleet.System).Starbase.StarbaseFleet = fleet;
                        Console.WriteLine();
                    }
                    
                    starbases.Add(fleet);
                }

               
            }


            foreach (Fleet station in ResearchStations)
            {
                Fleets.Remove(station);
            }
            foreach (Fleet station in MiningStations)
            {
                Fleets.Remove(station);
            }
            foreach (Fleet starbase in starbases)
            {
                Fleets.Remove(starbase);
            }


        }

        public void ApplyEmpireData(InfraStructureData infraData)
        {
            InfraStructureData = infraData;
        }
        public void EmpireEcon()
        {
            NationalOutput = new Dictionary<string, ulong>();
            int districtAmount = 0;
            int starbaseAmount = 0;
            int shipyardAmount = 0;
            int luxuryresidenceAmount = 0;
            //city districts
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Owner == planet.Controller && planet.Pops.Count != 0)
                    {
                        districtAmount += planet.Districts.FindAll(d => d.Type == "\"district_city\"").Count;
                        luxuryresidenceAmount += planet.Buildings.FindAll(b => b.Type == "building_luxury_residence" && !b.ruined).Count;
                        luxuryresidenceAmount += planet.Buildings.FindAll(b => b.Type == "building_paradise_dome" && !b.ruined).Count;
                        planet.CalculateEconomy(EconGmData);
                    }
                }
                if (system.Starbase.Level != "starbase_outpost")
                {
                    starbaseAmount++;
                    shipyardAmount += system.Starbase.Modules.FindAll(m => m == "shipyard").Count;
                }
            }
            CalculcateInfrastructureOutput("city_districts", districtAmount);
            CalculcateInfrastructureOutput("luxury_residences", luxuryresidenceAmount);
            CalculcateInfrastructureOutput("stations", starbaseAmount);
            CalculcateInfrastructureOutput("shipyards", shipyardAmount);

            //research_stations
            CalculcateInfrastructureOutput("research_stations", ResearchStations.Count);
            //Mining Stations
            CalculcateInfrastructureOutput("mining_stations", MiningStations.Count);
            //stations
            
        }

        public void CalculcateInfrastructureOutput(string infrastructureType, int amount)
        {
            foreach (KeyValuePair<string, float> industry in InfraStructureData.Infrastructures[infrastructureType].InfrastructureIndustries)
            {
                float GmModifier = 1;
                if (EconGmData.ContainsKey(industry.Key))
                {
                    GmModifier = EconGmData[industry.Key];
                }
                if (NationalOutput.ContainsKey(industry.Key))
                {
                    NationalOutput[industry.Key] += (ulong)(InfraStructureData.GdpPerInfrastructure*InfraStructureData.Infrastructures[infrastructureType].InfrastructureWeight*GmModifier*industry.Value*amount / InfraStructureData.Infrastructures[infrastructureType].InfrastructureIndustries.Count);
                }
                else
                {
                    NationalOutput.Add(industry.Key, (ulong)(InfraStructureData.GdpPerInfrastructure * InfraStructureData.Infrastructures[infrastructureType].InfrastructureWeight * GmModifier * industry.Value *amount / InfraStructureData.Infrastructures[infrastructureType].InfrastructureIndustries.Count));
                }
            }
                
        }

        public Dictionary<string, ulong> GetGrossGDP()
        {
            EmpireEcon();
            Dictionary<string, ulong> totalOutput = new Dictionary<string, ulong>();
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Population != 0)
                    {
                        foreach (KeyValuePair<string, ulong> industry in planet.Output)
                        {
                            if (totalOutput.ContainsKey(industry.Key))
                            {
                                totalOutput[industry.Key] += industry.Value;
                            }
                            else
                            {
                                totalOutput.Add(industry.Key, industry.Value);
                            }
                        }
                    } 
                }
            }
            foreach (KeyValuePair<string, ulong> industry in NationalOutput)
            {
                if (totalOutput.ContainsKey(industry.Key))
                {
                    totalOutput[industry.Key] += industry.Value;
                }
                else
                {
                    totalOutput.Add(industry.Key, industry.Value);
                }
            }
            return totalOutput;
        }

        public Dictionary<string,float> GetGlobalStrataOutput()
        {
            Dictionary<string, float> StrataOutput = new Dictionary<string, float>();
            int planetNum = 0;
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Population != 0)
                    {
                        planetNum++;
                        Dictionary<string, float> PlanetStrata = planet.OutputStrataGDP(EconGmData);

                        foreach (KeyValuePair<string, float> industry in PlanetStrata)
                        {
                            if (StrataOutput.ContainsKey(industry.Key))
                            {
                                StrataOutput[industry.Key] += industry.Value;
                            }
                            else
                            {
                                StrataOutput.Add(industry.Key, industry.Value);
                            }
                        }
                    }
                }
            }
            Dictionary<string, float> FinalOutput = new Dictionary<string, float>();
            foreach(KeyValuePair<string,float> strata in StrataOutput)
            {
                FinalOutput.Add(strata.Key, strata.Value / planetNum);
            }
            return FinalOutput;

        }

        public void SetParliament()
        {
            Dictionary<Planet,Dictionary<Faction, float>> FactionPopularities = new Dictionary<Planet, Dictionary<Faction, float>>();
            ulong TotalPopulation = 0;
            
            foreach(GalacticObject system in GalacticObjects)
            {
                foreach(Planet planet in system.Planets)
                {
                    if(planet.Pops.Count != 0)
                    {
                        
                        planet.CalculatePopularity(PopsimGmData);
                        TotalPopulation += planet.Population;
                        Dictionary<Faction, float> PlanetFactions = planet.PlanetFactions;
                        var Query = PlanetFactions.OrderBy(f => f.Value).Reverse();
                        Dictionary<Faction, float> FinalFactions = new Dictionary<Faction, float>();
                        float percentage = 0;
                        foreach (KeyValuePair<Faction,float> Faction in Query)
                        {
                            if (percentage < 0.6)
                            {
                                percentage += Faction.Value;
                                FinalFactions.Add(Faction.Key,Faction.Value);
                            }
                        }
                        float total = FinalFactions.Sum(f => f.Value);
                        foreach(KeyValuePair<Faction,float> Faction in FinalFactions)
                        {
                            FinalFactions[Faction.Key] = Faction.Value / total;
                        }
                        FactionPopularities.Add(planet, FinalFactions);
                    }
                }
            }
            foreach(KeyValuePair<Planet,Dictionary<Faction,float>> Planet in FactionPopularities)
            {
                float PopulationPercentage = (float)(Planet.Key.Population / TotalPopulation);
                int seats = (int)(6000 * PopulationPercentage);
                foreach(KeyValuePair<Faction,float> Faction in Planet.Value)
                {
                    if (GeneralAssembly.ContainsKey(Faction.Key))
                    {
                        GeneralAssembly[Faction.Key] += (int)(seats * Faction.Value);
                    }
                    else
                    {
                        GeneralAssembly.Add(Faction.Key, (int)(seats * Faction.Value));
                    }
                }
            }
        }
        public Dictionary<Faction,float> CalculateGlobalPopularity()
        {
            Dictionary<Faction, float> GlobalPopularity = new Dictionary<Faction, float>();
            Dictionary<Planet, Dictionary<Faction, float>> FactionPopularities = new Dictionary<Planet, Dictionary<Faction, float>>();
            ulong TotalPopulation = 0;
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Pops.Count != 0)
                    {

                        planet.CalculatePopularity(PopsimGmData);
                        TotalPopulation += planet.Population;
                        FactionPopularities.Add(planet, planet.PlanetFactions);
                    }
                }
            }
            foreach (KeyValuePair<Planet, Dictionary<Faction, float>> Planet in FactionPopularities)
            {
                float PopulationPercentage = (float)(Planet.Key.Population / TotalPopulation);
                foreach (KeyValuePair<Faction, float> Faction in Planet.Value)
                {
                    if (GlobalPopularity.ContainsKey(Faction.Key))
                    {
                        GlobalPopularity[Faction.Key] += (PopulationPercentage * Faction.Value);
                    }
                    else
                    {
                        GlobalPopularity.Add(Faction.Key, (PopulationPercentage * Faction.Value));
                    }
                }
            }

            return GlobalPopularity;
        }

        public Dictionary<Group,float> CalculateGlobalGroupSize()
        {
            Dictionary<Planet, Dictionary<Group, float>> GroupSizeByPlanet = new Dictionary<Planet, Dictionary<Group, float>>();
            ulong TotalPopulation = 0;
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Pops.Count != 0)
                    {

                        planet.CalculatePopularity(PopsimGmData);
                        TotalPopulation += planet.Population;
                        GroupSizeByPlanet.Add(planet, planet.PlanetGroups);
                    }
                }
            }
            Dictionary<Group, float> GroupSize = new Dictionary<Group, float>();
            foreach(KeyValuePair<Planet, Dictionary<Group, float>> planet in GroupSizeByPlanet)
            {
                float PopulationPercentage = (float)(planet.Key.Population / TotalPopulation);

                foreach(KeyValuePair<Group, float> group in planet.Value)
                {
                    if (GroupSize.ContainsKey(group.Key))
                    {
                        GroupSize[group.Key] += (PopulationPercentage * group.Value);
                    }
                    else
                    {
                        GroupSize.Add(group.Key, (PopulationPercentage * group.Value));
                    }
                }
            }

            return GroupSize;
        }

        public ulong GetGlobalPopulation()
        {
            ulong TotalPopulation = 0;
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Pops.Count != 0)
                    {

                        planet.CalculatePopulation();
                        TotalPopulation += planet.Population;
                    }
                }
            }

            return TotalPopulation;
        }
    }
}
