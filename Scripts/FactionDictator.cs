using AutoloadSpace;
using Godot;

namespace PlayerSpace;

public partial class FactionDictator : Node
{
    public override void _Ready()
    {
        CallDeferred(nameof(SubscribeToEvents));
    }

    private void SubscribeToEvents()
    {
        Clock.Instance.DailyHourZeroFirstQuarter += EndOfDay;
        Clock.Instance.DailyHourZeroThirdQuarter += DayStart;
        Clock.Instance.DailyHourZeroFourthQuarter += AfterDayStart;
        Clock.Instance.Weekly += Weekly;
    }

    private void UnsubscribeFromEvents()
    {
        Clock.Instance.DailyHourZeroFirstQuarter -= EndOfDay;
        Clock.Instance.DailyHourZeroThirdQuarter -= DayStart;
        Clock.Instance.DailyHourZeroFourthQuarter -= AfterDayStart;
        Clock.Instance.Weekly -= Weekly;
    }

    private void Weekly()
    {
        GD.PrintRich($"[rainbow]Faction : Weekly!!!!!");
    }

    private void EndOfDay()
    {
        //GD.PrintRich($"[rainbow]Faction : EndOfDay!!!!!");
        Banker banker = new();

        foreach (FactionData factionData in SaveManager.Instance.saveGameData.allFactionDataList)
        {
            factionData.SubtractFactionResources();
            factionData.CopyFactionResourcesToYesterday();

            banker.AddLeaderInfluence(factionData);

            // Some research points will be lost because heroes and population will continue to research
            // items that are already done.  The amount should be insignificant in the long run.
            foreach (CountyData countyData in factionData.countiesFactionOwns)
            {
                // Generate passive research for all heroes.
                Research.GeneratePassiveResearch(countyData.heroesInCountyList);
                // Generate passive research for each county population, not including heroes.
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
    }

    private void DayStart()
    {
        foreach (FactionData factionData in SaveManager.Instance.saveGameData.allFactionDataList)
        {
            // This is just commented out until we get to Research.
            GD.PrintRich($"[rainbow]Faction : StartOfDay!!!!!");

            if (factionData != Autoload.Instance.playerFactionData)
            {
                //factionAI.AssignResearch(factionData);
            }

            Research.CreateResearchableResearchList(factionData);

            // Assign Passive research for each county population, not including heroes.
            foreach (CountyData countyData in factionData.countiesFactionOwns)
            {
                //GD.PrintRich($"[rainbow]{countyData.countyName} is checking population passive research.");
                Research.AssignPassiveResearch(countyData.populationDataList);
            }
        }
    }

    public static void ConvertFactionToDeadFaction(int factionId)
    {
        FactionData factionData = FactionData.GetFactionDataFromId(factionId);
        factionData.factionStatus = AllEnums.FactionStatus.Dead;
    }

    private void AfterDayStart()
    {
        foreach (FactionData factionData in SaveManager.Instance.saveGameData.allFactionDataList)
        {
            if (factionData != Autoload.Instance.playerFactionData)
            {
                FactionAI.DecideIfHeroUsesNewestEquipment(factionData);
            }
        }
    }

    private void OnTreeExit()
    {
        Clock.Instance.DailyHourZeroFirstQuarter -= EndOfDay;
        Clock.Instance.DailyHourZeroFirstQuarter -= DayStart;
    }
}