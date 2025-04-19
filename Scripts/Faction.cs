using Godot;
using System;

namespace PlayerSpace
{
    public partial class Faction : Node
    {
        [Export] public FactionData factionData;

        public override void _Ready()
        {
            // This was CallDeferred, but it should be happening before County so I changed it.  It could cause issues
            // in the future.
            //CallDeferred(nameof(SubscribeToEvents));
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            Clock.Instance.DailyHourOne += EndOfDay;
            Clock.Instance.Weekly += Weekly;
            Clock.Instance.DailyHourThree += DayStart;
            Clock.Instance.DailyHourFour += AfterDayStart;
        }

        private void AfterDayStart()
        {
            if (factionData != Globals.Instance.playerFactionData)
            {
                FactionAI.DecideIfHeroUsesNewestEquipment(this);
            }
        }

        private void Weekly()
        {
            //GD.PrintRich($"[rainbow]Faction : Weekly!!!!!");
        }

        private void EndOfDay()
        {
            //GD.PrintRich($"[rainbow]Faction : EndOfDay!!!!!");
            Banker banker = new();

            factionData.SubtractFactionResources();
            factionData.CopyFactionResourcesToYesterday();

            banker.AddLeaderInfluence(factionData);

            // Some research points will be lost because heroes and population will continue to research
            // items that are already done.  The amount should be insignificant in the long run.
            // Generate passive research for all heroes.
            Research.GeneratePassiveResearch(factionData.allHeroesList);

            // Generate passive research for each county population, not including heroes.
            foreach (CountyData countyData in factionData.countiesFactionOwns)
            {
                Research.GeneratePassiveResearch(countyData.populationDataList);
            }

            // Check for completed research and then complete it.
            foreach (ResearchItemData researchItemData in factionData.researchableResearch)
            {
                if (researchItemData.CheckIfResearchDone())
                {
                    researchItemData.CompleteResearch();
                }
            }

            TopBarControl.Instance.UpdateTopBarGoodLabels();
        }

        private void DayStart()
        {
            // This is just commented out until we get to Research.
            GD.PrintRich($"[rainbow]Faction : StartOfDay!!!!!");

            if (factionData != Globals.Instance.playerFactionData)
            {
                //factionAI.AssignResearch(factionData);
            }

            Research.CreateResearchableResearchList(factionData);

            
            // Assign to all heroes passive research
            //Research.AssignPassiveResearch(factionData.allHeroesList);

            // Assign Passive research for each county population, not including heroes.
            foreach (CountyData countyData in factionData.countiesFactionOwns)
            {
                //GD.PrintRich($"[rainbow]{countyData.countyName} is checking population passive research.");
                Research.AssignPassiveResearch(countyData.populationDataList);
            }
            /*
            // This is just for testing.
            foreach (ResearchItemData researchItemData in factionData.researchItems)
            {
                GD.Print($"{factionData.factionName} research in " +
                    $"{researchItemData.researchName}: {researchItemData.AmountOfResearchDone}");
            }
            */
        }

        private void OnTreeExit()
        {
            Clock.Instance.DailyHourOne -= EndOfDay;
            Clock.Instance.DailyHourOne -= DayStart;
        }
    }
}