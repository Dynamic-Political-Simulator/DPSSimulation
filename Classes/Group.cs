using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Group
    {
        public string GroupId { get; set; }
        public string Name { get; set; }
        public int PartyInvolvementFactor { get; set; }
        public float Radicalisation { get; set; }
        public PoliticalAlignment Alignment { get; set; }

        public Dictionary<Faction, float> CalculateGroupPopularity(List<Faction> Factions, Dictionary<Faction, float> GmPopsimData)
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
                if (GmPopsimData.ContainsKey(Faction.Key))
                {
                    Popularity[Faction.Key] = Popularity[Faction.Key] + ((float)0.005 * Popularity[Faction.Key] * GmPopsimData[Faction.Key]) + ((float)0.08 * GmPopsimData[Faction.Key]);
                }

                if (Popularity[Faction.Key] < 0)
                {
                    Popularity[Faction.Key] = 0;
                }
            }
            var popularitysum = Popularity.Sum(p => p.Value);
            for (int i = 0; i < Popularity.Keys.Count; i++)
            {
                Popularity[Popularity.Keys.ElementAt(i)] = Popularity[Popularity.Keys.ElementAt(i)] / popularitysum;
            }
            return Popularity;
        }
    }
}
