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
        War currentWar = new();
        foreach (War war in aggressorFactionData.wars)
        {
            if (war.defenderFactionData.factionId == defenderFactionData.factionId)
            {
                currentWar = war;
            }
        }

        aggressorFactionData.wars.Remove(currentWar);
        defenderFactionData.wars.Remove(currentWar);

        aggressorFactionData.diplomacyMatrices[defenderFactionData.factionId].AtWar = false;
        defenderFactionData.diplomacyMatrices[aggressorFactionData.factionId].AtWar = false;

        GD.Print($"{aggressorFactionData.factionName} has ended war on {defenderFactionData.factionName}.");
        EventLog.Instance.AddLog(
            $"{aggressorFactionData.factionName} {TranslationServer.Translate("PHRASE_HAS_ENDED_WAR_WITH")} {defenderFactionData.factionName}.");

        RespondToEndOfWar();
    }

    private static void RespondToEndOfWar()
    {
        GD.Print("At some point we need to add the RespondToEndOfWar().");
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
    /// Todo: We need them to spawn closest to the enemy armies first.
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
        }

        if (possibleDefenders.Count < 1 && battleLocation.countyData.populationDataList.Count > 0)
        {
            PopulationData highestLoyaltyPopulation = battleLocation.countyData.populationDataList
                .Where(p => p.LoyaltyAdjusted > Globals.Instance.loyaltyCheckNumber)
                .OrderByDescending(p => p.LoyaltyAdjusted)
                .FirstOrDefault();

            if (highestLoyaltyPopulation != null)
            {
                highestLoyaltyPopulation.ConvertPopulationToAide();
                possibleDefenders.Add(highestLoyaltyPopulation);
            }
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
                    Globals.Instance.GetCountyDataFromLocationId(possibleDefenders[0].Location).countyNode,
                    possibleDefenders[0]);
                possibleDefenders[0].isWillingToFight = true;
            }
            
        }
    
        // Then the hero starts recruiting.
        if (possibleDefenders.Count > 0)
        {
            possibleDefenders[0].numberOfSubordinatesWanted =
                Recruiter.GetMaxNumberOfRecruits(possibleDefenders[0]);
        }
    }
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
