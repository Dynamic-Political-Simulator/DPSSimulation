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
                    Popularity[Faction.Key] = Popularity[Faction.Key] + ((float)0.075 * Popularity[Faction.Key] * GmPopsimData[Faction.Key]) + ((float)0.015 * GmPopsimData[Faction.Key]);
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

        public static bool operator ==(Group a, Group b)
        {
            if ((a is null && !(b is null)) || (!(a is null) && b is null)) return false;
            if (a is null && b is null) return true;
            return a.GroupId == b.GroupId;
        }

        public static bool operator !=(Group a, Group b)
        {
            if ((a is null && !(b is null)) || (!(a is null) && b is null)) return !false;
            if (a is null && b is null) return !true;
            return a.GroupId != b.GroupId;
        }

        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.GroupId == ((Group)obj).GroupId;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.GroupId.GetHashCode();
        }
    }
}
