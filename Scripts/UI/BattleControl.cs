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

        SelectToken countyAttackerSelectToken;
        SelectToken countyDefendersSelectToken;

        private List<string> combatLogs;
        public void StartBattle()
        {
            SelectCounty selectCounty = (SelectCounty)GetParent().GetParent();

            // Attackers Army
            foreach (CountyPopulation attackerCountyPopulation in selectCounty.countyData.visitingPopulationList)
            {
                if (attackerCountyPopulation.IsArmyLeader == true && attackerCountyPopulation.token != null)
                {
                    countyAttackerSelectToken = attackerCountyPopulation.token;
                    countyAttackerSelectToken.Hide();
                    attackerTokenTextureRect.Texture = countyAttackerSelectToken.unselectedTexture;
                    attackerMoraleLabel.Text = countyAttackerSelectToken.countyPopulation.moraleExpendable.ToString();
                    break;
                }
            }

            // Defenders Army
            foreach (CountyPopulation defenderCountyPopulation in selectCounty.countyData.heroCountyList)
            {
                if (defenderCountyPopulation.IsArmyLeader == true && defenderCountyPopulation.token != null)
                {
                    countyDefendersSelectToken = defenderCountyPopulation.token;
                    countyDefendersSelectToken.Hide();
                    defenderTokenTextureRect.Texture = countyDefendersSelectToken.unselectedTexture;
                    defenderMoraleLabel.Text = countyDefendersSelectToken.countyPopulation.moraleExpendable.ToString();
                    break;
                }
            }
            Show();
            SubscribeToHourChange();
        }

        private void SubscribeToHourChange()
        {
            Clock.Instance.HourChanged += HourlyBattleInCounty;
            countyAttackerSelectToken.currentBattle = this;
        }
        private void HourlyBattleInCounty()
        {
            GD.Print("Hourly Battle.");

            // County defender attacks county attacker.
            Attack(countyAttackerSelectToken.countyPopulation, countyDefendersSelectToken.countyPopulation, false);

            // County attacker attacks county defender.
            Attack(countyDefendersSelectToken.countyPopulation, countyAttackerSelectToken.countyPopulation, true);

            ContinueBattleCheck();
        }

        private void ContinueBattleCheck()
        {
            // Both have zero morale.
            if(countyAttackerSelectToken.countyPopulation.moraleExpendable == 0 
                && countyDefendersSelectToken.countyPopulation.moraleExpendable == 0)
            {
                ArmyFlees(countyAttackerSelectToken.countyPopulation);
            }
            // Attacker has zero morale.
            if(countyAttackerSelectToken.countyPopulation.moraleExpendable == 0)
            {
                ArmyFlees(countyAttackerSelectToken.countyPopulation);
            }
            // Defender has zero morale.
            if(countyDefendersSelectToken.countyPopulation.moraleExpendable == 0)
            {
                ArmyFlees(countyDefendersSelectToken.countyPopulation);
            }
        }

        private void ArmyFlees(CountyPopulation countyPopulation)
        {
            if(countyPopulation.lastLocation != -1)
            {
                countyPopulation.token.tokenMovement
                    .StartMove(countyPopulation.lastLocation);
                EndBattle();
            }
            else
            {
                RandomNeighborMove(countyPopulation);
            }
        }

        private void RandomNeighborMove(CountyPopulation countyPopulation)
        {
            GD.Print("Random Neighors Move.");
            SelectCounty selectCounty = (SelectCounty)Globals.Instance.countiesParent.GetChild(countyPopulation.location);
            List<SelectCounty> countyNeighbors = selectCounty.neighborCounties;
            SelectCounty destinationCounty = FindFactionOwnedNeighborCounty(countyNeighbors, countyPopulation);
            if(destinationCounty != null)
            {
                countyPopulation.token.tokenMovement.StartMove(destinationCounty.countyData.countyId);
                CountyCaptured();
            }
            else
            {
                CountyCaptured();
            }
        }

        private SelectCounty FindFactionOwnedNeighborCounty(List<SelectCounty> countyNeighbors, CountyPopulation countyPopulation)
        {
            List<SelectCounty> eligibleCounties = countyNeighbors
                .Where(c => c.countyData.factionData == countyPopulation.factionData)
                .ToList();

            if (eligibleCounties.Count > 0)
            {
                int randomIndex = Globals.Instance.random.Next(0, eligibleCounties.Count);
                SelectCounty chosenCounty = eligibleCounties[randomIndex];

                countyPopulation.destination = chosenCounty.countyData.countyId;
                countyPopulation.token.tokenMovement.StartMove(countyPopulation.destination);

                return chosenCounty;
            }
            else
            {
                return null;
            }
        }

        private void CountyCaptured()
        {
            GD.PrintRich("[rainbow]County Captured.");
            EndBattle();
        }

        public void EndBattle()
        {
            Clock.Instance.HourChanged -= HourlyBattleInCounty;
            Hide();
        }

        // This is confusing.  Needs a fucking rewrite.
        private void Attack(CountyPopulation gettingShotAtCountyPopulation, CountyPopulation shootingCountyPopulation, bool isAttacker)
        {
            int attackRoll = random.Next(1, 101);
            BattleLogControl.Instance.AddLog($"{shootingCountyPopulation.firstName} {shootingCountyPopulation.lastName} rifle skill: " +
                $"{shootingCountyPopulation.rifleSkill} vs attack roll: {attackRoll}", isAttacker);

            if (shootingCountyPopulation.rifleSkill > attackRoll)
            {
                BattleLogControl.Instance.AddLog($"{shootingCountyPopulation.firstName} {shootingCountyPopulation.lastName} has hit!", isAttacker);
                //GD.Print($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} has hit!");
                int coolRoll = random.Next(1, 101);
                if (coolRoll > gettingShotAtCountyPopulation.coolSkill)
                {
                    int moraleDamage = random.Next(Globals.Instance.moraleDamageMin, Globals.Instance.moraleDamageMax);
                    gettingShotAtCountyPopulation.moraleExpendable 
                        = Math.Max(gettingShotAtCountyPopulation.moraleExpendable - moraleDamage, 0);
                    BattleLogControl.Instance.AddLog($"{gettingShotAtCountyPopulation.firstName} " +
                        $"{gettingShotAtCountyPopulation.lastName} has failed their cool roll!  They have lost {moraleDamage}", !isAttacker);
                }
                attackerMoraleLabel.Text = countyAttackerSelectToken.countyPopulation.moraleExpendable.ToString();
                defenderMoraleLabel.Text = countyDefendersSelectToken.countyPopulation.moraleExpendable.ToString();
            }
            else
            {
                BattleLogControl.Instance.AddLog($"{shootingCountyPopulation.firstName} {shootingCountyPopulation.lastName} has missed!", isAttacker);
            }
        }
        private void ButtonUp()
        {
            GD.Print("Battle Log Control Clicked.");
            PlayerUICanvas.Instance.BattleLogControl.Show();
        }


    }
}