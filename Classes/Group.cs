using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Group
    {
        public string Name { get; set; }
        public int PartyInvolvementFactor { get; set; }
        public float Radicalisation { get; set; }
        public PoliticalAlignment Alignment { get; set; }

        public Dictionary<Faction, float> CalculateGroupPopularity(List<Faction> Factions)
        {
            Dictionary<Faction, float> Popularity = new Dictionary<Faction, float>();
            Dictionary<Faction, float> Compatabilities = new Dictionary<Faction, float>();
            foreach (Faction Faction in Factions)
            {
                Compatabilities.Add(Faction, Faction.CalculcateCompatability(this));
            }
            foreach (KeyValuePair<Faction, float> Faction in Compatabilities)
            {
                Popularity.Add(Faction.Key, Faction.Value / Compatabilities.Sum(c => c.Value));
            }
            return Popularity;
        }
    }
}
