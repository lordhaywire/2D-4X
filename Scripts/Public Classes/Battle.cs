using System;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public class Battle(CountyData battleLocation, int attackerFactionId)
{
    public readonly CountyData battleLocation = battleLocation;

    public int attackerFactionId = attackerFactionId;
    // No need for the defender Faction ID because it can always be gotten from the battle location.

    public Godot.Collections.Array<PopulationData> attackingArmy = [];
    public Godot.Collections.Array<PopulationData> defendingArmy = [];
    public Godot.Collections.Array<PopulationData> attackingMia = [];
    public Godot.Collections.Array<PopulationData> defendingMia = [];

    public void SortArmiesListBestHeroFirst()
    {
        attackingArmy = new Godot.Collections.Array<PopulationData>(
            attackingArmy
                .OrderByDescending(person => person.isHero) // heroes first
                .ThenByDescending(person => person.skills[AllEnums.Skills.Leadership].skillLevel
                                            + AttributeData.GetAttributeBonus(
                                                person.attributes[AllEnums.Attributes.Charisma].attributeLevel, false,
                                                false)) // sort by leadership
        );

        foreach (PopulationData attacker in attackingArmy)
        {
            GD.Print(
                $"Attacker: {attacker.GetFullName()} Leadership: {attacker.skills[AllEnums.Skills.Leadership].skillLevel}");
        }

        defendingArmy = new Godot.Collections.Array<PopulationData>(
            defendingArmy
                .OrderByDescending(person => person.isHero)
                .ThenByDescending(person => person.skills[AllEnums.Skills.Leadership].skillLevel
                                            + AttributeData.GetAttributeBonus(
                                                person.attributes[AllEnums.Attributes.Charisma].attributeLevel, false,
                                                false)) // sort by leadership
        );

        foreach (PopulationData defender in defendingArmy)
        {
            GD.Print(
                $"Defender: {defender.GetFullName()} Leadership: {defender.skills[AllEnums.Skills.Leadership].skillLevel}");
        }
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

        // Determine the corresponding MIA list
        Godot.Collections.Array<PopulationData> miaPeople =
            army == defendingArmy ? attackingMia :
            army == attackingArmy ? defendingMia :
            null;

        // See if the dead person is a hero.
        if (deadPerson.isHero)
        {
            // Add as many subordinates to the next (starting with the first one) hero's subordinate list as it can.
            foreach (PopulationData otherHero in army.Where(h => h != deadPerson))
            {
                AddSubordinatesFromOtherHero(deadPerson, otherHero);
            }

            // If there is just 1 subordinate we don't want to make them a temporary hero.
            if (deadPerson.heroSubordinates.Count > 1)
            {
                // Test to gain a temporary leader.
                PopulationData firstNonHero = army.FirstOrDefault(person => !person.isHero);

                int skillLevel = firstNonHero.skills[AllEnums.Skills.Leadership].skillLevel;
                int attributeBonus =
                    AttributeData.GetAttributeBonus(
                        firstNonHero.attributes[AllEnums.Attributes.Charisma].attributeLevel, false, false);
                int additionalBonus = 0;
                int perkBonus = PerkData.GetPerkBonus(firstNonHero, AllEnums.Perks.LeaderOfPeople);
                if (SkillData.CheckWithBonuses(skillLevel, attributeBonus, additionalBonus, perkBonus))
                {
                    // Todo: We also could add perks for temp herodom.
                    // Todo: This will make him idle, which I think is bad.
                    firstNonHero.ConvertPopulationToAide();
                    firstNonHero.RemoveSubordinateFromHeroSubordinateList(army);

                    battleLocation.visitingHeroArmyList.Add(firstNonHero);

                    // Add in as many subordinates as he can lead.
                    AddSubordinatesFromOtherHero(deadPerson, firstNonHero);
                    GD.Print($"{FactionData.GetFactionDataFromId(firstNonHero.factionId).factionName}: " +
                             $"{firstNonHero.GetFullName()} has been promoted to a temp hero.");
                    if (FactionData.GetFactionDataFromId(firstNonHero.factionId).CheckIfPlayerFaction())
                    {
                        PlayerLog.Instance.AddLog(
                            $"{firstNonHero.GetFullName()} {TranslationServer.Translate("PHRASE_HAS_BEEN_PROMOTED")}.");
                    }

                    TokenSpawner.Spawn(Globals.Instance.GetCountyDataFromLocationId(firstNonHero.Location).countyNode,
                        firstNonHero);
                }
                else
                {
                    // All left over dudes become MIA.
                    foreach (PopulationData leftOverSubordinate in deadPerson.heroSubordinates)
                    {
                        miaPeople.Add(leftOverSubordinate);
                        army.Remove(leftOverSubordinate);
                    }

                    deadPerson.heroSubordinates.Clear();
                }
            }
            else
            {
                // All left over dudes become MIA.
                foreach (PopulationData leftOverSubordinate in deadPerson.heroSubordinates)
                {
                    miaPeople.Add(leftOverSubordinate);
                    army.Remove(leftOverSubordinate);
                }

                deadPerson.heroSubordinates.Clear();
            }


            if (deadPerson.heroToken.spawnedTokenButton != null)
            {
                TokenSpawner.Unspawn(Globals.Instance.GetCountyDataFromLocationId(deadPerson.Location).countyNode,
                    deadPerson);
            }
        }

        deadPerson.DeathByCombat(AllEnums.CauseOfDeath.Bullet);
    }

    private void AddSubordinatesFromOtherHero(PopulationData startingHero, PopulationData otherHero)
    {
        int availableSubordinateSlots = otherHero.numberOfSubordinatesWanted - otherHero.heroSubordinates.Count;

        if (availableSubordinateSlots > 0)
        {
            List<PopulationData> subordinatesToMove = startingHero.heroSubordinates
                .Take(availableSubordinateSlots)
                .ToList(); // materialize the sequence

            // Take as many as fit
            foreach (PopulationData subordinate in subordinatesToMove)
            {
                otherHero.heroSubordinates.Add(subordinate);
                startingHero.heroSubordinates.Remove(subordinate);
            }

            GD.PrintRich(
                $"[rainbow]{otherHero.GetFullName()} gained {availableSubordinateSlots} new subordinates.");
        }
    }


    /// <summary>
    /// Check if the hero has a last location, if he does then check to see if it is still controlled by his faction.  It not then randomly move him.
    /// </summary>
    /// <param name="retreatingHero"></param>
    public void ArmyFlees(PopulationData retreatingHero)
    {
        retreatingHero.heroToken.isRetreating = true;
        if (retreatingHero.lastLocation == -1)
        {
            TokenMovement.RandomNeighborMove(retreatingHero);
        }
        else
        {
            County county = (County)Globals.Instance.countiesParent.GetChild(retreatingHero.lastLocation);
            FactionData lastLocationFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(county.countyData.factionId);
            FactionData retreatingHeroFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(retreatingHero.factionId);
            if (lastLocationFactionData.factionName == retreatingHeroFactionData.factionName)
            {
                retreatingHero.heroToken.tokenMovement.StartMove(retreatingHero.lastLocation);
            }
            else
            {
                TokenMovement.RandomNeighborMove(retreatingHero);
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

    public static int GetAverageArmyMorale(Godot.Collections.Array<PopulationData> army)
    {
        int morale = 0;
        int averageMorale = 0;
        foreach (PopulationData person in army)
        {
            morale += person.moraleExpendable;
        }

        if (army.Count != 0)
        {
            averageMorale = morale / army.Count;
        }

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