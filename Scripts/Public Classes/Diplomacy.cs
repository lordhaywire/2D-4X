using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    // Should this be dependancy injection?
    public class Diplomacy
    {
        public void DeclareWar(War war)
        {
            //GD.Print($"{war.aggressorFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
            EventLog.Instance.AddLog($"{war.aggressorFactionData.factionName} {TranslationServer.Translate("PHRASE_HAS_DECLARED_WAR")} {war.defenderFactionData.factionName}.");
            Globals.Instance.selectedRightClickCounty.countyData
                .factionData.diplomacy.RespondToDeclarationOfWar(war);
        }

        public void DeclareWarConfirmation(CountyData countyData)
        {
            DeclareWarControl.Instance.Show();
            
            DeclareWarControl.Instance.declareWarTitleLabel.Text
                = $"{TranslationServer.Translate("PHRASE_DECLARE_WAR_CONFIRMATION")} {countyData.factionData.factionName}";
            
        }

        public void RespondToDeclarationOfWar(War war)
        {
            //GD.Print($"{war.defenderFactionData.factionName} is responding to the declaration of war.");

            war.aggressorFactionData.factionWarDictionary[war.defenderFactionData.factionName] = true;
            war.defenderFactionData.factionWarDictionary[war.aggressorFactionData.factionName] = true;
            // Add the wars to the factions so they know what wars they are in.
            war.aggressorFactionData.wars.Add(war);
            war.defenderFactionData.wars.Add(war);
        }

        public void DefenderSpawnArmies(County battleLocation)
        {
            PopulationData defenderHero = CheckForArmies(battleLocation);
            if (defenderHero != null)
            {
                // Defender's faction data.
                TokenSpawner.Spawn(battleLocation, defenderHero);
            }
            else
            {
                //GD.Print("Defender Spawn Armies - Defender Hero is null");
            }
        }

        public static PopulationData CheckForArmies(County battleLocation)
        {
            // Checkes for spawned armies.  If there is one, then it returns null, otherwise it spawns one.
            //GD.Print("Defending Army List Count: " + battleLocation.countyData.armiesInCountyList.Count());
            if(battleLocation.countyData.armiesInCountyList.Count > 0)
            {
                foreach(PopulationData populationData in battleLocation.countyData.armiesInCountyList)
                {
                    if(populationData.heroToken != null)
                    {
                        return null;
                    }
                    else
                    {
                        return populationData;
                    }
                }
            }
            //GD.Print("Defending Hero List Count: " + battleLocation.countyData.herosInCountyList.Count());
            if (battleLocation.countyData.heroesInCountyList.Count > 0)
            {
                foreach (PopulationData populationData in battleLocation.countyData.heroesInCountyList)
                {
                    if (populationData.HeroType == AllEnums.HeroType.FactionLeader)
                    {
                        populationData.ChangeToArmy();
                        return populationData;
                    }
                    else
                    {
                        if (populationData.LoyaltyAdjusted > Globals.Instance.loyaltyCheckNumber)
                        {
                            populationData.ChangeToArmy();
                            return populationData;
                        }
                        else
                        {
                            // This is wrong.  We need to make it check then rest of the population if there are no loyal heroes.
                            //GD.Print("No loyal heroes in county for defense.");
                            return null;
                        }
                    }
                }
            }
            else
            {
                //GD.Print("Defenders Faction Name: " + battleLocation.countyData.factionData.factionName);
                if (battleLocation.countyData.factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount 
                    >= Globals.Instance.costOfHero)
                {
                    List<PopulationData> possibleDefenders = [];
                    foreach (PopulationData populationData in battleLocation.countyData.populationDataList)
                    {
                        if (populationData.LoyaltyAdjusted > Globals.Instance.loyaltyCheckNumber)
                        {
                            possibleDefenders.Add(populationData);
                        }
                    }

                    if (possibleDefenders.Count > 0)
                    {
                        // Order the possbileDefenders list by highest cool and rifle skill.
                        possibleDefenders = [.. possibleDefenders.OrderByDescending(populationData 
                            => populationData.skills[AllEnums.Skills.Cool].skillLevel).ThenByDescending(populationData 
                            => populationData.skills[AllEnums.Skills.Rifle].skillLevel)];
                        foreach (PopulationData populationData in possibleDefenders)
                        {
                            //GD.Print($"{populationData.firstName} {populationData.skills[AllEnums.Skills.Cool].skillLevel} " +
                             //   $"{populationData.skills[AllEnums.Skills.Rifle].skillLevel}");
                        }
                        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(possibleDefenders[0].location);
                        selectCounty.countyData.heroesInCountyList.Add(possibleDefenders[0]);
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
                    //GD.Print("Not enough influence to hire a hero for defence.");
                    return null;
                }

            }
            return null;
        }
    }
}