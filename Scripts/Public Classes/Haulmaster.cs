using Godot;
using System;
using System.Collections.Generic;

namespace PlayerSpace;
public class Haulmaster
{
    // Gathers stockpiled goods for the county improvement based on its input needs.
    public static void GatherStockpileGoods(CountyData countyData, CountyImprovementData countyImprovementData)
    {
        // Mark the improvement as currently producing.
        countyImprovementData.status = AllEnums.CountyImprovementStatus.Producing;

        foreach (KeyValuePair<GoodData, int> uniqueInputGood in countyImprovementData.uniqueInputGoods)
        {
            // Calculate the desired stockpile range.
            int minStockpileAmount = uniqueInputGood.Value * countyImprovementData.adjustedMaxWorkers 
                * Globals.Instance.minDaysStockpile;
            int maxStockpileAmount = uniqueInputGood.Value * countyImprovementData.adjustedMaxWorkers 
                * Globals.Instance.maxDaysStockpile;

            // Get the county's stockpile and available amount for the required good.
            GoodData countyGood = countyData.goods[uniqueInputGood.Key.countyGoodType];
            GD.Print("CountyImprovement Stockpiled Goods Count: " + countyImprovementData.countyStockpiledGoods.Count);
            GD.Print($"Stockpiled: {countyImprovementData.countyStockpiledGoods[uniqueInputGood.Key.countyGoodType]}");
            GD.Print("County Good: " + countyGood.goodName);
            GD.Print("County Good County Good Type: " + countyGood.countyGoodType);
            GD.Print("Unique Input Good County Good Type: " + uniqueInputGood.Key.countyGoodType);

            GD.Print($"{uniqueInputGood.Key.goodName}, ");
            GD.Print($"{countyImprovementData.improvementName} requires:");
            GD.Print($"Available: {countyGood.Amount}, ");

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
            GD.Print($"Updated {countyImprovementData.improvementName} stockpile for {uniqueInputGood.Key.goodName}: " +
                     $"Available: {countyGood.Amount}, " +
                     $"Stockpiled: {countyImprovementData.countyStockpiledGoods[countyGood.countyGoodType]} " +
                     $"Min Stockpiled: {minStockpileAmount} " + 
                     $"Status: {countyImprovementData.status}");
        }
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
    }
}
