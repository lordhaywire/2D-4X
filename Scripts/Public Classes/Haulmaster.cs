using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerSpace;
public class Haulmaster
{
    public static void ReturnHalfOfConstructionCost(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.goodsConstructionCost)
        {
            int returnedAmount = keyValuePair.Value / 2;
            if (keyValuePair.Key.goodType == AllEnums.GoodType.CountyGood
                || keyValuePair.Key.goodType == AllEnums.GoodType.Both)
            {
                countyData.goods[keyValuePair.Key.countyGoodType].Amount += returnedAmount;
            }
            else
            {
                countyData.factionData.factionGoods[keyValuePair.Key.factionGoodType].Amount += returnedAmount;
            }
        }
    }

    /// <summary>
    /// Subtracts improvement storage from County.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    public static void SubtractImprovementStorageFromCounty(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods)
        {
            if (keyValuePair.Key.countyGoodType == AllEnums.CountyGoodType.StorageNonperishable)
            {
                countyData.nonperishableStorage -= keyValuePair.Value.storageAmount;
                GD.Print($"{countyData.countyName} now has {countyData.nonperishableStorage} nonperishable storage.");
            }
            else
            {
                countyData.perishableStorage -= keyValuePair.Value.storageAmount;
                GD.Print($"{countyData.countyName} now has {countyData.perishableStorage} perishable storage.");
            }
        }
    }

    /// <summary>
    /// Count all the available county storage. This is dumb.
    /// </summary>
    /*
    public static void AssignMaxStorageToCountyStorage(CountyData countyData)
    {
        countyData.perishableStorage = Globals.Instance.startingPerishableStorage;
        countyData.nonperishableStorage = Globals.Instance.startingNonperishableStorage;
        GD.Print($"{countyData.countyName} has {countyData.perishableStorage} perishable storage.");
        GD.Print($"{countyData.countyName} has {countyData.nonperishableStorage} perishable storage.");
    }
    */

    public static void CountCountyMaxStorage(CountyData countyData)
    {
        countyData.nonperishableStorage = Globals.Instance.startingNonperishableStorage;
        countyData.perishableStorage = Globals.Instance.startingPerishableStorage;
        GD.Print("Initial County Storage: " + countyData.nonperishableStorage);
        foreach (CountyImprovementData countyImprovementData in countyData.completedCountyImprovementList)
        {
            if(countyImprovementData.countyImprovementType == AllEnums.CountyImprovementType.Storage)
            {
                foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods) 
                {
                    if(keyValuePair.Key.countyGoodType == AllEnums.CountyGoodType.StorageNonperishable)
                    {
                        countyData.nonperishableStorage += keyValuePair.Value.storageAmount;
                    }
                    else
                    {
                        countyData.perishableStorage += keyValuePair.Value.storageAmount;
                    }
                }
            }
        }
    }
    public static void AssignMaxStorageToGoods(CountyData countyData)
    {
        foreach (KeyValuePair<AllEnums.CountyGoodType, GoodData> keyValuePair in countyData.goods)
        {
            GoodData goodData = keyValuePair.Value;

            if (goodData.perishable == AllEnums.Perishable.Perishable)
            {
                goodData.MaxAmount = countyData.perishableStorage
                    / Globals.Instance.numberOfPerishableGoods;

            }
            else if (goodData.perishable == AllEnums.Perishable.Nonperishable)
            {
                goodData.MaxAmount = countyData.nonperishableStorage
                    / Globals.Instance.numberOfNonperishableGoods;
            }
            //GD.Print($"AssignMaxStorageToGoods: {countyData.countyName} : {goodData.goodName}: {goodData.MaxAmount}");
        }
    }

    /// <summary>
    /// Adds county improvement storage to the county.
    /// </summary>
    /// <param name="countyData"></param>
    /// <param name="countyImprovementData"></param>
    public static void AddImprovementStorageToCounty(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, ProductionData> keyValuePair in countyImprovementData.outputGoods)
        {
            if (keyValuePair.Key.countyGoodType == AllEnums.CountyGoodType.StorageNonperishable)
            {
                countyData.nonperishableStorage += keyValuePair.Value.storageAmount;
                GD.Print($"{countyData.countyName} now has {countyData.nonperishableStorage} nonperishable storage.");
            }
            else
            {
                countyData.perishableStorage += keyValuePair.Value.storageAmount;
                GD.Print($"{countyData.countyName} now has {countyData.perishableStorage} perishable storage.");
            }
        }
    }

    // Gathers stockpiled goods for the county improvement based on its input needs.
    public static void GatherStockpileGoods(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        // Mark the improvement as currently producing.
        countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;

        // This includes if the county improvement already just has remnants as a good.
        int minNumberOfRemnantsUsedForInputGoods = 0; // GetNumberOfRemnantsNeededAsInputGoods(countyImprovementData);
        int maxNumberOfRemnantsUsedForInputGoods = 0; // GetNumberOfRemnantsNeededAsInputGoods(countyImprovementData);

        foreach (KeyValuePair<GoodData, int> uniqueInputGood in countyImprovementData.uniqueInputGoods)
        {
            // Calculate the desired stockpile range.
            int minStockpileAmount = uniqueInputGood.Value * countyImprovementData.adjustedMaxWorkers
                * Globals.Instance.minDaysStockpile;
            int maxStockpileAmount = uniqueInputGood.Value * countyImprovementData.adjustedMaxWorkers
                * Globals.Instance.maxDaysStockpile;

            // Add the amount of stockpiled remnants depending on the number of unique input goods that are using remnants.
            if (uniqueInputGood.Key.countyGoodType == AllEnums.CountyGoodType.Remnants || uniqueInputGood.Key.useRemnants)
            {
                minNumberOfRemnantsUsedForInputGoods += minStockpileAmount;
                maxNumberOfRemnantsUsedForInputGoods += maxStockpileAmount;
                minStockpileAmount = minNumberOfRemnantsUsedForInputGoods;
                maxStockpileAmount = maxNumberOfRemnantsUsedForInputGoods;
            }

            // Get the county's stockpile and available amount for the required good.
            GoodData countyGood = countyData.goods[uniqueInputGood.Key.countyGoodType];
            /*
            GD.Print("CountyImprovement Stockpiled Goods Count: " + countyImprovementData.countyStockpiledGoods.Count);
            GD.Print($"Stockpiled: {countyImprovementData.countyStockpiledGoods[uniqueInputGood.Key.countyGoodType]}");
            GD.Print("County Good: " + countyGood.goodName);
            GD.Print("County Good County Good Type: " + countyGood.countyGoodType);
            GD.Print("Unique Input Good County Good Type: " + uniqueInputGood.Key.countyGoodType);

            GD.Print($"{uniqueInputGood.Key.goodName}, ");
            GD.Print($"{countyImprovementData.improvementName} requires:");
            GD.Print($"Available: {countyGood.Amount}, ");
            */
            // Skip if the current stockpile meets or exceeds the maximum desired amount.
            if (countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]
                >= maxStockpileAmount)
            {
                continue;
            }

            // Determine how much to transfer from the county's stock to the improvement's stockpile.
            int amountToStockpile = Math.Min(minStockpileAmount, countyGood.Amount);

            // Transfer goods and update stockpile.
            if (amountToStockpile > 0)
            {
                countyGood.Amount -= amountToStockpile;
                countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType] += amountToStockpile;
            }

            // If the stockpile is below the minimum desired amount, update the improvement status and
            // add a day to each person's employed but idle count.
            if (countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]
                < minStockpileAmount)
            {
                countyImprovementData.status = AllEnums.CountyImprovementStatus.LowStockpiledGoods;
            }

            // Log the post-transfer state.
            /*
            GD.Print($"Updated {countyImprovementData.improvementName} stockpile for {uniqueInputGood.Key.goodName}: " +
                     $"Available: {countyGood.Amount}, " +
                     $"Stockpiled: {countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]} " +
                     $"Min Stockpiled: {minStockpileAmount} " +
                     $"Status: {countyImprovementData.status}");
            */
        }
    }

    public static bool CheckEnoughGoods(CountyImprovementData countyImprovementData, PopulationData populationData)
    {
        bool hasEnoughInputGoods = true;

        foreach (KeyValuePair<GoodData, int> inputGood in countyImprovementData.uniqueInputGoods)
        {
            int stockpileAmount = countyImprovementData.countyStockpiledGoods[inputGood.Key.countyGoodType];
            //GD.Print($"{populationData.location} Input Good vs Stockpile amount: {inputGood.Value} " +
            //    $"vs {stockpileAmount}");
            if (inputGood.Value > stockpileAmount)
            {
                hasEnoughInputGoods = false;

                break; // No need to check further if one good is insufficient.
            }
        }
        return hasEnoughInputGoods;
    }
    public static void DeductStockPiledGoods(CountyImprovementData countyImprovementData)
    {
        // Deduct input goods and perform work actions.
        foreach (KeyValuePair<GoodData, int> inputGood in countyImprovementData.uniqueInputGoods)
        {
            countyImprovementData.countyStockpiledGoods[inputGood.Key.countyGoodType]
                -= inputGood.Value;
        }
    }

    /// <summary>
    /// This has to generate the stock piled goods from the developer created inputGoods list.  The inputGoods list
    /// is later copied to the uniqueList because it has to be.
    /// If the player selects Use Remnants then this will break.
    /// </summary>
    /// <param name="countyImprovementData"></param>
    public static void GenerateStockpileGoodsDictionary(CountyImprovementData countyImprovementData)
    {
        foreach (KeyValuePair<GoodData, int> keyValuePair in countyImprovementData.inputGoods)
        {
            countyImprovementData.countyStockpiledGoods[keyValuePair.Key.countyGoodType] = 0;
        }
        // If this county improvement's countyStockpiledGoods doesn't contain remnants then add it.
        if (!countyImprovementData.countyStockpiledGoods.TryGetValue(AllEnums.CountyGoodType.Remnants, out int value))
        {
            countyImprovementData.countyStockpiledGoods[AllEnums.CountyGoodType.Remnants] = 0;
            GD.Print($"Remnants added to {countyImprovementData.improvementName}.");
        }
        else
        {
            GD.Print($"It already has {value}" +
                $" remnants in the countyStockedpiledGoods at {countyImprovementData.improvementName}");
        }
    }
}
