using Godot;
using System;

namespace PlayerSpace
{
    public partial class FactionNode : Node
    {
        [Export] public FactionData factionData;

        public override void _Ready()
        {
            CallDeferred(nameof(SubscribeToEvents));
        }

        private void SubscribeToEvents()
        {
            Clock.Instance.HourZero += DayStart;
        }
        private void DayStart()
        {
            Banker banker = new();
            banker.GenerateLeaderInfluence();
        }

        private void OnTreeExit()
        {
            Clock.Instance.HourZero -= DayStart;
        }
    }
}