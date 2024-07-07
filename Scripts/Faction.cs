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
            banker.AddHeroResearch(factionData);
            TopBarControl.UpdateFactionResources(); // This method should under FactionData or the Banker.
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