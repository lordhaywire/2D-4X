using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class BattleControl : Control
{
    private readonly Random random = new();

    [ExportGroup("Tokens")] [Export] public Separator heroSeparator;
    [Export] public Separator armySeparator;

    [ExportGroup("War")] [Export] private TextureRect defenderTokenTextureRect;
    [Export] private TextureRect attackerTokenTextureRect;

    [Export] private Label defenderMoraleLabel;
    [Export] private Label attackerMoraleLabel;

    private HeroToken countyAttackerSelectToken;
    private HeroToken countyDefendersSelectToken;

    private Battle battle;

    //private List<string> combatLogs;

    private void CountyCaptured()
    {
        EndBattle();
        FactionData factionData =
            SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(countyAttackerSelectToken.populationData
                .factionId);
        CountyDictator.Instance.CaptureCounty(countyDefendersSelectToken.populationData.Location, factionData);
    }

    public void StartBattle(Battle currentBattle)
    {
        GD.Print("Start Battle.");

        battle = currentBattle;
        County selectCounty = (County)GetParent().GetParent();


        // How could any of the token's ever be equal to null?
        // Attackers Army
        foreach (PopulationData attackerCountyPopulation in selectCounty.countyData.visitingArmyList)
        {
            if (attackerCountyPopulation.heroToken != null)
            {
                countyAttackerSelectToken = attackerCountyPopulation.heroToken;
                countyAttackerSelectToken.Hide();
                attackerTokenTextureRect.Texture = countyAttackerSelectToken.unselectedTexture;
                attackerMoraleLabel.Text = countyAttackerSelectToken.populationData.moraleExpendable.ToString();
                countyAttackerSelectToken.InCombat = true;
                break;
            }
        }

        // Defenders Army
        foreach (PopulationData defenderCountyPopulation in selectCounty.countyData.heroesInCountyList)
        {
            if (defenderCountyPopulation.heroToken != null)
            {
                countyDefendersSelectToken = defenderCountyPopulation.heroToken;
                countyDefendersSelectToken.Hide();
                defenderTokenTextureRect.Texture = countyDefendersSelectToken.unselectedTexture;
                defenderMoraleLabel.Text = countyDefendersSelectToken.populationData.moraleExpendable.ToString();
                countyDefendersSelectToken.InCombat = true;
                break;
            }
        }

        Show();
        SubscribeToHourChange();
    }

    private void SubscribeToHourChange()
    {
        Clock.Instance.HourChanged += HourlyBattleInCounty;
    }

    private void HourlyBattleInCounty()
    {
        GD.Print("Hourly Battle.");
        Godot.Collections.Array<PopulationData> defenders = [];
        Godot.Collections.Array<PopulationData> attackers = [];

        // Gather all the defenders.
        foreach (PopulationData defendingHero in battle.battleLocation.heroesInCountyList)
        {
            // Check to see if the hero is actually willing to fight, and if they are not then their subordinates don't fight either.
            if (defendingHero.isWillingToFight)
            {
                defenders.Add(defendingHero);
                defenders.AddRange(defendingHero.heroSubordinates);
            }
        }

        // Gather all the attackers
        foreach (PopulationData attackingHero in battle.battleLocation.visitingArmyList)
        {
            attackers.Add(attackingHero);
            attackers.AddRange(attackingHero.heroSubordinates);
        }

        // Defenders attack attackers.
        foreach (PopulationData defender in defenders)
        {
            int randomIndex = random.Next(0, attackers.Count);
            Attack(defender, attackers[randomIndex], false);
        }

        // Attackers attack defenders.
        foreach (PopulationData attacker in attackers)
        {
            if (Globals.Instance.winAllBattles)
            {
                attacker.moraleExpendable = 100; // This is just for testing. Cheat!
            }

            int randomIndex = random.Next(0, defenders.Count);
            Attack(attacker, defenders[randomIndex], true);
        }

        ContinueBattleCheck();
    }

    private void ContinueBattleCheck()
    {
        // Todo: Add hourly leadership checks to restore a tiny bit of morale to each fighter.
        // Todo: Add a cool check if someone runs away.

        int attackerAverageMorale = Battle.GetAverageArmyMorale(countyAttackerSelectToken.populationData);
        int defenderAverageMorale = Battle.GetAverageArmyMorale(countyDefendersSelectToken.populationData);

        // Both have zero morale.
        if (attackerAverageMorale == 0 && defenderAverageMorale == 0)
        {
            ArmyFlees(countyAttackerSelectToken.populationData);
            EventLog.Instance.AddLog($"{countyAttackerSelectToken.populationData.firstName} " +
                                     $"{countyAttackerSelectToken.populationData.lastName} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }

        // Attacker has zero morale.
        if (countyAttackerSelectToken.populationData.moraleExpendable == 0)
        {
            ArmyFlees(countyAttackerSelectToken.populationData);
            EventLog.Instance.AddLog($"{countyAttackerSelectToken.populationData.firstName} " +
                                     $"{countyAttackerSelectToken.populationData.lastName} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }

        // Defender has zero morale.
        if (countyDefendersSelectToken.populationData.moraleExpendable == 0)
        {
            ArmyFlees(countyDefendersSelectToken.populationData);
            EventLog.Instance.AddLog($"{countyDefendersSelectToken.populationData.firstName} " +
                                     $"{countyDefendersSelectToken.populationData.lastName} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }
    }

    private void ArmyFlees(PopulationData populationData)
    {
        populationData.heroToken.isRetreating = true;
        if (populationData.lastLocation == -1)
        {
            RandomNeighborMove(populationData);
        }
        else
        {
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.lastLocation);
            FactionData selectedFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(selectCounty.countyData.factionId);
            FactionData populationFactionData =
                SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(populationData.factionId);
            if (selectedFactionData.factionName == populationFactionData.factionName)
            {
                populationData.heroToken.tokenMovement.StartMove(populationData.lastLocation);
                EndBattle();
            }
            else
            {
                RandomNeighborMove(populationData);
            }
        }
    }

    private void RandomNeighborMove(PopulationData populationData)
    {
        //GD.Print("Random Neighbors Move!");
        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.Location);
        List<County> countyNeighbors = selectCounty.neighborCounties;
        County destinationCounty = FindFactionOwnedNeighborCounty(countyNeighbors, populationData);
        if (destinationCounty != null)
        {
            //GD.Print("Destination County: " + destinationCounty.countyData.countyName);
            populationData.heroToken.tokenMovement.StartMove(destinationCounty.countyData.countyId);
            CountyCaptured();
        }
        else
        {
            CountyCaptured();
        }
    }

    private static County FindFactionOwnedNeighborCounty(List<County> countyNeighbors, PopulationData populationData)
    {
        List<County> eligibleCounties =
            [.. countyNeighbors.Where(c => c.countyData.factionId == populationData.factionId)];

        if (eligibleCounties.Count > 0)
        {
            int randomIndex = Globals.Instance.random.Next(0, eligibleCounties.Count);
            County chosenCounty = eligibleCounties[randomIndex];

            populationData.destination = chosenCounty.countyData.countyId;
            populationData.heroToken.tokenMovement.StartMove(populationData.destination);

            return chosenCounty;
        }
        else
        {
            return null;
        }
    }

    public void EndBattle()
    {
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
        battle.battleLocation.battles.Remove(battle);
        countyAttackerSelectToken.InCombat = false;
        countyDefendersSelectToken.InCombat = false;
        Hide();
    }

    // This is confusing.  Needs a fucking rewrite.
    private void Attack(PopulationData shootingPopulation, PopulationData gettingShotAtPopulation, bool isAttacker)
    {
        string attackersLog;
        string defendersLog = "";

        int shooterSkillLevel = shootingPopulation.skills[AllEnums.Skills.Rifle].skillLevel;
        int shooterAttributeLevel = shootingPopulation
            .attributes[shootingPopulation.skills[AllEnums.Skills.Rifle].attribute]
            .attributeLevel;
        int shooterAttributeBonus = AttributeData.GetAttributeBonus(shooterAttributeLevel, false, false);
        int shooterAdditionalBonus =
            shootingPopulation.inventory[AllEnums.InventorySlot.Offensive].equipmentData.equipmentBonus;
        FactionData shooterFactionData = FactionData.GetFactionDataFromId(shootingPopulation.factionId);
        FactionData gettingShotAtFactionData = FactionData.GetFactionDataFromId(gettingShotAtPopulation.factionId);

        // First skill check for to hit.  If this passes then the attacker's shot has gotten close to their target, and target
        // must roll their cool.  The second skill check for the attacker is to see if they actually hit and damaged the target.
        if (SkillData.CheckWithBonuses(shooterSkillLevel, shooterAttributeBonus, shooterAdditionalBonus,
                0)) // TODO: Perk Bonus
        {
            // Check Defenders Morale
            int gettingShotAtSkillLevel = gettingShotAtPopulation.skills[AllEnums.Skills.Cool].skillLevel;
            int gettingShotAtAttributeLevel = gettingShotAtPopulation
                .attributes[gettingShotAtPopulation.skills[AllEnums.Skills.Cool].attribute].attributeLevel;
            int gettingShotAtAttributeBonus =
                AttributeData.GetAttributeBonus(gettingShotAtAttributeLevel, false, false);

            attackersLog =
                $"{shooterFactionData.factionName}: {shootingPopulation.GetFullName()} {Tr("PHRASE_IS_SHOOTING_AT")} {gettingShotAtPopulation.GetFullName()}.";

            if (!SkillData.CheckWithBonuses(gettingShotAtSkillLevel, gettingShotAtAttributeBonus, 0,
                    0)) // TODO: Perk Bonus
            {
                int moraleDamage = random.Next(Globals.Instance.moraleDamageMin, Globals.Instance.moraleDamageMax);
                gettingShotAtPopulation.moraleExpendable
                    = Math.Max(gettingShotAtPopulation.moraleExpendable - moraleDamage, 0);

                defendersLog = $"{gettingShotAtFactionData.factionName}: " +
                               $"{gettingShotAtPopulation.firstName} " +
                               $"{gettingShotAtPopulation.lastName} {Tr("PHRASE_FAILED_COOL_ROLL")}.  " +
                               $"{Tr("PHRASE_MORALE_LOST")} {moraleDamage}.";
            }
            else
            {
                defendersLog = $"{gettingShotAtFactionData.factionName}: " +
                               $"{gettingShotAtPopulation.firstName} " +
                               $"{gettingShotAtPopulation.lastName} {Tr("PHRASE_ISNT_SCARED")}.";
            }

            attackerMoraleLabel.Text = countyAttackerSelectToken.populationData.moraleExpendable.ToString();
            defenderMoraleLabel.Text = countyDefendersSelectToken.populationData.moraleExpendable.ToString();
            // Second skill check to see if they damaged the person they shot at.
            if (SkillData.CheckWithBonuses(shooterSkillLevel, shooterAttributeBonus, shooterAdditionalBonus,
                    0)) // TODO: Perk Bonus
            {
                // Apply Damage.
                int damageReceived = Battle.GenerateCombatDamage();
                gettingShotAtPopulation.hitPoints += damageReceived;
                // Cool Check for everyone on the person getting shot ats team.
                CheckArmyCool();
                // Possible Death.
                if (gettingShotAtPopulation.hitPoints == 0)
                {
                    // Second cool check.
                }
            }
        }
        else
        {
            attackersLog = $"{shooterFactionData.factionName}: " +
                           $"{shootingPopulation.firstName} " +
                           $"{shootingPopulation.lastName} {Tr("WORD_MISSED")}.";
        }

        string finalLog = $"{attackersLog} {defendersLog}";
        BattleLogControl.Instance.AddLog(finalLog, false);
        // Check if the attacker learns rifle experience.
        SkillData.LearningCheck(shootingPopulation, false);

        // Check if the defender's cool skill learns anything.
        SkillData.LearningCheck(gettingShotAtPopulation, true);
    }

    private void CheckArmyCool()
    {
        throw new NotImplementedException();
    }

    private static void ButtonUp()
    {
        //GD.Print("Battle Log Control Clicked.");
        PlayerUICanvas.Instance.BattleLogControl.Show();
    }

    private void OnTreeExit()
    {
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
    }
}