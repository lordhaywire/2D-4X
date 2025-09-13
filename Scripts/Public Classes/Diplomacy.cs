using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace PlayerSpace;

public class Diplomacy
{
    public static bool IsFactionAtWar(FactionData factionData, FactionData otherFactionData)
    {
        return factionData.diplomacyMatrices[otherFactionData.factionId].AtWar;
    }

    public static void CreateWar(FactionData aggressorFactionData, FactionData defenderFactionData)
    {
        War war = new()
        {
            aggressorFactionData = aggressorFactionData,
            defenderFactionData = defenderFactionData
        };

        aggressorFactionData.diplomacyMatrices[war.defenderFactionData.factionId].AtWar = true;
        defenderFactionData.diplomacyMatrices[war.aggressorFactionData.factionId].AtWar = true;
        // Add the wars to the factions so they know what wars they are in.
        aggressorFactionData.wars.Add(war);
        defenderFactionData.wars.Add(war);

        GD.Print($"{war.aggressorFactionData.factionName} has declared war on {war.defenderFactionData.factionName}.");
        EventLog.Instance.AddLog(
            $"{war.aggressorFactionData.factionName} {TranslationServer.Translate("PHRASE_HAS_DECLARED_WAR")} {war.defenderFactionData.factionName}.");

        RespondToDeclarationOfWar(war);
    }

    public static void EndWar(FactionData aggressorFactionData, FactionData defenderFactionData)
    {
    }

    private static void RespondToDeclarationOfWar(War war)
    {
        GD.Print($"{war.defenderFactionData.factionName} is responding to the declaration of war.");
        foreach (CountyData countyData in war.defenderFactionData.countiesFactionOwns)
        {
            CheckForAndSpawnDefendingHeroes(countyData.countyNode);
            EventLog.Instance.AddLog($"{war.defenderFactionData.factionName}" +
                                     $" is raising armies at {countyData.countyName}!");
        }
    }

    /// <summary>
    /// We need them to spawn closest to the enemy armies first.
    /// </summary>
    /// <param name="battleLocation"></param>
    private static void CheckForAndSpawnDefendingHeroes(County battleLocation)
    {
        GD.Print("Defending Heroes List Count: " + battleLocation.countyData.heroesInCountyList.Count);

        // If the faction doesn't have the influence to hire a hero, then just get the fuck out of this method.
        FactionData factionData =
            SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(battleLocation.countyData
                .factionId);
        if (factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount
            < Globals.Instance.costOfHero)
        {
            return;
        }

        List<PopulationData> possibleDefenders = [];

        foreach (PopulationData possibleDefender in battleLocation.countyData.heroesInCountyList)
        {
            if (possibleDefender.LoyaltyAdjusted > Globals.Instance.loyaltyCheckNumber)
            {
                possibleDefenders.Add(possibleDefender);
            }
            else
            {
                // Make the low loyalty heroes flee the county.
            }

            if (possibleDefenders.Count > 0)
            {
                // Order the possbileDefenders list by highest cool and rifle skill.
                possibleDefenders =
                [
                    .. possibleDefenders.OrderByDescending(populationData
                            => populationData.skills[AllEnums.Skills.Cool].skillLevel)
                        .ThenByDescending(populationData
                            => populationData.skills[AllEnums.Skills.Rifle].skillLevel)
                ];

                if (possibleDefenders[0]?.heroToken == null)
                {
                    TokenSpawner.Spawn(
                        Globals.Instance.GetCountyDataFromLocationId(possibleDefenders[0].location).countyNode,
                        possibleDefenders[0]);
                    // Have the hero start recruiting max suboridinates, and equip best equipment?  Maybe equipment should be set by the AI personality.
                    return;
                }
            }
            else
            {
                if (battleLocation.countyData.populationDataList.Count == 0)
                    return;

                PopulationData highestLoyaltyPopulation = battleLocation.countyData.populationDataList
                    .OrderByDescending(p => p.LoyaltyAdjusted)
                    .First();
                highestLoyaltyPopulation.ConvertPopulationToAide();
            }


            GD.Print("Defending Hero List Count: " + battleLocation.countyData.heroesInCountyList.Count);
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
            }

            else
            {
                //GD.Print("Not enough influence to hire a hero for defense.");
                return null;
            }
        }

        return null;
        */
    }

/*
public void DeclareWarConfirmation(CountyData countyData)
{
    DeclareWarControl.Instance.Show();
    FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(countyData.factionId);
    DeclareWarControl.Instance.declareWarTitleLabel.Text
        = $"{TranslationServer.Translate("PHRASE_DECLARE_WAR_CONFIRMATION")} {factionData.factionName}";
}
*/
}