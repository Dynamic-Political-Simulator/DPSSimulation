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
            foreach (KeyValuePair<string, int> Alignment in Alignment.Alignments)
            {
                int h = ((Alignment.Value * Group.Alignment.Alignments[Alignment.Key] * 12) - ((Alignment.Value - Group.Alignment.Alignments[Alignment.Key]) * 275));
                Compatabilities.Add((Alignment.Value * Group.Alignment.Alignments[Alignment.Key] * 12) - ((Alignment.Value - Group.Alignment.Alignments[Alignment.Key]) * 275));
            }

            float TotalCompatability = Compatabilities.Sum();
            float NegativeCompatabilities = Compatabilities.Where(c => c < 0).Sum() * 2;
            TotalCompatability += NegativeCompatabilities;
            if (Establishment > Group.Radicalisation)
            {
                TotalCompatability *= (1 + Establishment);
            }
            else
            {
                TotalCompatability *= (1 + Group.Radicalisation);
            }

            if (TotalCompatability < 300 * Establishment)
            {
                TotalCompatability = 300 * Establishment;
            }


            return TotalCompatability;
        }

        public static bool operator ==(Faction a, Faction b)
        {
            if ((a is null && !(b is null)) || (!(a is null) && b is null)) return false;
            if (a is null && b is null) return true;
            return a.FactionId == b.FactionId;
        }

        public static bool operator !=(Faction a, Faction b)
        {
            if ((a is null && !(b is null)) || (!(a is null) && b is null)) return !false;
            if (a is null && b is null) return !true;
            return a.FactionId != b.FactionId;
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

            return this.FactionId == ((Faction)obj).FactionId;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.FactionId.GetHashCode();
        }
    }
}
