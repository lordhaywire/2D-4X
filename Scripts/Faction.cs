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

            Research.CreateResearchableResearchList(factionData);

            // Assign to all heroes passive research
            Research.AssignPassiveResearch(factionData, factionData.allHeroesList);

            // Assign passive research to all County Population Research should go here.
            // This is in County.cs

            // Generate all passive research for all population including heroes.

            // Goes through each hero and checks to see if they are researching and if they are then
            // it does a skill check and skill learning roll.
            // If the research is done it also makes them idle.
            //Banker.CheckForHeroResearch(factionData);

            TopBarControl.Instance.UpdateResourceLabels();
        }

        private void DayStart()
        {
            FactionAI factionAI = new();
            if (factionData != Globals.Instance.playerFactionData)
            {
                factionAI.AssignResearch(factionData);
            }

            // This is just for testing.
            foreach (ResearchItemData researchItemData in factionData.researchItems)
            {
                //GD.Print($"{factionData.factionName} research in " +
                //    $"{researchItemData.researchName}: {researchItemData.AmountOfResearchDone}");
            }
        }

        private void OnTreeExit()
        {
            Clock.Instance.SetDay -= EndOfDay;
            Clock.Instance.SetDay -= DayStart;
        }
    }
}