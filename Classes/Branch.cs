using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Branch
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
        public Faction PerceivedAlignment { get; set; }
        public Dictionary<Group, float> Modifiers { get; set; }
        public float NationalMod { get; set; }

        public float CalculateBranchPopularity(Dictionary<Group, float> Groups)
        {
            Dictionary<Group, float> baseCompatabilities = new Dictionary<Group, float>();
            foreach (Group g in Groups.Keys)
            {
                baseCompatabilities.Add(g, PerceivedAlignment.CalculcateCompatability(g));
            }
            Dictionary<Group, float> popularities = new Dictionary<Group, float>();
            float mod = this.NationalMod;
            foreach (KeyValuePair<Group, float> kvp in baseCompatabilities)
            {
                float popularity = kvp.Value / baseCompatabilities.Values.Max();
                if (Modifiers.ContainsKey(kvp.Key))
                {
                    mod += Modifiers[kvp.Key];
                }

                popularity = popularity + ((float)0.075 * popularity * mod) + ((float)0.015 * mod);

                if (popularity < 0)
                {
                    popularity = 0;
                }
                popularities.Add(kvp.Key, popularity);
            }
            float final = 0;
            foreach (KeyValuePair<Group, float> keyValuePair in popularities)
            {
                final += keyValuePair.Value * Groups[keyValuePair.Key];
            }
            return final;
        }
    }
}
