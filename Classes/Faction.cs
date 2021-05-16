using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Faction
    {
        public string Name { get; set; }
        public List<string> Players { get; set; }
        public float Establishment { get; set; }
        public float Radicalisation { get; set; }
        public PoliticalAlignment Alignment { get; set; }

        public float CalculcateCompatability(Group Group)
        {
            List<int> Compatabilities = new List<int>();
            for(int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Compatabilities.Add(Alignment.Alignement[y, x] * Group.Alignment.Alignement[y, x] * 12 - ((Alignment.Alignement[y, x] - Group.Alignment.Alignement[y, x]) * 275));
                }
                
            }
            float TotalCompatability = Compatabilities.Sum();
            float NegativeCompatabilities = Compatabilities.Where(c => c < 0).Sum();
            if(Establishment > Radicalisation)
            {
                NegativeCompatabilities = (NegativeCompatabilities * 2) * Establishment;
            }
            else
            {
                NegativeCompatabilities = (NegativeCompatabilities * 2) * Radicalisation;
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
