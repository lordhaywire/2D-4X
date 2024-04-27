using Godot;
using System;

namespace PlayerSpace
{
    public partial class TopBarControl : Control
    {
        public static TopBarControl Instance { get; private set; }

        [Export] private Clock clock;
        [Export] private Label influenceLabel;
        [Export] private Label moneyLabel;
        [Export] private Label foodLabel;
        [Export] private Label scrapLabel;

        public override void _Ready()
        {
            Instance = this;
            Globals.Instance.playerFactionData.InfluenceChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.FoodChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.MoneyChanged += UpdateExpendables;
            Globals.Instance.playerFactionData.ScrapChanged += UpdateExpendables;
            UpdateExpendables();
        }

        public void UpdateExpendables()
        {
            //GD.Print("Expendables have been updated, motherfucker!");
            influenceLabel.Text = Globals.Instance.playerFactionData.Influence.ToString();
            moneyLabel.Text = Globals.Instance.playerFactionData.Money.ToString();
            foodLabel.Text = Globals.Instance.playerFactionData.FoodFaction.ToString();
            scrapLabel.Text = Globals.Instance.playerFactionData.ScrapFaction.ToString();
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}