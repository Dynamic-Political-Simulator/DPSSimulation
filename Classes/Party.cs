using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DPSSimulation.Classes
{
    public class Party
    {
        public Dictionary<Group, float> PopGroupEnlistment { get; set; } = new Dictionary<Group, float>();
        public Dictionary<Group, float> UpperPartyMembership { get; set; } = new Dictionary<Group, float>();
        public Dictionary<Group, float> LowerPartyMembership { get; set; } = new Dictionary<Group, float>();
        public Dictionary<Faction, float> UpperPartyAffinity { get; set; } = new Dictionary<Faction, float>();
        public Dictionary<Faction, float> LowerPartyAffinity { get; set; } = new Dictionary<Faction, float>();
        public float UpperPartyPercentage { get; set; }

        public void SetPopGroupEnlistment(Dictionary<Group, float> GroupsAndJeremy) //Needs Groups with associated weights
        {
            foreach (KeyValuePair<Group, float> GroupJeremy in GroupsAndJeremy)
            {
                if (PopGroupEnlistment.ContainsKey(GroupJeremy.Key))
                {
                    PopGroupEnlistment[GroupJeremy.Key] = (GroupJeremy.Key.PartyInvolvementFactor / 10f) + ((GroupJeremy.Key.PartyInvolvementFactor * GroupJeremy.Key.Radicalisation) / 30);
                }
                else
                {
                    PopGroupEnlistment.Add(GroupJeremy.Key, (GroupJeremy.Key.PartyInvolvementFactor / 10f) + ((GroupJeremy.Key.PartyInvolvementFactor * GroupJeremy.Key.Radicalisation) / 30));
                }
                // Console.WriteLine(GroupJeremy.Key.Name + ": " + PopGroupEnlistment[GroupJeremy.Key]);
                PopGroupEnlistment[GroupJeremy.Key] = (float)(PopGroupEnlistment[GroupJeremy.Key] + (PopGroupEnlistment[GroupJeremy.Key] * 0.05 * GroupJeremy.Value) + (0.005 * GroupJeremy.Value));

                if (PopGroupEnlistment[GroupJeremy.Key] < 0)
                {
                    PopGroupEnlistment[GroupJeremy.Key] = 0;
                }
            }
        }

        public float NationalEnlistment(Dictionary<Group, float> GroupsAndSizes)
        {
            float Enlistment = 0;

            foreach (KeyValuePair<Group, float> GroupAndSize in GroupsAndSizes)
            {
                Enlistment += GroupAndSize.Value * PopGroupEnlistment[GroupAndSize.Key];
            }

            return Enlistment;
        }

        public Dictionary<Group, float> GroupPercentageOfParty(Dictionary<Group, float> GroupsAndSizes)
        {
            Dictionary<Group, float> GroupPercentageOfParty = new Dictionary<Group, float>();
            float NatioanEnlistment = NationalEnlistment(GroupsAndSizes);

            foreach (KeyValuePair<Group, float> Group in PopGroupEnlistment)
            {
                GroupPercentageOfParty.Add(Group.Key, Group.Value * GroupsAndSizes[Group.Key] / NatioanEnlistment);
            }

            return GroupPercentageOfParty;
        }

        public void PartyUpperAndLowerCalculation(Dictionary<Faction, float> FactionsAndJeremyUpper, Dictionary<Faction, float> FactionsAndJeremyLower, ulong Population, Dictionary<Group, float> GroupsAndSizes)
        {
            //-------------------------------------------------------------------------------------------------
            //------------Setting Memmbership------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------
            Dictionary<Group, float> PIFcalcValue = new Dictionary<Group, float>();
            Dictionary<Group, float> GPP = GroupPercentageOfParty(GroupsAndSizes);
            foreach (KeyValuePair<Group, float> Group in GPP)
            {
                // Console.WriteLine(Group.Key.Name + ": " + Group.Value);
                if ((Group.Key.PartyInvolvementFactor - 1) < 1)
                {
                    PIFcalcValue.Add(Group.Key, Group.Value * Group.Key.PartyInvolvementFactor);
                }
                else
                {
                    PIFcalcValue.Add(Group.Key, Group.Value * (Group.Key.PartyInvolvementFactor * (Group.Key.PartyInvolvementFactor - 1)));
                }
            }
            // Console.WriteLine();
            foreach (KeyValuePair<Group, float> Group in PopGroupEnlistment)
            {
                // Console.WriteLine(Group.Key.Name + ": " + PIFcalcValue[Group.Key] / PIFcalcValue.Sum(g => g.Value));
                if (UpperPartyMembership.ContainsKey(Group.Key))
                {
                    UpperPartyMembership[Group.Key] = PIFcalcValue[Group.Key] / PIFcalcValue.Sum(g => g.Value);
                }
                else
                {
                    UpperPartyMembership.Add(Group.Key, PIFcalcValue[Group.Key] / PIFcalcValue.Sum(g => g.Value));
                }
            }

            ulong PeopleInParty = (ulong)(Population * NationalEnlistment(GroupsAndSizes));

            Dictionary<Group, ulong> PeopleInLowerParty = new Dictionary<Group, ulong>();

            foreach (KeyValuePair<Group, float> Group in PopGroupEnlistment)
            {
                PeopleInLowerParty.Add(Group.Key, (ulong)((Group.Value * GroupsAndSizes[Group.Key] * Population) - (UpperPartyMembership[Group.Key] * PeopleInParty * UpperPartyPercentage)));
            }
            foreach (KeyValuePair<Group, ulong> Group in PeopleInLowerParty)
            {
                if (LowerPartyMembership.ContainsKey(Group.Key))
                {
                    LowerPartyMembership[Group.Key] = (float)Group.Value / (ulong)PeopleInLowerParty.Sum(g => (decimal)g.Value);
                }
                else
                {
                    LowerPartyMembership.Add(Group.Key, (float)Group.Value / (ulong)PeopleInLowerParty.Sum(g => (decimal)g.Value));
                }
            }

            //------------------------------------------------------------
            //---------------Upper Lower Factions-------------------------
            //------------------------------------------------------------

            //Upper Party Factions
            Dictionary<Faction, float> UpperAffinity = new Dictionary<Faction, float>();
            foreach (KeyValuePair<Faction, float> FactionAndJeremy in FactionsAndJeremyUpper)
            {
                List<float> Compatabilities = new List<float>();
                foreach (KeyValuePair<Group, float> Group in UpperPartyMembership)
                {
                    Compatabilities.Add(FactionAndJeremy.Key.CalculcateCompatability(Group.Key) * Group.Value);
                }

                float JeremyCompat = (float)(Compatabilities.Sum() + (Compatabilities.Sum() * FactionAndJeremy.Value * 0.08) + (100 * FactionAndJeremy.Value));
                if (JeremyCompat < 0)
                {
                    JeremyCompat = 0;
                }
                UpperAffinity.Add(FactionAndJeremy.Key, JeremyCompat);
            }
            foreach (KeyValuePair<Faction, float> Faction in UpperAffinity)
            {
                if (UpperPartyAffinity.ContainsKey(Faction.Key))
                {
                    UpperPartyAffinity[Faction.Key] = UpperAffinity[Faction.Key] / UpperAffinity.Sum(f => f.Value);
                }
                else
                {
                    UpperPartyAffinity.Add(Faction.Key, UpperAffinity[Faction.Key] / UpperAffinity.Sum(f => f.Value));
                }
            }
            //lower Party Factions
            Dictionary<Faction, float> LowerAffinity = new Dictionary<Faction, float>();
            foreach (KeyValuePair<Faction, float> FactionAndJeremy in FactionsAndJeremyLower)
            {
                List<float> Compatabilities = new List<float>();
                foreach (KeyValuePair<Group, float> Group in LowerPartyMembership)
                {
                    Compatabilities.Add(FactionAndJeremy.Key.CalculcateCompatability(Group.Key) * Group.Value);
                }

                float JeremyCompat = (float)(Compatabilities.Sum() + (Compatabilities.Sum() * FactionAndJeremy.Value * 0.08) + (100 * FactionAndJeremy.Value));
                if (JeremyCompat < 0)
                {
                    JeremyCompat = 0;
                }
                LowerAffinity.Add(FactionAndJeremy.Key, JeremyCompat);
            }
            foreach (KeyValuePair<Faction, float> Faction in LowerAffinity)
            {
                if (LowerPartyAffinity.ContainsKey(Faction.Key))
                {
                    LowerPartyAffinity[Faction.Key] = LowerAffinity[Faction.Key] / LowerAffinity.Sum(f => f.Value);
                }
                else
                {
                    LowerPartyAffinity.Add(Faction.Key, LowerAffinity[Faction.Key] / LowerAffinity.Sum(f => f.Value));
                }
            }

        }
    }
}
