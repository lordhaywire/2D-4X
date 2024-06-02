using Godot;

namespace PlayerSpace
{
    public partial class Faction : Node
    {
        [Export] public FactionData factionData;

        public override void _Ready()
        {
            CallDeferred(nameof(SubscribeToEvents));
        }

        private void SubscribeToEvents()
        {
            Clock.Instance.FirstRun += DayStart;
            Clock.Instance.HourZero += DayStart;
        }
        private void DayStart()
        {
            FactionAI factionAI = new();
            Banker banker = new();
            Research research = new();
            banker.GenerateLeaderInfluence();
            research.ApplyHeroResearch(factionData);
            factionAI.AssignResearch(factionData);
        }

        private void OnTreeExit()
        {
            Clock.Instance.HourZero -= DayStart;
        }
    }
}