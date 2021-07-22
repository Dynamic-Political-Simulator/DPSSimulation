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
                float ass = PerceivedAlignment.CalculcateCompatability(g); // WTF, why it 225 all the time??
                baseCompatabilities.Add(g, ass);
                // Console.WriteLine("---");
                // Console.WriteLine(g.Name);
                // Console.WriteLine("Base:" + ass);
            }
            Dictionary<Group, float> popularities = new Dictionary<Group, float>();
            foreach (KeyValuePair<Group, float> kvp in baseCompatabilities)
            {
                float popularity = kvp.Value;
                float mod = this.NationalMod;
                if (Modifiers.ContainsKey(kvp.Key))
                {
                    mod += Modifiers[kvp.Key];
                }

                popularity = popularity + ((float)0.075 * popularity * mod) + ((float)0.015 * mod);

                if (popularity < 0)
                {
                    popularity = 0;
                }

                popularities.Add(kvp.Key, popularity / baseCompatabilities.Values.Max());
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
