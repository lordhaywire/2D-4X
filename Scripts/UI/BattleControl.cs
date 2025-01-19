using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace
{
    public partial class BattleControl : Control
    {
        private readonly Random random = new();
        [ExportGroup("Tokens")]
        [Export] public Separator heroSeparator;
        [Export] public Separator armySeparator;

        [ExportGroup("War")]
        [Export] private TextureRect defenderTokenTextureRect;
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
            CountyDictator.Instance.CaptureCounty(countyDefendersSelectToken.populationData.location
                , countyAttackerSelectToken.populationData.factionData);
        }
        public void StartBattle(Battle currentbattle)
        {
            //GD.Print("Start Battle.");

            battle = currentbattle;
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
            foreach (PopulationData defenderCountyPopulation in selectCounty.countyData.armiesInCountyList)
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
            //GD.Print("Hourly Battle.");
            // County defender attacks county attacker.
            Attack(countyAttackerSelectToken.populationData, countyDefendersSelectToken.populationData, false);

            // County attacker attacks county defender.
            //countyAttackerSelectToken.populationData.moraleExpendable = 100; // This is just for testing.
            Attack(countyDefendersSelectToken.populationData, countyAttackerSelectToken.populationData, true);

            ContinueBattleCheck();
        }

        private void ContinueBattleCheck()
        {
            // Both have zero morale.
            if (countyAttackerSelectToken.populationData.moraleExpendable == 0
                && countyDefendersSelectToken.populationData.moraleExpendable == 0)
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
                return;
            }
            else
            {
                County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.lastLocation);
                if (selectCounty.countyData.factionData.factionName == populationData.factionData.factionName)
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
            //GD.Print("Random Neighors Move!");
            County selectCounty = (County)Globals.Instance.countiesParent.GetChild(populationData.location);
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
            List<County> eligibleCounties = countyNeighbors
                .Where(c => c.countyData.factionData == populationData.factionData)
                .ToList();

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
        private void Attack(PopulationData gettingShotAtCountyPopulation, PopulationData shootingCountyPopulation, bool isAttacker)
        {
            if (SkillData.Check(shootingCountyPopulation, shootingCountyPopulation.skills[AllEnums.Skills.Rifle].skillLevel
                , shootingCountyPopulation.skills[AllEnums.Skills.Rifle].attribute, false) == true)
            {
                BattleLogControl.Instance.AddLog
                    ($"{shootingCountyPopulation.firstName} {shootingCountyPopulation.lastName} {Tr("PHRASE_HAS_HIT")}.", isAttacker);
                if (SkillData.Check(gettingShotAtCountyPopulation, gettingShotAtCountyPopulation.skills[AllEnums.Skills.Cool].skillLevel
                    , gettingShotAtCountyPopulation.skills[AllEnums.Skills.Cool].attribute, false) == false)
                {
                    int moraleDamage = random.Next(Globals.Instance.moraleDamageMin, Globals.Instance.moraleDamageMax);
                    gettingShotAtCountyPopulation.moraleExpendable
                        = Math.Max(gettingShotAtCountyPopulation.moraleExpendable - moraleDamage, 0);
                    BattleLogControl.Instance.AddLog($"{gettingShotAtCountyPopulation.firstName} " +
                        $"{gettingShotAtCountyPopulation.lastName} {Tr("PHRASE_FAILED_COOL_ROLL")}.  " +
                        $"{Tr("PHRASE_MORALE_LOST")} {moraleDamage}.", !isAttacker);
                }
                else
                {
                    BattleLogControl.Instance.AddLog($"{gettingShotAtCountyPopulation.firstName} " +
                        $"{gettingShotAtCountyPopulation.lastName} {Tr("PHRASE_ISNT_SCARED")}.", !isAttacker);
                }
                attackerMoraleLabel.Text = countyAttackerSelectToken.populationData.moraleExpendable.ToString();
                defenderMoraleLabel.Text = countyDefendersSelectToken.populationData.moraleExpendable.ToString();
            }
            else
            {
                BattleLogControl.Instance.AddLog($"{shootingCountyPopulation.firstName} " +
                    $"{shootingCountyPopulation.lastName} {Tr("WORD_MISSED")}.", isAttacker);
            }

            // Check if rifle experience is learned by the attacker.
            SkillData.CheckLearning(shootingCountyPopulation, false);

            // Check if the defenders cool skill learns anything.
            SkillData.CheckLearning(gettingShotAtCountyPopulation, true);
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
}