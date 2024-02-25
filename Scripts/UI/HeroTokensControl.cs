using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace
{
    public partial class HeroTokensControl : Control
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
            // Attackers Army
            SelectCounty selectCounty = (SelectCounty)GetParent().GetParent();

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
            Clock.Instance.HourChanged += BattleInCounty;
        }
        private void BattleInCounty()
        {
            GD.Print("Hourly Battle.");

            // County defender attacks county attacker.
            Attack(countyAttackerSelectToken.countyPopulation, countyDefendersSelectToken.countyPopulation, false);

            // County attacker attacks county defender.
            Attack(countyDefendersSelectToken.countyPopulation, countyAttackerSelectToken.countyPopulation, true);

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
                    gettingShotAtCountyPopulation.moraleExpendable -= moraleDamage;
                    BattleLogControl.Instance.AddLog($"{gettingShotAtCountyPopulation.firstName} " +
                        $"{gettingShotAtCountyPopulation.lastName} has failed their cool roll!  They have lost {moraleDamage}", !isAttacker);
                }
                attackerMoraleLabel.Text = countyAttackerSelectToken.countyPopulation.moraleExpendable.ToString();
                defenderMoraleLabel.Text = countyDefendersSelectToken.countyPopulation.moraleExpendable.ToString();
            }
            else
            {
                BattleLogControl.Instance.AddLog($"{shootingCountyPopulation.firstName} {shootingCountyPopulation.lastName} has missed!", isAttacker);

                //GD.Print("Defender has missed!");
            }
        }
        private void ButtonUp()
        {
            GD.Print("Battle Control Clicked.");
            PlayerUICanvas.Instance.BattleLogControl.Show();
        }


    }
}