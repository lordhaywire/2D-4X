using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;

public partial class CountyGeneration : Node
{
    public override void _Ready()
    {
        AssignFactionDataToCountyData();
        AssignCountyDataToFaction();
        UpdateResources();
        UpdateInitialCountyStorage();
    }

    private static void UpdateResources()
    {
        // Assign a copy of each resource to each county.
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            CopyAndAssignResources(county.countyData, AllGoods.Instance.allGoods);
            UpdateScavengableResources(county);
        }
    }

    private static void UpdateScavengableResources(County county)
    {
        county.countyData.scavengableCannedFood = Globals.Instance.maxScavengableFood;
        county.countyData.scavengableRemnants = Globals.Instance.maxScavengableScrap;
    }

    private static void CopyAndAssignResources(CountyData countyData, GoodData[] AllGoods)
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
        SetInitialMaxStorage(countyData.goods);
        // This is just for testing.  Sets all resources to a starting amount.
        // This has to be after the initial storage is set.
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
        {
            keyValuePair.Value.Amount = Globals.Instance.startingAmountOfEachGood;
        }
    }

    private static void SetInitialMaxStorage(Godot.Collections.Dictionary<AllEnums.CountyGoodType, GoodData> resources)
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in resources)
        {
            GoodData goodData = keyValuePair.Value;

            if (goodData.perishable == AllEnums.Perishable.Perishable)
            {
                goodData.MaxAmount = Globals.Instance.startingPerishableStorage
                    / Globals.Instance.numberOfPerishableGoods;
                //+ (Globals.Instance.startingPerishableStorage % Globals.Instance.numberOfPerishableResources);

            }
            else if(goodData.perishable == AllEnums.Perishable.Nonperishable)
            {
                goodData.MaxAmount = Globals.Instance.startingNonperishableStorage
                    / Globals.Instance.numberOfNonperishableGoods;
                //+ (Globals.Instance.startingNonperishableStorage % Globals.Instance.numberOfNonperishableResources);
            }
            //GD.Print($"{county.countyData.countyName} - {resource.name}: " +
            //       $"{resource.MaxAmount}");
        }
    }

    private static void UpdateInitialCountyStorage()
    {
        foreach (County county in Globals.Instance.countiesParent.GetChildren().Cast<County>())
        {
            county.countyData.perishableStorage = Globals.Instance.startingPerishableStorage;
            county.countyData.nonperishableStorage = Globals.Instance.startingNonperishableStorage;
            //GD.Print($"{county.countyData.countyName} has {county.countyData.perishableStorage} perishable storage.");
        }
    }

    // This is just temporary until we set up random faction generation.
    private static void AssignFactionDataToCountyData()
    {
        // Cowlitz
        County selectCounty = (County)Globals.Instance.countiesParent.GetChild(0);
        //GD.Print("Assigning Faction data: " + Globals.Instance.factionDatas[0].factionName);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[0];
        //GD.Print("Assigned Faction Data: " + selectCounty.countyData.factionData.factionName);
        // Tillamook
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(1);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
        // Douglas
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(2);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
        // Portland
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(3);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
        // Wasco
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(4);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[3];
        // Harney
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(5);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[1];
        // Umatilla
        selectCounty = (County)Globals.Instance.countiesParent.GetChild(6);
        selectCounty.countyData.factionData = Globals.Instance.factionDatas[2];
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