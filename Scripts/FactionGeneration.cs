using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;
using GameGeneration;
using Godot;

namespace PlayerSpace;

public partial class FactionGeneration : Node
{
    private string factionDirectory = "res://Resources/Factions/";

    //private List<FactionData> allFactionDataList = [];

    public override void _Ready()
    {
        CreateFactionsFromDisk();
        NotifyPropertyListChanged();
    }

    private void CreateFactionsFromDisk()
    {
        Godot.Collections.Array<Resource> loadedResources =
            Autoload.Instance.ReadResourcesFromDisk(factionDirectory);

        Godot.Collections.Array<FactionData> factionDataArray = new Godot.Collections.Array<FactionData>();

        foreach (Resource res in loadedResources)
        {
            if (res is FactionData factionData)
            {
                factionDataArray.Add(factionData);
            }
        }

        SaveManager.Instance.saveGameData.allFactionDataList = factionDataArray;
        for (int i = 0; i < SaveManager.Instance.saveGameData.allFactionDataList.Count; i++)
        {
            SaveManager.Instance.saveGameData.allFactionDataList[i].factionId = i;

            if (SaveManager.Instance.saveGameData.allFactionDataList[i].isPlayer)
            {
                Autoload.Instance.playerFactionData = SaveManager.Instance.saveGameData.allFactionDataList[i];
            }

            GD.Print(
                $"{SaveManager.Instance.saveGameData.allFactionDataList[i].factionName} has been loaded from disk.");
            // The order is important.
            CreateFactionGoodDictionary(SaveManager.Instance.saveGameData.allFactionDataList[i]);
            AddFactionsToDiplomacyWar(SaveManager.Instance.saveGameData.allFactionDataList[i]);
            AddStartingResearch(SaveManager.Instance.saveGameData.allFactionDataList[i]);
            //ConvertFactionDataListsToGodotArrayInAutoload(allFactionDataList);
        }
    }

    private static void ConvertFactionDataListsToGodotArrayInAutoload(List<FactionData> allFactionDataListCSharp)
    {
        SaveManager.Instance.saveGameData.allFactionDataList.Clear();
        foreach (FactionData factionData in allFactionDataListCSharp)
        {
            SaveManager.Instance.saveGameData.allFactionDataList.Add(factionData);
        }
    }

    private void AddStartingResearch(FactionData factionData)
    {
        foreach (ResearchItemData researchItemData in Autoload.Instance.allResearchItemData)
        {
            //GD.Print("Faction ID that is getting assigned: " + factionData.factionID);
            // Todo: What the fuck is this?  Why are we doing?
            researchItemData.factionId = factionData.factionId;
            //GD.PrintRich($"[rainbow]{FactionData.GetFactionDataFromID(researchItemData.factionID).factionName}: {researchItemData.researchName}");

            ResearchItemData researchItemDataCopy = researchItemData.NewCopy(researchItemData);
            if (researchItemDataCopy.researchedAtStart)
            {
                // We need to add some randomness to the starting factions starting research, except
                // for the player factions.
                researchItemDataCopy.AmountOfResearchDone = researchItemDataCopy.costOfResearch;
                AssignResearchedCountyImprovements(factionData, researchItemDataCopy);
            }

            factionData.researchItems.Add(researchItemDataCopy);
        }
    }

    private void AssignResearchedCountyImprovements(FactionData currentFactionData, ResearchItemData researchItemData)
    {
        GD.PrintRich($"[rainbow]Adding county improvements for! " + researchItemData.researchName);

        GameGenerationCanvasLayer.Instance.UpdateStatusLabelText(
            $"{Tr("PHRASE_RESEARCH_FOR")} {Tr(researchItemData.researchName)} {Tr("PHRASE_HAS_BEEN_COMPLETED")}.");

        if (researchItemData.countyImprovementDatas.Length > 0)
        {
            foreach (CountyImprovementData countyImprovementData in researchItemData.countyImprovementDatas)
            {
                // This is to set the starting adjusted max builders and workers.
                countyImprovementData.adjustedMaxBuilders = countyImprovementData.maxBuilders;
                countyImprovementData.adjustedMaxWorkers = countyImprovementData.maxWorkers;

                currentFactionData.AddCountyImprovementToAllCountyImprovements(countyImprovementData);
            }
        }
    }

    private static void CreateFactionGoodDictionary(FactionData factionData)
    {
        foreach (GoodData goodData in Autoload.Instance.allGoodData)
        {
            if (goodData.goodType == AllEnums.GoodType.CountyGood)
            {
                continue;
            }

            factionData.factionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
            factionData.yesterdaysFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
            factionData.amountUsedFactionGoods.Add(goodData.factionGoodType, (GoodData)goodData.Duplicate());
        }

        // This is for testing.  We are going to have a different, more random way of
        // generating starting resources for each faction.
        // Todo - Make a developer option that allows these numbers to be changed.
        factionData.factionGoods[AllEnums.FactionGoodType.Influence].Amount = 1500;
        factionData.factionGoods[AllEnums.FactionGoodType.Money].Amount = 1500;
        
        foreach (KeyValuePair<AllEnums.FactionGoodType, GoodData> keyValuePair in factionData.factionGoods)
        {
            GD.Print($"{keyValuePair.Value.goodName}:{keyValuePair.Value.Amount} has been added to {factionData.factionName}");
        }
    }

    private void AddFactionsToDiplomacyWar(FactionData factionData)
    {
        //GD.Print("Faction Name: " + factionData.factionName);
        foreach (FactionData warFactionData in
                 SaveManager.Instance.saveGameData.allFactionDataList.Where(warFactionData =>
                     warFactionData != factionData))
        {
            // Add warFactionData to factionWarDictionary with a default value of false
            factionData.factionWarDictionary[warFactionData.factionName] = false;
        }
    }
}