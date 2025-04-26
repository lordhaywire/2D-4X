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
            UpdateScavengableResources(county);
        }
    }

    private static void UpdateScavengableResources(County county)
    {
        county.countyData.scavengeableCannedFood = Globals.Instance.maxScavengeableFood;
        county.countyData.scavengeableRemnants = Globals.Instance.maxScavengeableScrap;
    }

    private static void CopyAndAssignGoods(CountyData countyData, GoodData[] AllGoods)
    {
        foreach (GoodData goodData in AllGoods)
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
        //GD.Print("Assigning Faction data: " + Globals.Instance.factionDatas[0].factionName);
        selectCounty.countyData.factionData = Globals.Instance.allFactionData[0];
        //GD.Print("Assigned Faction Data: " + selectCounty.countyData.factionData.factionName);
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
        // This goes through every county and adds itself to the faction data that is already assigned to the county.
        foreach (County selectCounty in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            selectCounty.countyData.factionData.countiesFactionOwns.Add(selectCounty.countyData);
            //GD.Print($"Faction: {selectCounty.countyData.factionData.factionName} {selectCounty.countyData.countyName}");
        }
    }


}