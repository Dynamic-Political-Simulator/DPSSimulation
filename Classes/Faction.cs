using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Faction
    {
        public string FactionId { get; set; }
        public string Name { get; set; }
        public List<string> Players { get; set; }
        public float Establishment { get; set; }
        public PoliticalAlignment Alignment { get; set; }

        public float CalculcateCompatability(Group Group)
        {
            List<int> Compatabilities = new List<int>();

            foreach(KeyValuePair<string, int> Alignment in Alignment.Alignments)
            {
                Compatabilities.Add(Alignment.Value * Group.Alignment.Alignments[Alignment.Key] * 12 - ((Alignment.Value - Group.Alignment.Alignments[Alignment.Key]) * 275));
            }

            float TotalCompatability = Compatabilities.Sum();
            float NegativeCompatabilities = Compatabilities.Where(c => c < 0).Sum();
            if(Establishment > Group.Radicalisation)
            {
                NegativeCompatabilities = (NegativeCompatabilities * 2) * Establishment;
            }
            else
            {
                NegativeCompatabilities = (NegativeCompatabilities * 2) * Group.Radicalisation;
            }
            TotalCompatability += NegativeCompatabilities;

            if (TotalCompatability < 300 * Establishment)
            {
                TotalCompatability = 300 * Establishment;
            }

            
            return TotalCompatability;
        }
    }
}
