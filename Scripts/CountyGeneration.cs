using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class CountyGeneration : Node
{
    //private List<CountyData> allCountyData = [];
    private const string CountiesDirectory = "res://Resources/Counties/";
    
    public override void _Ready()
    {
        Godot.Collections.Array<Resource> loadedResources =
            Autoload.Instance.ReadResourcesFromDisk(CountiesDirectory);

        Godot.Collections.Array<CountyData> countyDataArray = new Godot.Collections.Array<CountyData>();

        foreach (Resource res in loadedResources)
        {
            if (res is CountyData countyData)
            {
                countyDataArray.Add(countyData);
            }
        }

        SaveManager.Instance.saveGameData.allCountyDataList = countyDataArray;

        // This whole thing could probably be done in 1 foreach.  Maybe?
        AssignFactionDataToCountyData();
        AssignCountyDataToFaction();
        UpdateGoods();
        
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            Haulmaster.CountCountyMaxStorage(countyData);
            Haulmaster.AssignMaxStorageToGoods(countyData);
        }

        AssignStartingGoodsToCounty();
        AssignTerrainToTerrainList();
        GenerateExplorationEvents();
        //ConvertCountyDataListToGodotArrayInSaveGame();
    }

    private void ConvertCountyDataListToGodotArrayInSaveGame()
    {
        SaveManager.Instance.saveGameData.allCountyDataList.Clear();
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            SaveManager.Instance.saveGameData.allCountyDataList.Add(countyData);
        }
    }
    
    private void AssignTerrainToTerrainList()
    {
        // Add all the human set terrains to a Godot Collection for later use.
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            countyData.allTerrains =
            [
                countyData.primaryTerrain,
                countyData.secondaryTerrain,
                countyData.tertiaryTerrain
            ];
        }
    }

    private void GenerateExplorationEvents()
    {
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            List<StoryEventData> allEvents = [];

            for (int i = 0; i < countyData.allTerrains.Count; i++)
            {
                AllEnums.Terrain terrain = countyData.allTerrains[i];

                if (!Autoload.Instance.eventsByTerrainDictionary.TryGetValue(terrain,
                        out List<StoryEventData> terrainEvents))
                    continue;

                int numberOfEvents = i == 0 ? Autoload.Instance.numberOfPrimaryTerrainEvents
                    : i == 1 ? Autoload.Instance.numberOfSecondaryTerrainEvents
                    : Autoload.Instance.numberOfTertiaryTerrainEvents;

                List<StoryEventData> selectedEvents = [];

                List<StoryEventData> shuffledTerrainEvents = new List<StoryEventData>(terrainEvents);
                for (int j = shuffledTerrainEvents.Count - 1; j > 0; j--)
                {
                    int randIndex = (int)(GD.Randi() % (ulong)(j + 1));
                    (shuffledTerrainEvents[j], shuffledTerrainEvents[randIndex]) =
                        (shuffledTerrainEvents[randIndex], shuffledTerrainEvents[j]);
                }

                for (int j = 0; j < Math.Min(numberOfEvents, shuffledTerrainEvents.Count); j++)
                {
                    selectedEvents.Add((StoryEventData)shuffledTerrainEvents[j].Duplicate());
                }

                allEvents.AddRange(selectedEvents);
            }

            // Shuffle combined events
            for (int i = allEvents.Count - 1; i > 0; i--)
            {
                int randIndex = (int)(GD.Randi() % (ulong)(i + 1));
                (allEvents[i], allEvents[randIndex]) = (allEvents[randIndex], allEvents[i]);
            }

            countyData.explorationEvents = new Godot.Collections.Array<StoryEventData>(allEvents);

            foreach (StoryEventData storyEventData in countyData.explorationEvents)
            {
                storyEventData.eventCounty = countyData.countyNode;
                GD.Print($"{countyData.countyName} {storyEventData.storyEventTitle}");
            }
        }
    }
    
    private void AssignStartingGoodsToCounty()
    {
        // This is just for testing.  Sets all resources to a starting amount.
        // This has to be after the initial storage is set.
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in SaveManager.Instance.saveGameData.allCountyDataList.SelectMany(countyData => countyData.goods))
        {
            keyValuePair.Value.Amount = Autoload.Instance.startingAmountOfEachGood;
        }
    }

    private void UpdateGoods()
    {
        // Assign a copy of each good to each county.
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            CopyAndAssignGoods(countyData, Autoload.Instance.allGoodData);
            UpdateScavengeableResources(countyData);
        }
    }

    private static void UpdateScavengeableResources(CountyData countyData)
    {
        countyData.scavengeableCannedFood = Autoload.Instance.maxScavengeableFood;
        countyData.scavengeableRemnants = Autoload.Instance.maxScavengeableScrap;
    }

    private static void CopyAndAssignGoods(CountyData countyData, List<GoodData> allGoods)
    {
        foreach (GoodData goodData in allGoods)
        {
            if (goodData.countyGoodType != AllEnums.CountyGoodType.None)
            {
                countyData.goods.Add(goodData.countyGoodType, (GoodData)goodData.Duplicate());
                countyData.yesterdaysGoods.Add(goodData.countyGoodType, (GoodData)goodData.Duplicate());
                countyData.amountOfGoodsUsed.Add(goodData.countyGoodType, (GoodData)goodData.Duplicate());
            }
        }
    }

    // This is just temporary until we set up random faction generation.
    private void AssignFactionDataToCountyData()
    {
        // Cowlitz
        SaveManager.Instance.saveGameData.allCountyDataList[0].factionId = 0;
        // Tillamook
        SaveManager.Instance.saveGameData.allCountyDataList[1].factionId = 1;
        // Douglas
        SaveManager.Instance.saveGameData.allCountyDataList[2].factionId = 1;
        // Portland
        SaveManager.Instance.saveGameData.allCountyDataList[3].factionId = 3;
        // Wasco
        SaveManager.Instance.saveGameData.allCountyDataList[4].factionId = 3;
        // Harney
        SaveManager.Instance.saveGameData.allCountyDataList[5].factionId = 1;
        // Umatilla
        SaveManager.Instance.saveGameData.allCountyDataList[6].factionId = 2;
    }

    private void AssignCountyDataToFaction()
    {
        // This goes through every county and adds itself to the faction data already assigned to the county.
        foreach (CountyData countyData in SaveManager.Instance.saveGameData.allCountyDataList)
        {
            FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(countyData.factionId);
            factionData.countiesFactionOwns.Add(countyData);
            //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
        }
    }
}