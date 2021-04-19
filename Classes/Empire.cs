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
        public Dictionary<string, decimal> NationalOutput { get; set; } = new Dictionary<string, decimal>();
        public InfraStructureData InfraStructureData { get; set; }

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
                        planet.CalculateEconomy();
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
                if (NationalOutput.ContainsKey(industry.Key))
                {
                    NationalOutput[industry.Key] += (decimal)(InfraStructureData.GdpPerInfrastructure*InfraStructureData.Infrastructures[infrastructureType].InfrastructureWeight* InfraStructureData.GmInfrastructures[infrastructureType].InfrastructureWeight*industry.Value*InfraStructureData.GmInfrastructures[infrastructureType].InfrastructureIndustries[industry.Key]*amount / InfraStructureData.Infrastructures[infrastructureType].InfrastructureIndustries.Count);
                }
                else
                {
                    NationalOutput.Add(industry.Key, (decimal)(InfraStructureData.GdpPerInfrastructure * InfraStructureData.Infrastructures[infrastructureType].InfrastructureWeight * InfraStructureData.GmInfrastructures[infrastructureType].InfrastructureWeight * industry.Value * InfraStructureData.GmInfrastructures[infrastructureType].InfrastructureIndustries[industry.Key]*amount / InfraStructureData.Infrastructures[infrastructureType].InfrastructureIndustries.Count));
                }
            }
                
        }

        public Dictionary<string, decimal> GetGrossGDP()
        {
            Dictionary<string, decimal> totalOutput = new Dictionary<string, decimal>();
            foreach (GalacticObject system in GalacticObjects)
            {
                foreach (Planet planet in system.Planets)
                {
                    if (planet.Population != 0)
                    {
                        foreach (KeyValuePair<string, decimal> industry in planet.Output)
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
            foreach (KeyValuePair<string, decimal> industry in NationalOutput)
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
    }
}
