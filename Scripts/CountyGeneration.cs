using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class CountyGeneration : Node
{
    public List<CountyData> allCountyData = [];
    private const string CountiesDirectory = "res://Resources/Counties/";
    
    public override void _Ready()
    {
        allCountyData = Autoload.Instance.ReadResourcesFromDisk(CountiesDirectory).Cast<CountyData>().ToList();
        
        // This whole thing could probably be done in 1 foreach.  Maybe?
        AssignFactionDataToCountyData();
        AssignCountyDataToFaction();
        UpdateGoods();
        
        foreach (CountyData countyData in allCountyData)
        {
            Haulmaster.CountCountyMaxStorage(countyData);
            Haulmaster.AssignMaxStorageToGoods(countyData);
        }

        AssignStartingGoodsToCounty();
        AssignTerrainToTerrainList();
        GenerateExplorationEvents();
        ConvertCountyDataListToGodotArrayInSaveGame();
    }

    private void ConvertCountyDataListToGodotArrayInSaveGame()
    {
        SaveManager.Instance.saveGameData.allCountyDataList.Clear();
        foreach (CountyData countyData in allCountyData)
        {
            SaveManager.Instance.saveGameData.allCountyDataList.Add(countyData);
        }
    }
    
    private void AssignTerrainToTerrainList()
    {
        // Add all the human set terrains to a Godot Collection for later use.
        foreach (CountyData countyData in allCountyData)
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
        foreach (CountyData countyData in allCountyData)
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
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in allCountyData.SelectMany(countyData => countyData.goods))
        {
            keyValuePair.Value.Amount = Autoload.Instance.startingAmountOfEachGood;
        }
    }

    private void UpdateGoods()
    {
        // Assign a copy of each good to each county.
        foreach (CountyData countyData in allCountyData)
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
        CountyData cowlitzCountyData = allCountyData[0];
        cowlitzCountyData.factionId = 0;
        // Tillamook
        CountyData tillamookCountyData = allCountyData[1];
        tillamookCountyData.factionId = 1;
        // Douglas
        CountyData douglasCountyData = allCountyData[2];
        douglasCountyData.factionId = 1;
        // Portland
        CountyData portlandCountyData = allCountyData[3];
        portlandCountyData.factionId = 3;
        // Wasco
        CountyData wascoCountyData = allCountyData[4];
        wascoCountyData.factionId = 3;
        // Harney
        CountyData harneyCountyData = allCountyData[5];
        harneyCountyData.factionId = 1;
        // Umatilla
        CountyData umatillaCountyData = allCountyData[6];
        umatillaCountyData.factionId = 2;
    }

    private void AssignCountyDataToFaction()
    {
        // This goes through every county and adds itself to the faction data already assigned to the county.
        foreach (CountyData countyData in allCountyData)
        {
            FactionData factionData = SaveManager.Instance.saveGameData.ConvertFactionIdToFactionData(countyData.factionId);
            factionData.countiesFactionOwns.Add(countyData);
            //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
        }
    }
}