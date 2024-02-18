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

            foreach (CountyPopulation attackerCountyPopulation in selectCounty.countyData.visitingPopulation)
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
            foreach (CountyPopulation defenderCountyPopulation in selectCounty.countyData.heroCountyPopulation)
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
            Attack(countyAttackerSelectToken.countyPopulation, countyDefendersSelectToken.countyPopulation, true);

            // County attacker attacks county defender.
            Attack(countyDefendersSelectToken.countyPopulation, countyAttackerSelectToken.countyPopulation, false);

        }

        // This is confusing.  Needs a fucking rewrite.
        private void Attack(CountyPopulation defenderCountyPopulation, CountyPopulation attackerCountyPopulation, bool isAttacker)
        {
            int attackRoll = random.Next(1, 101);
            BattleLogControl.Instance.AddLog($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} rifle skill: " +
                $"{attackerCountyPopulation.rifleSkill} vs {attackRoll}", isAttacker);
            /*
            GD.Print($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} rifle skill: " +
                $"{attackerCountyPopulation.rifleSkill} vs {attackRoll}");
            */
            if (attackerCountyPopulation.rifleSkill > attackRoll)
            {
                BattleLogControl.Instance.AddLog($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} has hit!", isAttacker);
                //GD.Print($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} has hit!");
                int coolRoll = random.Next(1, 101);
                if (defenderCountyPopulation.coolAttribute < coolRoll)
                {
                    int moraleDamage = random.Next(1, 21);
                    defenderCountyPopulation.moraleExpendable -= moraleDamage;
                }
                attackerMoraleLabel.Text = countyAttackerSelectToken.countyPopulation.moraleExpendable.ToString();
                defenderMoraleLabel.Text = countyDefendersSelectToken.countyPopulation.moraleExpendable.ToString();
            }
            else
            {
                BattleLogControl.Instance.AddLog($"{attackerCountyPopulation.firstName} {attackerCountyPopulation.lastName} has missed!", isAttacker);

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