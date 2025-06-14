using Godot;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class CountyGeneration : Node
{
    public override void _Ready()
    {
        // This whole thing could probably be done in 1 foreach.  Maybe?
        AssignFactionDataToCountyData();
        AssignCountyDataToFaction();
        UpdateGoods();
        //PrebuildCountyImprovements(); // Currently Empty.
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            Haulmaster.CountCountyMaxStorage(county.countyData);
            Haulmaster.AssignMaxStorageToGoods(county.countyData);
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

                if (!StoryEventList.Instance.eventsByTerrainDictionary.TryGetValue(terrain,
                        out List<StoryEventData> terrainEvents))
                    continue;

                int numberOfEvents = i == 0 ? Globals.Instance.numberOfPrimaryTerrainEvents : i == 1 ? Globals.Instance.numberOfSecondaryTerrainEvents : Globals.Instance.numberOfTertiaryTerrainEvents;

                List<StoryEventData> selectedEvents = [];
                for (int j = 0; j < terrainEvents.Count && selectedEvents.Count < numberOfEvents; j++)
                {
                    int randIndex = (int)(GD.Randi() % (ulong)(j + 1));
                    selectedEvents.Insert(randIndex, (StoryEventData)terrainEvents[j].Duplicate());
                }

                allEvents.AddRange(selectedEvents);
            }

            // Shuffle combined events
            for (int i = allEvents.Count - 1; i > 0; i--)
            {
                int randIndex = (int)(GD.Randi() % (ulong)(i + 1));
                (allEvents[i], allEvents[randIndex]) = (allEvents[randIndex], allEvents[i]);
            }

            // Convert to Godot.Collections.Array and assign
            county.countyData.explorationEvents = new Godot.Collections.Array<StoryEventData>(allEvents);
            
            // Assign the county to each story event so the story event knows where it is happening.
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
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            CopyAndAssignGoods(county.countyData, AllGoods.Instance.allGoods);
            UpdateScavengeableResources(county);
        }
    }

    private static void UpdateScavengeableResources(County county)
    {
        county.countyData.scavengeableCannedFood = Globals.Instance.maxScavengeableFood;
        county.countyData.scavengeableRemnants = Globals.Instance.maxScavengeableScrap;
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
        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(0);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[0];
        // Tillamook
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(1);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[1];
        // Douglas
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(2);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[1];
        // Portland
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(3);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[3];
        // Wasco
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(4);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[3];
        // Harney
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(5);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[1];
        // Umatilla
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(6);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[2];
    }

    private static void AssignCountyDataToFaction()
    {
        // This goes through every county and adds itself to the faction data already assigned to the county.
        foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
            //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
        }
    }
}