using Godot;
using System;

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
            Clock.Instance.SetDay += EndOfDay;
            Clock.Instance.SetDay += DayStart;
        }

        private void EndOfDay()
        {
            Banker banker = new();

            factionData.SubtractFactionResources();
            factionData.CopyFactionResourcesToYesterday();
            banker.AddLeaderInfluence(factionData);

            // Goes through each hero and checks to see if they are researching and if they are then
            // it does a skill check and skill learning roll.
            // If the research is done it also makes them idle.
            banker.AddHeroResearch(factionData);

            TopBarControl.Instance.UpdateResourceLabels();
        }

        private void DayStart()
        {
            FactionAI factionAI = new();
            factionAI.AssignResearch(factionData);

            /* This is just for testing.
            foreach (ResearchItemData researchItemData in factionData.researchItems)
            {
                GD.Print($"{factionData.factionName} research in " +
                    $"{researchItemData.researchName}: {researchItemData.AmountOfResearchDone}");
            }
            */
        }

        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}