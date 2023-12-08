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
            UpdateExpendables();
        }


        public void UpdateExpendables()
        {
            GD.Print("Expendables have been updated, motherfucker!");
            influenceLabel.Text = Globals.Instance.playerFactionData.Influence.ToString();
            moneyLabel.Text = Globals.Instance.playerFactionData.money.ToString();
            foodLabel.Text = Globals.Instance.playerFactionData.food.ToString();
            scrapLabel.Text = Globals.Instance.playerFactionData.scrap.ToString();
        }

        public void ChangeSpeed(int speed)
        {
            clock.ChangeSpeed(speed);
        }
    }
}