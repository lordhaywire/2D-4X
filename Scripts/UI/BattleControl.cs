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

    public void StartBattle(Battle currentBattle)
    {
        GD.Print("Start Battle.");

        battle = currentBattle;
        CheckForHeroButtons();

        Show();
        SubscribeToHourChange();
    }

    private void CheckForHeroButtons()
    {
        County county = (County)GetParent().GetParent();

        // Attackers Army Tokens
        foreach (PopulationData attackerPopulation in county.countyData.visitingHeroArmyList)
        {
            countyAttackerSelectToken = attackerPopulation.heroToken;
            countyAttackerSelectToken.Hide();
            attackerTokenTextureRect.Texture = countyAttackerSelectToken.unselectedTexture;
            attackerMoraleLabel.Text = countyAttackerSelectToken.populationData.moraleExpendable.ToString();
            countyAttackerSelectToken.InCombat = true;
        }

        // Defenders Army Tokens
        foreach (PopulationData defenderPopulation in county.countyData.heroesInCountyList)
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

    /// <summary>
    /// We do this every hour because other armies may join the battle.
    /// </summary>
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

        // Gather all the attackers
        battle.attackingArmy.Clear();
        foreach (PopulationData attackingHero in battle.battleLocation.visitingHeroArmyList)
        {
            AddHeroAndSubordinatesArmy(attackingHero, battle.attackingArmy);
        }

        // Sort armies to put the best leader first.
        battle.SortArmiesListBestHeroFirst();

        // If either army has been destroyed then we skip straight to the ContinueBattleCheck.
        // I think we will be able to get rid of this.
        if (battle.defendingArmy.Count <= 0 || battle.attackingArmy.Count <= 0)
        {
            ContinueBattleCheck();
            return;
        }

        // Defenders attack attackers.
        foreach (PopulationData defender in battle.defendingArmy)
        {
            int randomIndex = random.Next(0, battle.attackingArmy.Count);
            Attack(defender, battle.attackingArmy[randomIndex], false);
            if (battle.attackingArmy.Count == 0)
            {
                ContinueBattleCheck();
                break;
            }
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
            if (battle.defendingArmy.Count == 0)
            {
                ContinueBattleCheck();
                break;
            }
        }

        ContinueBattleCheck();
    }

    // This is confusing.  Needs a fucking rewrite.
    private void Attack(PopulationData shootingPopulation, PopulationData gettingShotAtPopulation, bool isAttacker)
    {
        Godot.Collections.Array<PopulationData> currentArmy = isAttacker ? battle.attackingArmy : battle.defendingArmy;

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

            attackerMoraleLabel.Text = Battle.GetAverageArmyMorale(battle.attackingArmy).ToString();
            defenderMoraleLabel.Text = Battle.GetAverageArmyMorale(battle.defendingArmy).ToString();
            // Second skill check to see if they damaged the person they shot at.
            if (SkillData.CheckWithBonuses(shooterSkillLevel, shooterAttributeBonus, shooterAdditionalBonus,
                    0)) // TODO: Perk Bonus
            {
                // Apply Damage.
                int damageReceived = Battle.GenerateCombatDamage();
                gettingShotAtPopulation.hitPoints -= damageReceived;
                damageLog = $"{gettingShotAtFactionData.factionName}: {gettingShotAtPopulation.GetFullName()} " +
                            $"{Tr("PHRASE_IS_DAMAGED_FOR")} {damageReceived} {Tr("WORD_HITPOINTS")}.";

                // Cool Check for everyone on the person getting shot ats team when someone is hurt.
                CheckArmyCool(currentArmy);

                // Possible Death.
                if (gettingShotAtPopulation.hitPoints <= 0)
                {
                    deathLog = $" {gettingShotAtPopulation.GetFullName()} {Tr("PHRASE_IS_DEAD")}.";

                    // Add death.
                    battle.SomeoneIsKilled(gettingShotAtPopulation);
                    CheckArmyCool(currentArmy); // Additional army check if someone dies.
                }
            }
        }
        else
        {
            attackersLog = $"{shooterFactionData.factionName}: " +
                           $"{shootingPopulation.firstName} " +
                           $"{shootingPopulation.lastName} {Tr("WORD_MISSED")}.";
        }

        string finalBattleLog = $"{attackersLog} {defendersLog}";
        BattleLogMarginContainer.Instance.AddLog(finalBattleLog);

        if (damageLog != "")
        {
            string finalDamageLog = $"{damageLog} {deathLog}";
            BattleLogMarginContainer.Instance.AddLog(finalDamageLog);
        }

        // Check if the attacker learns rifle experience.
        SkillData.LearningCheck(shootingPopulation, false);

        // Check if the defender's cool skill learns anything.
        SkillData.LearningCheck(gettingShotAtPopulation, true);
    }


    private void ContinueBattleCheck()
    {
        // Todo: Add hourly leadership checks to restore a tiny bit of morale to each fighter.

        // Check if an army is destroyed
        if (CheckIfAnArmyIsDestroyed())
        {
            return;
        }

        CheckIfAnArmyFlees();
    }

    private bool CheckIfAnArmyIsDestroyed()
    {
        FactionData attackersFaction = FactionData.GetFactionDataFromId(battle.attackerFactionId);
        FactionData defendersFaction = FactionData.GetFactionDataFromId(battle.battleLocation.factionId);

        string factionName = battle.defendingArmy.Count <= 0 ? defendersFaction.factionName : battle.attackingArmy.Count <= 0 ? attackersFaction.factionName : null;

        if (battle.defendingArmy.Count <= 0 || battle.attackingArmy.Count <= 0)
        {
            EndBattle();
            if (defendersFaction.isPlayer || attackersFaction.isPlayer)
            {
                EventLog.Instance.AddLog($"{factionName} : {battle.battleLocation.countyName} : {Tr("PHRASE_HAS_LOST_AN_ARMY")}.");
            }
            return true;
        }
        return false;
    }


    private void CheckIfAnArmyFlees()
    {
        int attackerAverageMorale = Battle.GetAverageArmyMorale(battle.attackingArmy);
        int defenderAverageMorale = Battle.GetAverageArmyMorale(battle.defendingArmy);

        // Both have low morale, so only the attacker flees.  Maybe we change this later?
        if (attackerAverageMorale <= Battle.GetMoraleLevelForFleeing(battle.attackingArmy[0])
            && defenderAverageMorale <= Battle.GetMoraleLevelForFleeing(battle.defendingArmy[0]))
        {
            battle.ArmyFlees(battle.attackingArmy[0]);
            EventLog.Instance.AddLog($"{battle.attackingArmy[0].GetFullName()} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }

        // Attacker has low morale.
        if (Battle.GetAverageArmyMorale(battle.attackingArmy) <=
            Battle.GetMoraleLevelForFleeing(battle.attackingArmy[0]))
        {
            battle.ArmyFlees(battle.attackingArmy[0]);
            EventLog.Instance.AddLog($"{battle.attackingArmy[0].GetFullName()} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }

        // Defender has low morale.
        if (Battle.GetAverageArmyMorale(battle.defendingArmy) <=
            Battle.GetMoraleLevelForFleeing(battle.defendingArmy[0]))
        {
            battle.ArmyFlees(battle.defendingArmy[0]);
            EventLog.Instance.AddLog($"{battle.defendingArmy[0].GetFullName()} " +
                                     $"{Tr("PHRASE_LOST_BATTLE")}");
        }
    }

    public void EndBattle()
    {
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
        battle.battleLocation.battles.Remove(battle);
        // TODO: Set everyone to not in combat!
        countyAttackerSelectToken.InCombat = false;
        countyDefendersSelectToken.InCombat = false;
        Hide();
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
                BattleLogMarginContainer.Instance.AddLog(combatLog);
            }

            SkillData.LearningCheck(population, true);
            // TODO: Have the possibility of running away somewhere.
        }
    }

    private void ButtonUp()
    {
        //GD.Print("Battle Log Control Clicked.");
        BattleLogMarginContainer.Instance.battle = battle;
        BattleLogMarginContainer.Instance.Show();
    }

    private void OnTreeExit()
    {
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
        Clock.Instance.HourChanged -= HourlyBattleInCounty;
    }
}