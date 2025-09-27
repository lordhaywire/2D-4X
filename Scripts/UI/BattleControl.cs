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

    //private List<string> combatLogs; This is here so that I am reminded to create permanent combat logs.

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

        // Attackers Army Tokens
        foreach (PopulationData attackerPopulation in selectCounty.countyData.visitingArmyList)
        {
            countyAttackerSelectToken = attackerPopulation.heroToken;
            countyAttackerSelectToken.Hide();
            attackerTokenTextureRect.Texture = countyAttackerSelectToken.unselectedTexture;
            attackerMoraleLabel.Text = countyAttackerSelectToken.populationData.moraleExpendable.ToString();
            countyAttackerSelectToken.InCombat = true;
        }

        // Defenders Army Tokens
        foreach (PopulationData defenderPopulation in selectCounty.countyData.heroesInCountyList)
        {
            if (defenderPopulation.heroToken != null)
            {
                countyDefendersSelectToken = defenderPopulation.heroToken;
                countyDefendersSelectToken.Hide();
                defenderTokenTextureRect.Texture = countyDefendersSelectToken.unselectedTexture;
                defenderMoraleLabel.Text = countyDefendersSelectToken.populationData.moraleExpendable.ToString();
                countyDefendersSelectToken.InCombat = true;
            }
        }

        Show();
        SubscribeToHourChange();
    }

    private void AddHeroAndSubordinatesArmy(PopulationData heroPopulationData,
        Godot.Collections.Array<PopulationData> armyList)
    {
        if (!heroPopulationData.isWillingToFight) return;
        armyList.Add(heroPopulationData);
        armyList.AddRange(heroPopulationData.heroSubordinates);
    }

    private void SubscribeToHourChange()
    {
        Clock.Instance.HourChanged += HourlyBattleInCounty;
    }

    private void HourlyBattleInCounty()
    {
        GD.Print("Hourly Battle.");

        // Gather all the defenders.
        battle.defendingArmy.Clear();
        foreach (PopulationData defendingHero in battle.battleLocation.heroesInCountyList)
        {
            // Check to see if the hero is actually willing to fight, and if they are not then their subordinates don't fight either.
            AddHeroAndSubordinatesArmy(defendingHero, battle.defendingArmy);
        }

        battle.attackingArmy.Clear();
        // Gather all the attackers
        foreach (PopulationData attackingHero in battle.battleLocation.visitingArmyList)
        {
            AddHeroAndSubordinatesArmy(attackingHero, battle.attackingArmy);
        }

        // Defenders attack attackers.
        foreach (PopulationData defender in battle.defendingArmy)
        {
            int randomIndex = random.Next(0, battle.attackingArmy.Count);
            Attack(defender, battle.attackingArmy[randomIndex], false);
        }

        // Attackers attack defenders.
        foreach (PopulationData attacker in battle.attackingArmy)
        {
            if (Globals.Instance.winAllBattles)
            {
                attacker.moraleExpendable = 100; // This is just for testing. Cheat!
            }

            int randomIndex = random.Next(0, battle.defendingArmy.Count);
            Attack(attacker, battle.defendingArmy[randomIndex], true);
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
        string damageLog = "";
        string deathLog = "";

        int shooterSkillLevel = shootingPopulation.skills[AllEnums.Skills.Rifle].skillLevel;
        int shooterAttributeLevel = shootingPopulation
            .attributes[shootingPopulation.skills[AllEnums.Skills.Rifle].attribute]
            .attributeLevel;
        int shooterAttributeBonus = AttributeData.GetAttributeBonus(shooterAttributeLevel, false, false);
        // If there is no equipment then the bonus is zero.
        int shooterAdditionalBonus =
            shootingPopulation.inventory.TryGetValue(AllEnums.InventorySlot.Offensive, out var item)
                ? item?.equipmentData?.equipmentBonus ?? 0
                : 0;

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

            // Cool Check
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
                gettingShotAtPopulation.hitPoints -= damageReceived;
                damageLog = $"{gettingShotAtFactionData.factionName}: {gettingShotAtPopulation.GetFullName()} " +
                            $"{Tr("PHRASE_IS_DAMAGED_FOR")} {damageReceived} {Tr("WORD_HITPOINTS")}.";

                // Possible Death.
                if (gettingShotAtPopulation.hitPoints <= 0)
                {
                    deathLog = $" {gettingShotAtPopulation.GetFullName()} {Tr("PHRASE_IS_DEAD")}.";
                    // Cool Check for everyone on the person getting shot ats team when someone dies.
                    CheckArmyCool(battle.defendingArmy);
                    GD.Print("Someone has died.");
                    
                    // Add death.
                }
                
                // Cool Check for everyone on the person getting shot ats team when someone is hurt.
                CheckArmyCool(battle.defendingArmy);
            }
        }
        else
        {
            attackersLog = $"{shooterFactionData.factionName}: " +
                           $"{shootingPopulation.firstName} " +
                           $"{shootingPopulation.lastName} {Tr("WORD_MISSED")}.";
        }

        string finalBattleLog = $"{attackersLog} {defendersLog}";
        BattleLogControl.Instance.AddLog(finalBattleLog, false);

        if (damageLog != "")
        {
            string finalDamageLog = $"{damageLog} {deathLog}";
            BattleLogControl.Instance.AddLog(finalDamageLog, false);
        }

        // Check if the attacker learns rifle experience.
        SkillData.LearningCheck(shootingPopulation, false);

        // Check if the defender's cool skill learns anything.
        SkillData.LearningCheck(gettingShotAtPopulation, true);
    }

    private void CheckArmyCool(Godot.Collections.Array<PopulationData> army)
    {
        string combatLog = "";

        foreach (PopulationData population in army)
        {
            FactionData factionData = FactionData.GetFactionDataFromId(population.factionId);
            int skillLevel = population.skills[AllEnums.Skills.Cool].skillLevel;
            int attributeLevel = population
                .attributes[population.skills[AllEnums.Skills.Cool].attribute].attributeLevel;
            int attributeBonus =
                AttributeData.GetAttributeBonus(attributeLevel, false, false);

            if (!SkillData.CheckWithBonuses(skillLevel, attributeBonus, 0, 0)) // TODO: Perk Bonus
            {
                int moraleDamage = random.Next(Globals.Instance.moraleDamageMin, Globals.Instance.moraleDamageMax);
                population.moraleExpendable
                    = Math.Max(population.moraleExpendable - moraleDamage, 0);
                combatLog =
                    $"{factionData.factionName}: {population.GetFullName()} {Tr("PHRASE_IS_SCARED")}.";
            }

            if (combatLog != "")
            {
                BattleLogControl.Instance.AddLog(combatLog, false);
            }

            SkillData.LearningCheck(population, true);
            // TODO: Update Average Morale.
            // TODO: Have the possibility of running away somewhere.
        }
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