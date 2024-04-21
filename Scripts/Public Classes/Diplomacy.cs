using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public class Diplomacy
    {
        public void DeclareWar(War war)
        {
            GD.Print($"{war.aggressorFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            EventLog.Instance.AddLog($"{war.aggressorFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            Globals.Instance.selectedRightClickCounty.countyData
                .factionData.diplomacy.RespondToDeclarationOfWar(war, Globals.Instance.selectedRightClickCounty);
        }

        public void DeclareWarConfirmation(CountyData countyData)
        {
            DeclareWarControl.Instance.Show();
            
            DeclareWarControl.Instance.declareWarTitleLabel.Text
                = AllText.Diplomacies.DECLAREWARE + countyData.factionData.factionName;
            
        }

        public void RespondToDeclarationOfWar(War war, County battleLocation)
        {
            GD.Print($"{war.defenderFactionData.factionName} is responding to the declaration of war.");

            war.aggressorFactionData.factionWarDictionary[war.defenderFactionData.factionName] = true;
            war.defenderFactionData.factionWarDictionary[war.aggressorFactionData.factionName] = true;
            // Add the wars to the factions so they know what wars they are in.
            war.aggressorFactionData.wars.Add(war);
            war.defenderFactionData.wars.Add(war);
        }

        public void DefenderSpawnArmies(County battleLocation)
        {
            CountyPopulation defenderHero = CheckForArmies(battleLocation);
            if (defenderHero != null)
            {
                // Defender's faction data.
                battleLocation.countyData.factionData.tokenSpawner.Spawn(battleLocation, defenderHero);
            }
            else
            {
                GD.Print("Defender Spawn Armies - Defender Hero is null");
            }
        }

        public CountyPopulation CheckForArmies(County battleLocation)
        {
            // Checkes for spawned armies.  If there is one, then it returns null, otherwise it spawns one.
            //GD.Print("Defending Army List Count: " + battleLocation.countyData.armiesInCountyList.Count());
            if(battleLocation.countyData.armiesInCountyList.Count() > 0)
            {
                foreach(CountyPopulation countyPopulation in battleLocation.countyData.armiesInCountyList)
                {
                    if(countyPopulation.token != null)
                    {
                        return null;
                    }
                    else
                    {
                        return countyPopulation;
                    }
                }
            }
            //GD.Print("Defending Hero List Count: " + battleLocation.countyData.herosInCountyList.Count());
            if (battleLocation.countyData.herosInCountyList.Count() > 0)
            {
                foreach (CountyPopulation countyPopulation in battleLocation.countyData.herosInCountyList)
                {
                    if (countyPopulation.isFactionLeader == true)
                    {
                        countyPopulation.ChangeToArmy();
                        return countyPopulation;
                    }
                    else
                    {
                        if (countyPopulation.loyaltyAttribute > Globals.Instance.loyaltyCheckNumber)
                        {
                            countyPopulation.ChangeToArmy();
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
                //GD.Print("Defenders Faction Name: " + battleLocation.countyData.factionData.factionName);
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
                        possibleDefenders = possibleDefenders.OrderByDescending(countyPopulation => countyPopulation.coolSkill.skillLevel)
                                     .ThenByDescending(countyPopulation => countyPopulation.rifleSkill.skillLevel)
                                     .ToList();
                        foreach (CountyPopulation countyPopulation in possibleDefenders)
                        {
                            GD.Print($"{countyPopulation.firstName} {countyPopulation.coolSkill.skillLevel} " +
                                $"{countyPopulation.rifleSkill.skillName}");
                        }
                        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(possibleDefenders[0].location);
                        selectCounty.countyData.herosInCountyList.Add(possibleDefenders[0]);
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