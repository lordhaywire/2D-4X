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
            FactionGeneration.Instance.playerFaction.InfluenceChanged += UpdateExpendables;
            UpdateExpendables();
        }


        public void UpdateExpendables()
        {
            GD.Print("Expendables have been updated, motherfucker!");
            influenceLabel.Text = FactionGeneration.Instance.playerFaction.Influence.ToString();
            moneyLabel.Text = FactionGeneration.Instance.playerFaction.money.ToString();
            foodLabel.Text = FactionGeneration.Instance.playerFaction.food.ToString();
            scrapLabel.Text = FactionGeneration.Instance.playerFaction.scrap.ToString();
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}