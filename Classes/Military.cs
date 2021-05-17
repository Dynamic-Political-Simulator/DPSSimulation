using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Military
    {
        public float MilitaryPoliticisation { get; set; }
        public Dictionary<Group,float> MilitaryGroups { get; set; }
        public Dictionary<Faction,float> MilitaryFactions { get; set; }

        public void SetMilitaryGroups(Dictionary<Group,float> NationalGroups)
        {
            MilitaryGroups.Clear();
            Group Apepolitical = NationalGroups.FirstOrDefault(f => f.Key.Name == "apolitical").Key;
            MilitaryGroups.Add(Apepolitical, 1 - MilitaryPoliticisation);
            NationalGroups.Remove(Apepolitical);
            Dictionary<Group, float> RawGroup = new Dictionary<Group, float>();
            foreach (KeyValuePair<Group,float> Group in NationalGroups)
            {
                RawGroup.Add(Group.Key, Group.Value * Group.Key.Alignment.Alignments["Militarism"]);
            }
            foreach(KeyValuePair<Group,float> Group in RawGroup)
            {
                MilitaryGroups.Add(Group.Key, (Group.Value / RawGroup.Sum(g => g.Value) * (1 - MilitaryPoliticisation)));
            }
        }

        public void SetMilitaryFactions(List<Faction> NationalFactions)
        {
            MilitaryFactions.Clear();
            Dictionary<Faction, float> MilitaryCompat = new Dictionary<Faction, float>();
            Dictionary<Group, float> NoFactionCompat = new Dictionary<Group, float>();


            foreach(Faction faction in NationalFactions)
            {
                Dictionary<Group, float> Compatabilities = new Dictionary<Group, float>();
                foreach(KeyValuePair<Group,float> Group in MilitaryGroups)
                {
                    Compatabilities.Add(Group.Key, faction.CalculcateCompatability(Group.Key)*Group.Value);
                }

                if(faction.Name == "No Faction")
                {
                    NoFactionCompat = Compatabilities;   
                }
                else
                {
                    MilitaryCompat.Add(faction, Compatabilities.Sum(g => g.Value) * MilitaryPoliticisation);
                }
            }
            Faction NoFaction = NationalFactions.FirstOrDefault(f => f.Name == "No Faction");
            if(NoFaction != null)
            {
                MilitaryCompat.Add(NoFaction, (float)((NoFactionCompat.Sum(g => g.Value) * 3.5 * (1 - MilitaryPoliticisation)) + (MilitaryCompat.Sum(f => f.Value) * (1 - MilitaryPoliticisation))));
            }
            foreach(KeyValuePair<Faction,float> Faction in MilitaryCompat)
            {
                MilitaryFactions.Add(Faction.Key, Faction.Value / MilitaryCompat.Sum(f => f.Value));
            }
            
        }
    }
}
