using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using AutoloadSpace;

namespace PlayerSpace;

public partial class CountyGeneration : Node
{
    public override void _Ready()
    {
        // This whole thing could probably be done in 1 foreach.  Maybe?
        AssignFactionDataToCountyData();
        AssignCountyDataToFaction();
        UpdateGoods();
        
        foreach (CountyData countyData in Autoload.Instance.allCountyData)
        {
            Haulmaster.CountCountyMaxStorage(countyData);
            Haulmaster.AssignMaxStorageToGoods(countyData);
        }

        AssignStartingGoodsToCounty();
        AssignTerrainToTerrainList();
        GenerateExplorationEvents();
    }

    private void AssignTerrainToTerrainList()
    {
        // Add all the human set terrains to a Godot Collection for later use.
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            county.countyData.allTerrains =
            [
                county.countyData.primaryTerrain,
                county.countyData.secondaryTerrain,
                county.countyData.tertiaryTerrain
            ];
        }
    }

    private void GenerateExplorationEvents()
    {
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            List<StoryEventData> allEvents = [];

            for (int i = 0; i < county.countyData.allTerrains.Count; i++)
            {
                AllEnums.Terrain terrain = county.countyData.allTerrains[i];

                if (!Autoload.Instance.eventsByTerrainDictionary.TryGetValue(terrain,
                        out List<StoryEventData> terrainEvents))
                    continue;

                int numberOfEvents = i == 0 ? Globals.Instance.numberOfPrimaryTerrainEvents
                    : i == 1 ? Globals.Instance.numberOfSecondaryTerrainEvents
                    : Globals.Instance.numberOfTertiaryTerrainEvents;

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

            county.countyData.explorationEvents = new Godot.Collections.Array<StoryEventData>(allEvents);

            foreach (StoryEventData storyEventData in county.countyData.explorationEvents)
            {
                storyEventData.eventCounty = county;
                GD.Print($"{county.countyData.countyName} {storyEventData.storyEventTitle}");
            }
        }
    }
    
    private void AssignStartingGoodsToCounty()
    {
        // This is just for testing.  Sets all resources to a starting amount.
        // This has to be after the initial storage is set.
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            CountyData countyData = county.countyData;
            foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
            {
                keyValuePair.Value.Amount = Globals.Instance.startingAmountOfEachGood;
            }
        }
    }

    private static void UpdateGoods()
    {
        // Assign a copy of each good to each county.
        foreach (CountyData countyData in Autoload.Instance.allCountyData)
        {
            CopyAndAssignGoods(countyData, Autoload.Instance.allGoodData);
            UpdateScavengeableResources(countyData);
        }
    }

    private static void UpdateScavengeableResources(CountyData countyData)
    {
        countyData.scavengeableCannedFood = Globals.Instance.maxScavengeableFood;
        countyData.scavengeableRemnants = Globals.Instance.maxScavengeableScrap;
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
    private static void AssignFactionDataToCountyData()
    {
        // Cowlitz
        CountyData cowlitzCountyData = Autoload.Instance.allCountyData[0];
        cowlitzCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[0];
        // Tillamook
        CountyData tillamookCountyData = Autoload.Instance.allCountyData[1];
        tillamookCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[1];
        // Douglas
        CountyData douglasCountyData = Autoload.Instance.allCountyData[2];
        douglasCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[1];
        // Portland
        CountyData portlandCountyData = Autoload.Instance.allCountyData[3];
        portlandCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[3];
        // Wasco
        CountyData wascoCountyData = Autoload.Instance.allCountyData[4];
        wascoCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[3];
        // Harney
        CountyData harneyCountyData = Autoload.Instance.allCountyData[5];
        harneyCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[1];
        // Umatilla
        CountyData umatillaCountyData = Autoload.Instance.allCountyData[6];
        umatillaCountyData.factionData = SaveManager.Instance.saveGameData.allFactionDataList[2];
    }

    private static void AssignCountyDataToFaction()
    {
        // This goes through every county and adds itself to the faction data already assigned to the county.
        foreach (CountyData countyData in Autoload.Instance.allCountyData)
        {
            countyData.factionData.countiesFactionOwns.Add(countyData);
            //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
        }
    }
}