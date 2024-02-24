using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace PlayerSpace
{
    public class Diplomacy
    {
        public void DeclareWar(War war)
        {
            GD.Print($"{war.attackerFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            EventLog.Instance.AddLog($"{war.attackerFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            Globals.Instance.selectedRightClickCounty.countyData
                .factionData.diplomacy.RespondToDeclarationOfWar(war, Globals.Instance.selectedRightClickCounty);
        }

        public void RespondToDeclarationOfWar(War war, SelectCounty battleLocation)
        {
            GD.Print($"{war.defenderFactionData.factionName} is responding to the declaration of war.");
            EventLog.Instance.AddLog($"{war.defenderFactionData.factionName} is raising armies!");

            DefenderSpawnArmies(war.defenderFactionData, battleLocation);
            // Add the wars to the factions so they know what wars they are in.
            war.attackerFactionData.wars.Add(war);
            war.defenderFactionData.wars.Add(war);
        }

        public void DefenderSpawnArmies(FactionData defenderFactionData, SelectCounty battleLocation)
        {
            defenderFactionData.tokenSpawner.Spawn(battleLocation, CheckForDefenders(battleLocation));
        }

        private CountyPopulation CheckForDefenders(SelectCounty battleLocation)
        {
            if (battleLocation.countyData.heroCountyList.Count > 0)
            {
                foreach (CountyPopulation countyPopulation in battleLocation.countyData.heroCountyList)
                {
                    if (countyPopulation.isFactionLeader == true)
                    {
                        countyPopulation.IsArmyLeader = true;
                        return countyPopulation;
                    }
                    else
                    {
                        if (countyPopulation.loyaltyAttribute > Globals.Instance.loyaltyCheckNumber)
                        {
                            countyPopulation.IsArmyLeader = true;
                            return countyPopulation;
                        }
                        else
                        {
                            // This is wrong.  We need to make it check then rest of the population if there are no loyal heroes.
                            GD.Print("No loyal heroes in county for defense.");
                            return null;
                        }
                    }
                }
            }
            else
            {
                GD.Print("Defenders Faction Name: " + battleLocation.countyData.factionData.factionName);
                if (battleLocation.countyData.factionData.Influence >= Globals.Instance.costOfHero)
                {
                    List<CountyPopulation> possibleDefenders = [];
                    foreach (CountyPopulation countyPopulation in battleLocation.countyData.countyPopulationList)
                    {
                        if (countyPopulation.loyaltyAttribute > Globals.Instance.loyaltyCheckNumber)
                        {
                            possibleDefenders.Add(countyPopulation);
                        }
                    }

                    if (possibleDefenders.Count > 0)
                    {
                        // Order the possbileDefenders list by highest cool and rifle skill.
                        possibleDefenders = possibleDefenders.OrderByDescending(countyPopulation => countyPopulation.coolSkill)
                                     .ThenByDescending(countyPopulation => countyPopulation.rifleSkill)
                                     .ToList();
                        foreach (CountyPopulation countyPopulation in possibleDefenders)
                        {
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.coolSkill} {countyPopulation.rifleSkill}");
                        }
                        possibleDefenders[0].ChangeToArmy();
                        return possibleDefenders[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    GD.Print("Not enough influence to hire a hero for defence.");
                    return null;
                }

            }
            return null;
        }
    }
}