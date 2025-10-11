using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public class Battle(CountyData battleLocation)
{
    public readonly CountyData battleLocation = battleLocation;

    public Godot.Collections.Array<PopulationData> attackingArmy = [];
    public Godot.Collections.Array<PopulationData> defendingArmy = [];

    public void SortArmiesListBestHeroFirst()
    {
        attackingArmy = new Godot.Collections.Array<PopulationData>(
            attackingArmy
                .OrderByDescending(p => p.isHero) // heroes first
                .ThenByDescending(p =>
                    p.isHero ? p.skills[AllEnums.Skills.Leadership].skillLevel : 0) // among heroes, by leadership
        );

        defendingArmy = new Godot.Collections.Array<PopulationData>(
            defendingArmy
                .OrderByDescending(p => p.isHero)
                .ThenByDescending(p => p.isHero ? p.skills[AllEnums.Skills.Leadership].skillLevel : 0)
        );
    }

    /// <summary>
    /// The army list should already be sorted at this point, because it is sorted every hour at the beginning of that hours battle.
    /// </summary>
    /// <param name="deadPerson"></param>
    public void SomeoneIsKilled(PopulationData deadPerson)
    {
        // Finds which army the deadPerson is in.
        Godot.Collections.Array<PopulationData> army =
            new[] { defendingArmy, attackingArmy }
                .FirstOrDefault(a => a.Contains(deadPerson));

        // Check if they are the army leader
        if (deadPerson == army[0]) // If it is null currently we want it to throw an error.
        {
            // See if there are any other heroes in the army.
            if (army[1].isHero)
            {
                // Add as many subordinates to this hero's subordinate list as it can.
                int availableSubordinateSlots = army[1].numberOfSubordinatesWanted - army[1].heroSubordinates.Count;
                if (availableSubordinateSlots > 0)
                {
                    List<PopulationData> subordinatesToMove = army[0].heroSubordinates
                        .Take(availableSubordinateSlots)
                        .ToList(); // materialize the sequence
                    
                    // Take as many as fit
                    foreach (PopulationData subordinate in subordinatesToMove)
                    {
                        army[1].heroSubordinates.Add(subordinate);
                        army[0].heroSubordinates.Remove(subordinate);
                    }

                    GD.PrintRich($"[rainbow]{army[1].GetFullName()} gained {availableSubordinateSlots} new subordinates.");
                }
            }
        }
        
        // If they aren't the army leader, check if they are a hero
        
        // If they are a hero, but not the army leader then the army leader will absorb as many of the subordinates that the hero has.
        
        // If there are left overs then check to see if a temporary leader is generated.
        
        // If a temporary leader is generated, then have the temp leader absorb as many of the subordinates that the dead hero has.
        
        // Left over subordinates go become "detached."  Still in the army but they don't benefit from the leader bonuses.

        deadPerson.DeathByCombat(AllEnums.CauseOfDeath.Bullet);  // Todo: Clean up DeathByCombat!!!!
    }
    public void CheckIfArmyFlees(Godot.Collections.Array<PopulationData> army)
    {
        PopulationData hero = army[0];
        if (GetAverageArmyMorale(hero) <= GetMoraleLevelForFleeing(hero))
        {
            ArmyFlees(hero);
            EventLog.Instance.AddLog($"{army[0].GetFullName()} " +
                                     $"{TranslationServer.Translate("PHRASE_LOST_BATTLE")}");
        }
    }
    
    public void ArmyFlees(PopulationData populationData)
    {
        populationData.heroToken.isRetreating = true;
        if (populationData.lastLocation == -1)
        {
            TokenMovement.RandomNeighborMove(populationData);
        }
        else
        {
            County county = (County)Globals.Instance.countiesParent.GetChild(populationData.lastLocation);
            FactionData selectedFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(county.countyData.factionId);
            FactionData populationFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(populationData.factionId);
            if (selectedFactionData.factionName == populationFactionData.factionName)
            {
                populationData.heroToken.tokenMovement.StartMove(populationData.lastLocation);
                battleLocation.countyNode.battleControl.EndBattle();
            }
            else
            {
                TokenMovement.RandomNeighborMove(populationData);
            }
        }
    }
    
    public void ArmyCountyCaptured()
    {
        battleLocation.countyNode.battleControl.EndBattle();
        FactionData factionData =
            SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(attackingArmy[0]
                .factionId);
        CountyDictator.CaptureCounty(defendingArmy[0].Location, factionData);
    }
    
    public static int GetAverageArmyMorale(PopulationData heroPopulationData)
    {
        int morale = 0;
        foreach (PopulationData subordinateData in heroPopulationData.heroSubordinates)
        {
            morale += subordinateData.moraleExpendable;
        }

        morale += heroPopulationData.moraleExpendable;
        int averageMorale = morale / (heroPopulationData.heroSubordinates.Count + 1);
        return averageMorale;
    }

    /// <summary>
    /// We will eventually have the leader of the army modify Globals.Instance.baseAverageArmyMoraleFlees.
    /// </summary>
    /// <param name="heroPopulationData"></param>
    /// <returns></returns>
    public static int GetMoraleLevelForFleeing(PopulationData heroPopulationData)
    {
        return Globals.Instance.baseAverageArmyMoraleFlees;
    }

    public static int GenerateCombatDamage()
    {
        Random random = new();
        return random.Next(1, Autoload.Instance.startingHitPoints + 1);
    }
}